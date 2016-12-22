using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BookStore_Web_.Models
{
    [MetadataType(typeof(MateAbsenceType))]
    public partial class Absence_Type
    {
        public class MateAbsenceType
        {
            [DisplayName("請假類別")]
            public string Absence_Type1 { get; set; }

        }
    }
}