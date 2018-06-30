using System;
using Microsoft.Exchange.WebServices.Data;

namespace Dccn.Calendar
{
    public class Event
    {
        private readonly Appointment _appointment;

        internal Event(Appointment appointment)
        {
            _appointment = appointment;
        }

        public string Id => _appointment.ICalUid;

        public string Subject => _appointment.Subject;

        public DateTime Start => _appointment.Start;
        public DateTime End => _appointment.End;

        public bool AllDay => _appointment.IsAllDayEvent;
        public bool Recurring => _appointment.IsRecurring;

        public long UnixTimestamp => new DateTimeOffset(_appointment.Start).ToUnixTimeMilliseconds();
        public long DurationMillis => (long) _appointment.Duration.TotalMilliseconds;
    }
}