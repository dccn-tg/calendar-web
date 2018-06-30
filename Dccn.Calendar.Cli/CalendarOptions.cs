using System;
using System.Collections.Generic;

namespace Dccn.Calendar.Cli
{
    public class CalendarOptions
    {
        public Uri ExchangeUrl { get; set; }

        public string Username { get; set; }
        public string Password { get; set; }

        public int? MaxEvents { get; set; }

        public string OutputPath { get; set; }

        public IDictionary<string, string> Calendars { get; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    }
}