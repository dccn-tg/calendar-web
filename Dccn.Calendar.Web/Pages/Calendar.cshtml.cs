using System.Threading.Tasks;
using Dccn.Calendar.Web.Services;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dccn.Calendar.Web.Pages
{
    [UsedImplicitly]
    public class CalendarModel : PageModel
    {
        private readonly ICalendarService _service;

        public CalendarModel(ICalendarService service)
        {
            _service = service;
        }

        public string Id { get; private set; }
        public string Name { get; private set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            var result = await _service.TryGetCalendarAsync(id);
            if (!result.Success)
            {
                return NotFound();
            }

            Id = id;
            Name = result.Calendar.Name;

            return Page();
        }
    }
}