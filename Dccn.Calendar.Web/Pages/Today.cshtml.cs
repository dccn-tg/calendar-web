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

            var events = (await _service.GetOverviewEventsAsync(Date))
                .Where(@event => !@event.IsHidden)
                .OrderBy(@event => @event.Start)
                .ThenBy(@event => @event.End)
                .Select(@event => new OverviewEvent
                {
                    Title = @event.Title,
                    Start = @event.Start,
                    End = @event.End,
                    Location = @event.Calendar.Location,
                    Ended = now > @event.End
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
