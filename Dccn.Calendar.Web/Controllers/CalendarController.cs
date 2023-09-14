using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dccn.Calendar.Web.Models;
using Dccn.Calendar.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace Dccn.Calendar.Web.Controllers
{
    [ApiController]
    [Route("api/[action]")]
    public class CalendarController : ControllerBase
    {
        private readonly ICalendarService _service;

        public CalendarController(ICalendarService service)
        {
            _service = service;
        }

        [HttpGet]
        [ActionName("Events")]
        [ProducesResponseType(200), ProducesResponseType(404)]
        public async Task<ActionResult<IEnumerable<CalendarEvent>>> EventsAsync(
            [FromQuery(Name = "id")] string calendarId,
            DateTime start,
            DateTime end)
        {
            var calendar = await _service.TryGetCalendarAsync(calendarId);
            if (calendar == null)
            {
                return NotFound();
            }

            var events = (await _service.GetEventsRangeAsync(calendar, start, end))
                .Select(@event => new CalendarEvent
                {
                    Id = @event.Id,
                    Title = @event.Title,
                    Start = @event.Start,
                    End = @event.End == @event.Start ? @event.End.AddMinutes(10) : @event.End,
                    AllDay = @event.AllDay,
                    Recurring = @event.Recurring,
                    Location = calendar.Location
                });

            return Ok(events);
        }
    }
}
