using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TotalStaffingSolutions.Models
{
    public class TimeSheetTuple
    {
        public Timesheet TimeSheetGeneralDetails { get; set; }
        public List<Timesheet_summaries> TimeSheetSummary { get; set; }
        public List<Timesheet_details> TimeSheetDetails { get; set; }
    }
}