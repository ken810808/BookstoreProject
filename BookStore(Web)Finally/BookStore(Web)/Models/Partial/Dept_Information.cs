using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BookStore_Web_.Models
{
    [MetadataType(typeof(MetaDept))]
    public partial class Dept_Information
    {
        public class MetaDept
        {
            public int Emp_DeptID { get; set; }
            [DisplayName("部門")]
            public string Dept_Name { get; set; }
        }
    }
}