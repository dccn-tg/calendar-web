using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dccn.Calendar.Web.Configuration;
using Microsoft.Extensions.Options;

namespace Dccn.Calendar.Web.Services
{
    public class CalendarService : ICalendarService
    {
        private readonly CalendarOptions _options;
        private readonly IDictionary<string, Calendar> _calendarCache;

        public CalendarService(IOptions<CalendarOptions> options)
        {
            _options = options.Value;
            _calendarCache = new Dictionary<string, Calendar>(StringComparer.OrdinalIgnoreCase);
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

            var sourceId = calendarOptions.Source;
            var source = _options.Sources[sourceId];

            var client = new CalendarClient(source.ExchangeUrl, source.Username, source.Password);
            if (_options.MaxEvents.HasValue)
            {
                client.MaxEvents = _options.MaxEvents.Value;
            }

            var calendar = await client.GetCalendarByIdAsync(calendarOptions.ExchangeId);
            if (calendarOptions.Name != null)
            {
                calendar.Name = calendarOptions.Name;
            }

            _calendarCache.Add(calendarId, calendar);
            return (true, calendar);
        }
    }

    public interface ICalendarService
    {
        Task<(bool Success, Calendar Calendar)> TryGetCalendarAsync(string calendarId);
    }
}
