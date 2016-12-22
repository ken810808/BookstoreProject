using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookStore_Web_.Models.VM
{
    public class Occu_Dept
    {
        public IEnumerable<Dept_Information> Dept_Information { get; set; }
        public IEnumerable<Emp_Occupation> Emp_Occupation { get; set; }
        public IEnumerable<Emp_Information>  Emp_Information{ get; set; }
    }
}