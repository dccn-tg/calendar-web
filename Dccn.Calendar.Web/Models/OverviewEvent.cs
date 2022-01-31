using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Dccn.Calendar.Web.Models
{
    public class OverviewEvent
    {
        public string Title { get; set; }

        [DataType(DataType.Time)]
        public DateTime Start { get; set; }

        [DataType(DataType.Time)]
        public DateTime End { get; set; }

        public string Location { get; set; }

        public bool Ended { get; set; }
    }
}
