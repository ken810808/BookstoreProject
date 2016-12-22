using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookStore_Web_.Models.VM
{
    public class UserSchedule
    {
        public string name { get; set; }
        public string status { get; set; }
        public string locationName { get; set; }
        public string Application_Reason { get; set; }
        public int ScheduleID { get; set; }
        public DateTime Application_DateTime { get; set; }
        public DateTime ScheduleDateTime { get; set; }     
    }
}