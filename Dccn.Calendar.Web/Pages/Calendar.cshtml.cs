using System.Threading.Tasks;
using Dccn.Calendar.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dccn.Calendar.Web.Pages
{
    public class CalendarModel : PageModel
    {
        private readonly ICalendarService _service;

        public CalendarModel(ICalendarService service)
        {
            _service = service;
        }

        public string CalendarId { get; private set; }
        public string CalendarName { get; private set; }

        public async Task<IActionResult> OnGetAsync(string calendarId)
        {
            var result = await _service.TryGetCalendarAsync(calendarId);
            if (!result.Success)
            {
                return NotFound();
            }

            CalendarId = calendarId;
            CalendarName = result.Calendar.Name;

            return Page();
        }
    }
}