using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Exchange.WebServices.Data;

namespace Dccn.Calendar
{
    public class Calendar
    {
        private static readonly PropertySet AppointmentProperties =
            new PropertySet(BasePropertySet.IdOnly,
                ItemSchema.Subject,
                AppointmentSchema.Start,
                AppointmentSchema.End,
                AppointmentSchema.IsAllDayEvent,
                AppointmentSchema.IsRecurring,
                AppointmentSchema.Duration,
                AppointmentSchema.ICalUid);

        private readonly CalendarFolder _folder;

        internal Calendar(CalendarClient client, CalendarFolder folder)
        {
            _folder = folder;
            Client = client;
        }

        public CalendarClient Client { get; }

        public string Id => _folder.Id.ToString();
        public string Name => _folder.DisplayName;

        public async Task<IEnumerable<Event>> EventsRangeAsync(DateTime start, DateTime end)
        {
            var view = new CalendarView(start, end, Client.MaxEvents)
            {
                PropertySet = AppointmentProperties
            };
            var appointments = await _folder.FindAppointments(view);

            return appointments
                .OrderBy(appointment => appointment.Start)
                .Select(appointment => new Event(appointment));
        }

        public Task<IEnumerable<Event>> EventsRangeAsync(DateTime start, TimeSpan duration)
        {
            return EventsRangeAsync(start, start.Add(duration));
        }
    }
}