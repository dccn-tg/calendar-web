using System;
using System.Collections.Generic;

namespace Dccn.Calendar.Web.Configuration
{
    public class CalendarOptions
    {
        public int? MaxEvents { get; set; }
        public IDictionary<string, Source> Sources { get; } = new Dictionary<string, Source>(StringComparer.OrdinalIgnoreCase);
        public IDictionary<string, Calendar> Calendars { get; } = new Dictionary<string, Calendar>(StringComparer.OrdinalIgnoreCase);

        public class Source
        {
            public Uri ExchangeUrl { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
        }

        public class Calendar
        {
            public string Name { get; set; }
            public string Location { get; set; }
            public string Source { get; set; }
            public string ExchangeId { get; set; }
            public bool ShowInTodayOverview { get; set; } = false;
        }
    }
}
