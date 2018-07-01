using System;

namespace Dccn.Calendar.Web.Models
{
    public class EventDto
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public bool AllDay { get; set; }
        public bool Recurring { get; set; }
        public string Location { get; set; }
    }
}
