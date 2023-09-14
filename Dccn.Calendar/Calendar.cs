using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Graph;
using Microsoft.Graph.Models;

namespace Dccn.Calendar
{
    public class Calendar
    {
        private readonly Microsoft.Graph.Models.Calendar _folder;

        internal Calendar(Microsoft.Graph.Models.Calendar folder, string mailBox)
        {
            _folder = folder;
            MailBox = mailBox;
        }

        public string Id => _folder.Id;
        public string Name => _folder.Name;
        public string MailBox { get; }

        public async Task<IEnumerable<Event>> EventsRangeAsync(CalendarClient client, string mailBox, DateTime start, DateTime end)
        {
            var response = await client.Client.Users[mailBox].Calendars[Id].CalendarView.GetAsync(request =>
            {
                request.QueryParameters.StartDateTime = start.ToString("o", CultureInfo.InvariantCulture);
                request.QueryParameters.EndDateTime = end.ToString("o", CultureInfo.InvariantCulture);
                request.QueryParameters.Select = new[]
                {
                    "id",
                    "subject",
                    "start",
                    "end",
                    "isAllDay",
                    "type"
                };
                request.QueryParameters.Orderby = new[] {"start/dateTime"};
                request.QueryParameters.Top = client.MaxEvents;
            });

            return response!.Value!.Select(@event => new Event(@event, this));
        }

        public Task<IEnumerable<Event>> EventsRangeAsync(CalendarClient client, string mailBox, DateTime start, TimeSpan duration)
        {
            return EventsRangeAsync(client, mailBox, start, start.Add(duration));
        }
    }
}
