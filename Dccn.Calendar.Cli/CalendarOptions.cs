using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Dccn.Calendar.Cli
{
    [UsedImplicitly(ImplicitUseKindFlags.Assign, ImplicitUseTargetFlags.WithMembers)]
    public class CalendarOptions
    {
        public Uri ExchangeUrl { get; set; }

        public string Username { get; set; }
        public string Password { get; set; }

        public string Source { get; set; }
        public string Filter { get; set; }
    }
}