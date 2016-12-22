using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class DefaultController : ApiController
    {
        Repository<Emp_Information> db = new Repository<Emp_Information>(new BookStoreObject_Entities());

        public IEnumerable<Emp_Information> GetShippers()
        {
            return db.GetAll();
        }

        public Emp_Information GetShipper(int id)
        {
            return db.GetByID(id);
        }

        public void PostShippers(Emp_Information shipper)
        {
            db.Create(shipper);
        }
        public void PutShippers(Emp_Information shipper)
        {
            db.Create(shipper);
        }

        public void DeleteShippers(int id)
        {
            db.Create(db.GetByID(id));
        }
    }
}
