using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BookStore_Web_.Models;

namespace BookStore_Web_.Controllers
{
    public class EmployeesController : ApiController
    {
        Repository<Emp_Information>Emp=new Repository<Emp_Information>(new BookStoreObject_Entities());

        public IEnumerable<Emp_Information> GetAll()
        {
            return Emp.GetAll();
        }
    }
}
