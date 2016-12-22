using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookStore_Web_.Models.VM
{
    public class Detail
    {
        public string Name { get; set; }
        public string location { get; set; }
        public DateTime Date { get; set; }
        public  int? kilometer { get; set; }
        public int money { get; set; }
    }
}