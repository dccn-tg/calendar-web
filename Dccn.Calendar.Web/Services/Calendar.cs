using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dccn.Calendar.Web.Services
{
    public class Calendar
    {
        public Calendar(Dccn.Calendar.Calendar calendar)
        {
            Inner = calendar;
            Id = Inner?.Id;
            Name = calendar?.Name;
            IsValid = calendar != null;
        }

        public Dccn.Calendar.Calendar Inner { get; }

        public bool IsValid { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string OverrideEventTitle { get; set; }
        public string MailBox => Inner.MailBox;

        public async Task<IEnumerable<Event>> EventsRangeAsync(CalendarClient client, DateTime start, DateTime end)
        {
            return (await Inner.EventsRangeAsync(client, MailBox, start, end))
                .Select(@event => new Event(@event, this));
;       }

        public async Task<IEnumerable<Event>> EventsRangeAsync(CalendarClient client, DateTime start, TimeSpan duration)
        {
            return (await Inner.EventsRangeAsync(client, MailBox, start, duration))
                .Select(@event => new Event(@event, this));
        }

        public class Event
        {
            public Event(Dccn.Calendar.Event @event, Calendar calendar)
            {
                Calendar = calendar;
                Inner = @event;
            }

            public Dccn.Calendar.Event Inner { get; }

            public string Id => Inner.Id;

            public Calendar Calendar { get; }

            public string Title => Calendar.OverrideEventTitle ?? Inner.Title;

            public DateTime Start => Inner.Start;
            public DateTime End => Inner.End;

            public bool AllDay => Inner.AllDay;
            public bool Recurring => Inner.Recurring;

            public bool IsHidden => Inner.IsHidden;
        }
    }
}
