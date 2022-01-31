using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dccn.Calendar.Web.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dccn.Calendar.Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ICalendarService _service;

        public IndexModel(ICalendarService service)
        {
            _service = service;
        }

        public ICollection<Services.Calendar> Calendars;

        public async Task OnGetAsync()
        {
            Calendars = (await _service.GetCalendarsAsync()).ToList();
        }
    }
}