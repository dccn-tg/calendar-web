using System;
using Microsoft.Graph.Models;

namespace Dccn.Calendar
{
    public class Event
    {
        private readonly Microsoft.Graph.Models.Event _event;
        private readonly string _originalTitle;

        internal Event(Microsoft.Graph.Models.Event @event, Calendar calendar)
        {
            _event = @event;
            Calendar = calendar;

            if (@event.Subject == null)
            {
                _originalTitle = "???";
            }
            else if (@event.Subject.StartsWith("##"))
            {
                _originalTitle = @event.Subject.Remove(0, 2);
            }
        }

        public Calendar Calendar { get; }

        public string Id => _event.Id;

        public string Title => _originalTitle ?? _event.Subject;

        public DateTime Start => _event.Start.ToDateTime().ToLocalTime();
        public DateTime End => _event.End.ToDateTime().ToLocalTime();

        public bool AllDay => _event.IsAllDay.GetValueOrDefault();
        public bool Recurring => _event.Type != EventType.SingleInstance;

        public long UnixTimestamp => new DateTimeOffset(Start).ToUnixTimeMilliseconds();
        public long DurationMillis => (long) End.Subtract(Start).TotalMilliseconds;
        public bool IsHidden => _originalTitle != null;
    }
}
