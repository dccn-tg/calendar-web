using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Dccn.Calendar.Web.Models;
using Dccn.Calendar.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dccn.Calendar.Web.Pages
{
    public class TodayModel : PageModel
    {
        private const int MaxDisplayedItems = 15;
        private static readonly TimeSpan Duration = TimeSpan.FromDays(1);

        private readonly ICalendarService _service;

        public TodayModel(ICalendarService service)
        {
            _service = service;
        }

        [DisplayFormat(DataFormatString = "{0:dddd}, {0:M}")]
        public DateTime Date { get; private set; }
        public ICollection<OverviewEvent> Events { get; private set; }

        public async Task<IActionResult> OnGetAsync(DateTime? date)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Date = date ?? DateTime.Today;
            var now = DateTime.Now;

            var tasks = (await _service.GetCalendarsAsync(true))
                .Select(async calendar => new {Calendar = calendar, Events = await calendar.EventsRangeAsync(Date, Duration)});

            var events = (await Task.WhenAll(tasks))
                .SelectMany(collection => collection.Events.Select(@event => new {collection.Calendar, Event = @event}))
                .Where(eventInfo => !eventInfo.Event.IsHidden)
                .OrderBy(eventInfo => eventInfo.Event.Start)
                .ThenBy(eventInfo => eventInfo.Event.End)
                .Select(eventInfo => new OverviewEvent
                {
                    Title = eventInfo.Event.Title,
                    Start = eventInfo.Event.Start,
                    End = eventInfo.Event.End,
                    Location = eventInfo.Calendar.Location,
                    Ended = now > eventInfo.Event.End
                })
                .ToList();

            var pastEvents = events.TakeWhile(@event => @event.Ended);
            var futureEvents = events.SkipWhile(@event => @event.Ended).Take(MaxDisplayedItems);

            Events = pastEvents
                .Concat(futureEvents)
                .TakeLast(MaxDisplayedItems)
                .ToList();

            return Page();
        }
    }
}
