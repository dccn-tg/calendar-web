using System;
using System.Collections.Generic;

namespace Dccn.Calendar.Web.Configuration
{
    public class CalendarOptions
    {
        public string TenantId { get; set; }
        public string ClientId { get; set; }
        public string Certificate { get; set; }
        public int? MaxEvents { get; set; }
        public IDictionary<string, Calendar> Calendars { get; } = new Dictionary<string, Calendar>(StringComparer.OrdinalIgnoreCase);

        public class Calendar
        {
            public string Name { get; set; }
            public string Location { get; set; }
            public string MailBox { get; set; }
            public string ExchangeId { get; set; }
            public string OverrideEventTitle { get; set; }
            public bool ShowInTodayOverview { get; set; } = false;
        }
    }
}
