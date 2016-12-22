using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BookStore_Web_.Models;

namespace BookStore_Web_.Controllers
{
    public class HomeController : Controller
    {
        Repository<Emp_Information> db = new Repository<Emp_Information>(new BookStoreObject_Entities());
        BookStoreObject_Entities detail = new BookStoreObject_Entities();
        // GET: Home
        public ActionResult Index(Emp_Information emp)
        {
            if (db.GetByID(emp.EmployeeID) != null)
            {
                var q = db.GetByID(emp.EmployeeID);
                if (q.Emp_Password == emp.Emp_Password)
                {
                    return View();
                }
                else
                {
                    ViewBag.message = "輸入錯誤，請從新輸入";
                    return RedirectToAction("login");
                }
            }
            else
            {
                ViewBag.message = "查無此員工";
                return RedirectToAction("login");
            }
        }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Loginvalid(Emp_Information Emp)
        {
            var ID=db.GetByID(Emp.EmployeeID);
            if (ID != null)
            {
                ViewBag.message = "登入成功";
                return RedirectToAction("Index");
            }
            else
                ViewBag.message = "帳號密碼有錯，請從新輸入";
            return RedirectToAction("Login");
        }

        [HttpGet]
        public ActionResult AddCount()
        {
            ViewBag.occu = new SelectList(detail.Dept_Information.ToList(), "Emp_DeptID", "Dept_Name");
            return View();
        }

    }
}