using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dccn.Calendar.Web.Configuration;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Dccn.Calendar.Web.Services
{
    public class CalendarService : ICalendarService
    {
        private readonly CalendarOptions _options;
        private readonly CalendarClient _client;
        private readonly IMemoryCache _calendarCache;

        public CalendarService(IOptions<CalendarOptions> options, IMemoryCache calendarCache)
        {
            _options = options.Value;
            _client = new CalendarClient(_options.TenantId, _options.ClientId, _options.Certificate);
            _calendarCache = calendarCache;
        }

        public async Task<Calendar> TryGetCalendarAsync(string calendarId)
        {
            if (calendarId == null)
            {
                return null;
            }

            if (_calendarCache.TryGetValue<Calendar>(calendarId, out var cachedCalendar))
            {
                return cachedCalendar;
            }

            if (!_options.Calendars.TryGetValue(calendarId, out var calendarOptions))
            {
                return null;
            }

            return await FetchCalendarAsync(calendarId, calendarOptions);
        }

        public async Task<IEnumerable<Calendar>> GetCalendarsAsync(bool overview)
        {
            var tasks = _options.Calendars
                .Select(pair => new {Id = pair.Key, Options = pair.Value})
                .Where(pair => !overview || pair.Options.ShowInTodayOverview)
                .Select(pair => FetchCalendarAsync(pair.Id, pair.Options));

            return await Task.WhenAll(tasks);
        }

        public async Task<IEnumerable<Calendar.Event>> GetOverviewEventsAsync(DateTime? date)
        {
            var calendars = (await GetCalendarsAsync(true)).ToList();
            var start = date.GetValueOrDefault(DateTime.Today);
            var end = start.AddDays(1).AddTicks(-1);
            var events = await _client.GetEventsAsync(calendars.Select(calendar => calendar.Inner), start, end);

            return events.Select(@event => new Calendar.Event(@event, calendars.First(calendar => calendar.Inner == @event.Calendar)));
        }

        public async Task<IEnumerable<Calendar.Event>> GetEventsRangeAsync(Calendar calendar, DateTime start, DateTime end)
        {
            return await calendar.EventsRangeAsync(_client, start, end);
        }

        private Task<Calendar> FetchCalendarAsync(string calendarId, CalendarOptions.Calendar calendarOptions)
        {
            return _calendarCache.GetOrCreateAsync(calendarId, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1);

                var calendar = new Calendar(
                    await _client.GetCalendarByIdAsync(calendarOptions.MailBox, calendarOptions.ExchangeId));
                if (calendarOptions.Name != null)
                {
                    calendar.Name = calendarOptions.Name;
                }

                calendar.Id = calendarId;
                calendar.Location = calendarOptions.Location;
                calendar.OverrideEventTitle = calendarOptions.OverrideEventTitle;

                return calendar;
            });
        }
    }

    public interface ICalendarService
    {
        Task<Calendar> TryGetCalendarAsync(string calendarId);
        Task<IEnumerable<Calendar>> GetCalendarsAsync(bool overview = false);
        Task<IEnumerable<Calendar.Event>> GetEventsRangeAsync(Calendar calendar, DateTime start, DateTime end);
        Task<IEnumerable<Calendar.Event>> GetOverviewEventsAsync(DateTime? date);
    }
}
