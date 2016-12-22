using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BookStore_Web_.Models
{
    [MetadataType(typeof(MetaOccu))]
    public partial class Emp_Occupation
    {
        public class MetaOccu
        {
            [DisplayName("部門")]
            public string Emp_OccupationName { get; set; }
        }
    }
}