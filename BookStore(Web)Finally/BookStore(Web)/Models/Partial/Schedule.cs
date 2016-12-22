using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BookStore_Web_.Models
{
    [MetadataType(typeof(MetaShedule))]
    public partial class Schedule
    {
        public class MetaShedule
        {
            public int ScheduleID { get; set; }
            public int EmployeeID { get; set; }
            public System.DateTime ScheduleDateTime { get; set; }
            public Nullable<int> LocationID { get; set; }
            public Nullable<System.DateTime> Application_DateTime { get; set; }
            [DisplayName("申請原因")]
            public string Application_Reason { get; set; }
            public int Check_ID { get; set; }
        }
    }
}