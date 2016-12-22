using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BookStore_Web_.Models
{
    [MetadataType(typeof(MetaEmpGender))]
    public partial class Emp_Gender
    {
        public class MetaEmpGender
        {
            public int Emp_GenderID { get; set; }
            [DisplayName("性別")]
            public string Emp_Gender1 { get; set; }
        }
    }
}