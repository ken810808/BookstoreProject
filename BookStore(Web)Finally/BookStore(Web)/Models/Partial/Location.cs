using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BookStore_Web_.Models
{
    [MetadataType(typeof(MetaLocation))]
    public partial class Location
    {
        public class MetaLocation
        {
            public int LocationID { get; set; }
            [DisplayName("經度")]
            public string Latitude { get; set; }
            [DisplayName("緯度")]
            public string Longitude { get; set; }
            [DisplayName("打卡地點")]
            public string Location_Name { get; set; }
            public Nullable<bool> Location_DElete { get; set; }
    
        }
    }
}