using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dccn.Calendar.Web.Configuration;
using Microsoft.Extensions.Options;

namespace Dccn.Calendar.Web.Services
{
    public class CalendarService : ICalendarService
    {
        private readonly CalendarOptions _options;
        private readonly IDictionary<string, Calendar> _calendarCache;
        private readonly IDictionary<string, CalendarClient> _clients;

        public CalendarService(IOptions<CalendarOptions> options)
        {
            _options = options.Value;
            // Thread safety: because the service is scoped, it might have to process multiple requests in parallel.
            // The cache needs to be able to deal with this.
            // See https://blog.stephencleary.com/2017/03/aspnetcore-synchronization-context.html
            _calendarCache = new ConcurrentDictionary<string, Calendar>(StringComparer.OrdinalIgnoreCase);
            _clients = new ConcurrentDictionary<string, CalendarClient>(StringComparer.OrdinalIgnoreCase);
        }

        public async Task<(bool Success, Calendar Calendar)> TryGetCalendarAsync(string calendarId)
        {
            if (calendarId == null)
            {
                return (false, null);
            }

            if (_calendarCache.TryGetValue(calendarId, out var cachedCalendar))
            {
                return (true, cachedCalendar);
            }

            if (!_options.Calendars.TryGetValue(calendarId, out var calendarOptions))
            {
                return (false, null);
            }

            return (true, await FetchCalendarAsync(calendarId, calendarOptions));
        }

        public async Task<IEnumerable<Calendar>> GetCalendarsAsync(bool overview)
        {
            var tasks = _options.Calendars
                .Select(pair => new {Id = pair.Key, Options = pair.Value})
                .Where(pair => !overview || pair.Options.ShowInTodayOverview)
                .Select(pair => FetchCalendarAsync(pair.Id, pair.Options));

            return await Task.WhenAll(tasks);
        }

        private async Task<Calendar> FetchCalendarAsync(string calendarId, CalendarOptions.Calendar calendarOptions)
        {
            if (_calendarCache.TryGetValue(calendarId, out var calendar))
            {
                return calendar;
            }

            var sourceId = calendarOptions.Source;
            if (!_clients.TryGetValue(sourceId, out var client))
            {
                var source = _options.Sources[sourceId];
                client = new CalendarClient(source.ExchangeUrl, source.Username, source.Password);

                if (_options.MaxEvents.HasValue)
                {
                    client.MaxEvents = _options.MaxEvents.Value;
                }

                _clients[sourceId] = client;
            }

            calendar = new Calendar(await client.GetCalendarByIdAsync(calendarOptions.ExchangeId));
            if (calendarOptions.Name != null)
            {
                calendar.Name = calendarOptions.Name;
            }

            calendar.Id = calendarId;
            calendar.Location = calendarOptions.Location;

            _calendarCache.Add(calendarId, calendar);
            return calendar;
        }
    }

    public interface ICalendarService
    {
        Task<(bool Success, Calendar Calendar)> TryGetCalendarAsync(string calendarId);
        Task<IEnumerable<Calendar>> GetCalendarsAsync(bool overview = false);
    }
}
