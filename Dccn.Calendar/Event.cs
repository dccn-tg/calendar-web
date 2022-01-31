using System;
using Microsoft.Exchange.WebServices.Data;

namespace Dccn.Calendar
{
    public class Event
    {
        private readonly Appointment _appointment;
        private readonly string _originalTitle;

        internal Event(Calendar calendar, Appointment appointment)
        {
            _appointment = appointment;
            Calendar = calendar;

            if (appointment.Subject == null)
            {
                _originalTitle = "???";
            }
            else if (appointment.Subject.StartsWith("##"))
            {
                _originalTitle = appointment.Subject.Remove(0, 2);
            }
        }

        public Calendar Calendar { get; }

        public string Id => _appointment.ICalUid;

        public string Title => _originalTitle ?? _appointment.Subject;

        public DateTime Start => _appointment.Start;
        public DateTime End => _appointment.End;

        public bool AllDay => _appointment.IsAllDayEvent;
        public bool Recurring => _appointment.IsRecurring;

        public long UnixTimestamp => new DateTimeOffset(_appointment.Start).ToUnixTimeMilliseconds();
        public long DurationMillis => (long) _appointment.Duration.TotalMilliseconds;
        public bool IsHidden => _originalTitle != null;
    }
}