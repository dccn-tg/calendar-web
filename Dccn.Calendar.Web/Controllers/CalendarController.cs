using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dccn.Calendar.Web.Models;
using Dccn.Calendar.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace Dccn.Calendar.Web.Controllers
{
    [Route("api/calendar")]
    [ApiController]
    public class CalendarController : ControllerBase
    {
        private readonly ICalendarService _service;

        public CalendarController(ICalendarService service)
        {
            _service = service;
        }

        [HttpGet]
        [ActionName("events")]
        [ProducesResponseType(typeof(IEnumerable<EventDto>), 200)]
        public async Task<IActionResult> EventsAsync([FromQuery(Name = "id")] string calendarId, DateTime start, DateTime end)
        {
            var (success, calendar) = await _service.TryGetCalendarAsync(calendarId);
            if (!success)
            {
                return NotFound();
            }

            var events = (await calendar.EventsRangeAsync(start, end))
                .Select(@event => new EventDto
                {
                    Id = @event.Id,
                    Title = @event.Subject,
                    Start = @event.Start,
                    End = @event.End,
                    AllDay = @event.AllDay,
                    Recurring = @event.Recurring,
                    Location = calendar.Name
                });

            return Ok(events);
        }
    }
}
