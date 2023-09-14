using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Azure.Identity;
using Microsoft.Graph;
using Microsoft.Graph.Models;

namespace Dccn.Calendar
{
    public class CalendarClient
    {
        private const int DefaultMaxEvents = 100;

        public CalendarClient(string tenantId, string clientId, string certificate)
        {
            var credential = new ClientCertificateCredential(tenantId, clientId, certificate);
            Client = new GraphServiceClient(credential);
        }

        public GraphServiceClient Client { get; }
        public int MaxEvents { get; set; } = DefaultMaxEvents;

        public async Task<IEnumerable<Calendar>> ListCalendarsAsync(string mailBox)
        {
            var response = await Client.Users[mailBox].Calendars.GetAsync(request =>
            {
                request.QueryParameters.Select = new[] {"id", "name"};
                request.QueryParameters.Orderby = new[] {"name"};
                request.QueryParameters.Top = 1000;
            });

            return response!.Value!.Select(calendar => new Calendar(calendar, mailBox));
        }

        public async Task<Calendar> GetCalendarByIdAsync(string mailBox, string id)
        {
            var response = await Client.Users[mailBox].Calendars[id].GetAsync(request =>
            {
                request.QueryParameters.Select = new[] {"id", "name"};
            });

            return response == null ? null : new Calendar(response, mailBox);
        }

        public async Task<IEnumerable<Event>> GetEventsAsync(IEnumerable<Calendar> calendars, DateTime start, DateTime end)
        {
            var content = new BatchRequestContent(Client);

            var requestMap = new Dictionary<string, Calendar>();
            foreach (var calendar in calendars)
            {
                var request = Client.Users[calendar.MailBox].Calendars[calendar.Id].CalendarView.ToGetRequestInformation(request =>
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
                    request.QueryParameters.Top = MaxEvents;
                });
                var requestId = await content.AddBatchRequestStepAsync(request);
                requestMap.Add(requestId, calendar);
            }

            var responses = await Client.Batch.PostAsync(content);

            var events = new List<Event>();
            foreach (var (requestId, calendar) in requestMap)
            {
                var response = await responses.GetResponseByIdAsync<EventCollectionResponse>(requestId);
                if (response.Value != null)
                {
                    events.AddRange(response.Value!.Select(@event => new Event(@event, calendar)));
                }
            }
            // events.Sort();


            // var response = await Client.Users[mailBox].CalendarView.GetAsync(request =>
            // {
            //     request.QueryParameters.StartDateTime = start.ToString("o", CultureInfo.InvariantCulture);
            //     request.QueryParameters.EndDateTime = end.ToString("o", CultureInfo.InvariantCulture);
            //     request.QueryParameters.Filter = $"/calendar/id in ({string.Join(',', calendars.Select(c => $"'{c}'"))})";
            //     request.QueryParameters.Select = new[]
            //     {
            //         "id",
            //         "subject",
            //         "start",
            //         "end",
            //         "isAllDay",
            //         "type"
            //     };
            //     request.QueryParameters.Orderby = new[] {"start/dateTime"};
            //     request.QueryParameters.Top = MaxEvents;
            // });

            return events;
        }
    }
}
