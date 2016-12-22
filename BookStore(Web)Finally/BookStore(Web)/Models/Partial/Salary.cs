using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BookStore_Web_.Models
{
    [MetadataType(typeof(Metasalary))]
    public partial class Salary
    {
        public class Metasalary
        {
            public int EmployeeID { get; set; }
            [DisplayName("月份")]
            public int DateTime { get; set; }
            [DisplayName("薪資")]
            public Nullable<int> Emp_Salary { get; set; }
            [DisplayName("工作時數")]
            public Nullable<int> Emp_hours { get; set; }
        }
    }
}