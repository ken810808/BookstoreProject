using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BookStore_Web_.Models
{
    [MetadataType(typeof(MetaCheckStatus))]
    public partial class ChechStatus
    {
        public class MetaCheckStatus
        {
            [DisplayName("審核狀態")]
            public string Status { get; set; }
        }
    }
}