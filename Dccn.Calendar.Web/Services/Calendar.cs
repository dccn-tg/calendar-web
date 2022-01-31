using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dccn.Calendar.Web.Services
{
    public class Calendar
    {
        private readonly Dccn.Calendar.Calendar _calendar;

        public Calendar(Dccn.Calendar.Calendar calendar)
        {
            _calendar = calendar;
            Id = _calendar?.Id;
            Name = calendar?.Name;
            IsValid = calendar != null;
        }

        public bool IsValid { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }

        public Task<IEnumerable<Event>> EventsRangeAsync(DateTime start, DateTime end)
        {
            return _calendar.EventsRangeAsync(start, end);
;       }

        public Task<IEnumerable<Event>> EventsRangeAsync(DateTime start, TimeSpan duration)
        {
            return _calendar.EventsRangeAsync(start, duration);
        }
    }
}
