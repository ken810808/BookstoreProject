using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookStore_Web_.Models.VM
{
    public class Sal_Chech
    {
        public IEnumerable<Salary> Salary { get; set; }
        public IEnumerable<CheckInDetail> CheckInDetail { get; set; }
    }
}