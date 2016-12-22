

using BookStore_Web_.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;
using System.Net.Mail;
using BookStore_Web_.Models.VM;


namespace BookStore_Web_.Controllers
{
    public class AdminController : Controller
    {
        #region

        private BookStoreObject_Entities db = new BookStoreObject_Entities();
        private Repository<Absence_Table> absenceTableRepository = null;
        private Repository<Absence_Type> absenceTypeRepository = null;
        private Repository<Emp_Information> EmpRepository = null;
        private Repository<Location> LocRepository = null;
        private Repository<Salary> salaryRepository = null;
        private Repository<Dept_Information> DepRepository = null;
        private Repository<Schedule> ScheduleRepository = null;
        private Repository<ChechStatus> CheckStatusRepository = null;
        private Repository<Location> RepositoryLocation = null;
        private Repository<Models.Schedule> RepositorySchedule = null;
        private Repository<Models.ChechStatus> RepositoryStatus = null;
        Emp_Information EmpUser = HomeController.User;
        
        
        //新增====================================================
        //打卡
        private Repository<CheckInDetail> CheckDetailRepository = null;
        private Repository<Salary> salaryReposirotry = null;

        //請假
        IEnumerable<Absence_Table> absenceTable = null;
        int EmpOccu;


        enum month : int
        {
            January = 1, February, March, April, May, June, July, August, September, October, November, December
        }
       
        public AdminController()
        {
            salaryReposirotry = new Repository<Salary>(db);
            CheckDetailRepository = new Repository<CheckInDetail>(db);
            EmpRepository = new Repository<Emp_Information>(db);
            LocRepository=new Repository<Location>(db);
            absenceTableRepository = new Repository<Absence_Table>(db);
            absenceTypeRepository = new Repository<Absence_Type>(db);
            salaryRepository = new Repository<Salary>(db);
            DepRepository= new Repository<Dept_Information>(db);
            ScheduleRepository = new Repository<Schedule>(db);
            CheckStatusRepository = new Repository<ChechStatus>(db);
            RepositoryLocation = new Repository<Location>(db);
            RepositorySchedule = new Repository<Schedule>(db);
            RepositoryStatus = new Repository<ChechStatus>(db);

            //============================================
            absenceTable = absenceTableRepository.GetAll().Where(p => p.Dispose == false);
            EmpOccu = db.Emp_Information.Where(p => p.EmployeeID == EmpUser.EmployeeID).Select(p => p.Emp_OccupationID).FirstOrDefault();
        }
        


        public ActionResult Index()
        {
            EmpUser = EmpRepository.GetByID(EmpUser.EmployeeID);
            Session["name"] = EmpUser.Emp_Name.ToString();
            return View();
        }

        #endregion
        //====================================================================
        #region 權限

        public ActionResult Authority()
        {
            Session["name"] = EmpUser.Emp_Name.ToString();
            var Emps = db.Emp_Information.AsQueryable();
            //設定預設排序的欄位 ModelNumber
           

           // ViewBag.Dept = new SelectList(db.Dept_Information.ToList(), "Emp_DeptID", "Dept_Name");
            return View(Emps.ToList());
        }

        public ActionResult LoadDept(int id)
        {           
            return PartialView(db.Emp_Information.Where(p=>p.Emp_DeptID==id));            
        }

        public ActionResult LoadOccu(int id = 1)
        {
            return PartialView(db.Emp_Information.Where(p => p.Emp_OccupationID == id));
        }

        public ActionResult Dept()
        {
            var dept = db.Dept_Information.Select(d => new { 
                DeptID=d.Emp_DeptID,
                DeptName=d.Dept_Name
            });
            return Json(dept, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Occu()
        {
            var Occu = db.Emp_Occupation.Select(d => new
            {
                OccuID = d.Emp_OccupationID,
                OccuName = d.Emp_OccupationName
            });
            return Json(Occu, JsonRequestBehavior.AllowGet);
        }
  
        [HttpGet]
        public ActionResult Update(int id=4)
        {
            ViewBag.Dept = new SelectList(db.Dept_Information.ToList(), "Emp_DeptID", "Dept_Name");
            ViewBag.Occu = new SelectList(db.Emp_Occupation.ToList(), "Emp_OccupationID", "Emp_OccupationName");
            return PartialView(db.Emp_Information.Find(id));
        }

        [HttpPost]
        public ActionResult Update(int id,Emp_Information emp)
        {
            Session["name"] = EmpUser.Emp_Name.ToString();
            Emp_Information Emp = EmpRepository.GetByID(id);
           Emp.Emp_OccupationID = emp.Emp_OccupationID;
           Emp.Emp_DeptID = emp.Emp_DeptID;
           EmpRepository.Update(Emp);
            return RedirectToAction("Authority");
        }

        
        //階層架構
        public ActionResult company()
        {
            Session["name"] = EmpUser.Emp_Name.ToString();
            var emp = db.Emp_Information.ToList();
            return View(emp);
        }

        #endregion
        //====================================================================
        #region 會員
        public ActionResult Browse(int? page)
        {
            Session["name"] = EmpUser.Emp_Name.ToString();
            var mag = EmpRepository.GetByID(EmpUser.EmployeeID);
            if (mag.Emp_DeptID == null)
            {
                var EmpInfo = db.Emp_Information.AsQueryable();
                return View(EmpInfo.ToList().ToPagedList(page ?? 1, 7));
            }
            else
            { 
               var EmpInfo = db.Emp_Information.Where(p => p.Emp_DeptID == mag.Emp_DeptID);
               return View(EmpInfo.ToList().ToPagedList(page ?? 1, 7));
            }
            
        }

        [HttpGet]
        public ActionResult EmpCreate()
        {
            Session["name"] = EmpUser.Emp_Name.ToString();
            ViewBag.Emp_Imformation = new SelectList(db.Emp_Gender.ToList(), "Emp_GenderID", "Emp_Gender1");
            ViewBag.Emp_Dept = new SelectList(db.Dept_Information.ToList(), "Emp_DeptID", "Dept_Name");
            ViewBag.Emp_Report = new SelectList(db.Emp_Information.ToList(), "Emp_Report", "Emp_Name");
            ViewBag.Emp_Occu = new SelectList(db.Emp_Occupation.ToList(), "Emp_OccupationID", "Emp_OccupationName");
            return View();
        }
        [HttpPost]
        public ActionResult EmpCreate(Emp_Information EmpInfo)
        {
            Session["name"] = EmpUser.Emp_Name.ToString();
            if (ModelState.IsValid)
            {
                HttpPostedFileBase file = Request.Files["image"];
                if (file.ContentLength > 0)
                {
                    //將上傳的檔案轉成二進位
                    var imgSize = file.ContentLength;
                    byte[] imgByte = new Byte[imgSize];
                    file.InputStream.Read(imgByte, 0, imgSize);
                    EmpInfo.Emp_Image = imgByte;
                }

                db.Emp_Information.Add(EmpInfo);
                db.SaveChanges();
            }
            ViewBag.Emp_Imformation = new SelectList(db.Emp_Gender.ToList(), "Emp_GenderID", "Emp_Gender1");
            ViewBag.Emp_Dept = new SelectList(db.Dept_Information.ToList(), "Emp_DeptID", "Dept_Name");
            ViewBag.Emp_Report = new SelectList(db.Emp_Information.ToList(), "Emp_Report", "Emp_Name");
            ViewBag.Emp_Occu = new SelectList(db.Emp_Occupation.ToList(), "Emp_OccupationID", "Emp_OccupationName");
            //return View();
            return RedirectToAction("Browse");
        }

        [HttpGet]
        public ActionResult EmpEdit(int id = 4)
        {
            Session["name"] = EmpUser.Emp_Name.ToString();
            Emp_Information emp = EmpRepository.GetByID(id);
            var q = from p in db.Emp_Information
                    where p.Emp_DeptID == null || p.Emp_DeptID == emp.Emp_DeptID && p.Emp_OccupationID < emp.Emp_OccupationID
                    select p;
            if (q != null)
            {
                ViewBag.Emp_Imformation = new SelectList(db.Emp_Gender.ToList(), "Emp_GenderID", "Emp_Gender1");
                ViewBag.Emp_Report = new SelectList(q.ToList(), "EmployeeID", "Emp_Name");
                ViewBag.Emp_Dept = new SelectList(db.Dept_Information.ToList(), "Emp_DeptID", "Dept_Name");
                ViewBag.Emp_Occu = new SelectList(db.Emp_Occupation.ToList(), "Emp_OccupationID", "Emp_OccupationName");
            }
            return View(db.Emp_Information.Find(id));

        }

        [HttpPost]
        public ActionResult EmpEdit(int id, Emp_Information emp)
        {
            Emp_Information abc = EmpRepository.GetByID(id);
            abc.Emp_Name = emp.Emp_Name;
            abc.Emp_GenderID = emp.Emp_GenderID;
            abc.Emp_DeptID = emp.Emp_DeptID;
            abc.Emp_Report = emp.Emp_Information2.EmployeeID;
            abc.Emp_OccupationID = emp.Emp_OccupationID;
            abc.Emp_Tel = emp.Emp_Tel;
            abc.Emp_CellPhone = emp.Emp_CellPhone;
            abc.Emp_E_Mail = emp.Emp_E_Mail;
            abc.Emp_Identity = emp.Emp_Identity;
            abc.Emp_Log = emp.Emp_Log;
            abc.Emp_Password = emp.Emp_Password;
            HttpPostedFileBase file = Request.Files["image"];
            if (file.ContentLength > 0)
            {
                //將上傳的檔案轉成二進位
                var imgSize = file.ContentLength;
                byte[] imgByte = new Byte[imgSize];
                file.InputStream.Read(imgByte, 0, imgSize);
                abc.Emp_Image = imgByte;
            }
            abc.Emp_Address = emp.Emp_Address;
            //Repository
            EmpRepository.Update(abc);
            return RedirectToAction("Browse");
        }

        public ActionResult GetImageByte(int id = 0)
        {
            Emp_Information emp = db.Emp_Information.Find(id);
            byte[] img = emp.Emp_Image;
            if (img == null)
            {
                return File(@"~\Content\大頭貼\預設.gif", "image/jpeg");
            }
            else
            { 
            return File(img, "image/jpeg");            
            }

        }

        public ActionResult EmpDept()
        {
            var depts = db.Dept_Information.Select(c => new
            {
                DeptID = c.Emp_DeptID,
                DeptName = c.Dept_Name
            });

            return Json(depts, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PartialBrowse(int? page, string sortBy, int? id, string abc)
        {
            var EmpInfo = db.Emp_Information.AsQueryable();
            if (id == null)
            {
                if (Request.UrlReferrer.Query != "" && Request.UrlReferrer.Query.Split('=')[1] != "0")
                {
                    page = Convert.ToInt32(Request.UrlReferrer.Query.Split('=')[1]);
                    return PartialView(EmpInfo.ToList().ToPagedList(page ?? 1, 7));
                }
                else
                    return PartialView(EmpInfo.ToList().ToPagedList(page ?? 1, 7));
            }
            else
            {
                return PartialView(EmpInfo.Where(p => p.Emp_DeptID == id).ToList().ToPagedList(page ?? 1, 7));
            }
        }

        public ActionResult PartialSortBy(string sortBy, int? id, string abc)
        {
            var EmpInfo = db.Emp_Information.AsQueryable();
            ViewBag.SortEmployeeID = string.IsNullOrEmpty(sortBy) ? "EmployeeID desc" : "";
            ViewBag.SortEmp_Name = string.IsNullOrEmpty(sortBy) ? "Emp_Name desc" : "";
            ViewBag.SortDeptID = string.IsNullOrEmpty(sortBy) ? "Emp_DepID desc" : "";

            switch (abc)
            {
                case "依員工編號排序":
                    EmpInfo = EmpInfo.OrderBy(p => p.EmployeeID);
                    break;
                case "依員工姓名排序":
                    EmpInfo = EmpInfo.OrderBy(p => p.Emp_Name);
                    break;
                case "依所屬部門排序":
                    EmpInfo = EmpInfo.OrderBy(p => p.Emp_DeptID);
                    break;
                case "依所屬主管排序":
                    EmpInfo = EmpInfo.OrderBy(p => p.Emp_Report);
                    break;
                case "===請選擇===":
                    EmpInfo = EmpInfo.OrderBy(p => p.EmployeeID);
                    break;
            }

            return PartialView(EmpInfo);
        }

        public ActionResult empbrowse()
        {
            var user = HomeController.User;
            ViewBag.Show = user.Emp_Name;
            return View(user);
        }

        [HttpPost]
        public ActionResult ShowInfo(Emp_Information info)
        {
            Emp_Information c = EmpRepository.GetByID(info.EmployeeID);
            ViewBag.Show = c.Emp_Name;

            return View(c);
        }

        //====================================================================
        //連動
        public ActionResult Dispose(int id = 0)
        {
            Emp_Information emp = EmpRepository.GetByID(id);
            emp.Emp_Dispose = true;
            EmpRepository.Update(emp);
            return RedirectToAction("Browse");
        }

        public ActionResult ConnDept(int DepID)
        {
            if (DepID != 0)
            {
                var q = from p in db.Emp_Information
                        where p.Emp_Report <= 3 && p.Emp_DeptID == DepID && p.Emp_Dispose == false || p.EmployeeID == 3
                        select p;
                var emp = q.Select(c => new
                {
                    ConnID = c.EmployeeID,
                    ConnName = c.Emp_Name
                });
                return Json(emp, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return RedirectToAction("EmpCreate");
            }
        }

        public ActionResult ConnDeptID(int id = 0)
        {
            var q = (from p in db.Emp_Information
                     where p.Emp_OccupationID == id
                     select p).FirstOrDefault();

            return RedirectToAction("EmpCreate");
        }

        #endregion
        //====================================================================
        #region 打卡
        public ActionResult PunchBrowse()
        {
            Session["name"] = EmpUser.Emp_Name.ToString();
            return View(LocRepository.GetAll());
        }

        public ActionResult LoadLocation()
        {
            return PartialView(LocRepository.GetAll());
        }
        
        public ActionResult PunchCreate()
        {
            return View();
        }

        [HttpPost]
        public ActionResult PunchCreate(Location local)
        {
            Location new_Locat = new Location();
            new_Locat.Location_Name = local.Location_Name;
            new_Locat.Longitude = local.Longitude;
            new_Locat.Latitude = local.Latitude;
            LocRepository.Create(new_Locat);
            return RedirectToAction("PunchBrowse");
        }    

        public ActionResult PunchGetAllLocation()
        {
            var locat = LocRepository.GetAll().Select(c => new
            {
                lat = double.Parse(c.Latitude),
                lng = double.Parse(c.Longitude)
            });
            return Json(locat, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(int id)
        {
            Location location = LocRepository.GetByID(id);
            LocRepository.Delete(location);
            return RedirectToAction("PunchBrowse");
        }

        public ActionResult PunchUpdate1(string sub, string sub1, string sub2, string sub3)
        {
            int id = int.Parse(sub);
            Location location = LocRepository.GetByID(id);

            location.Location_Name = sub1;
            location.Latitude = sub2;
            location.Longitude = sub3;
            LocRepository.Update(location);
            return RedirectToAction("PunchBrowse");
        }

        //新增
        public ActionResult Punchdetail(int? page)
        {
            Session["name"] = EmpUser.Emp_Name.ToString();
            var detail = db.CheckInDetail.Where(p => p.Emp_Information.Emp_Report == EmpUser.EmployeeID);
           return View(detail.ToList().ToPagedList(page ??1,8));
        }

        public ActionResult PunchView()
        {
            Session["name"] = EmpUser.Emp_Name.ToString();
            return View();
        }

        [HttpPost]
        public string punch_count(List<double> my_Location)
        {
            string myplace = null;
            int id = EmpUser.EmployeeID;
            if (my_Location[0] != null)
            {
                var q = LocRepository.GetAll().ToList();

                //if we have already save the record detail, we can break out from the foreach loop
                //如果已存有checkingdetail資料,我們可以跳出foreach 迴圈
                bool in_flag = false;

                // The valid location is less than 2 km  
                foreach (var loca in q)
                {
                    //var check = CheckDetailRepository.GetAll().ToList();
                    var j = double.Parse(loca.Latitude);
                    var i = double.Parse(loca.Longitude);
                    double valid_dist = dist_count(my_Location[1], my_Location[0], i, j);

                    if (valid_dist < 2)
                    {
                        CheckInDetail checkdetail = new CheckInDetail();

                        // Insert the valid location when u punch. 存入有效的地點
                        checkdetail.CheckInLocation = loca.Location_Name;
                        myplace = loca.Location_Name;
                        // Insert employee's ID   存入員工ID     
                        checkdetail.EmployeeID = id;

                        // Late or not  有無遲到
                        DateTime today_date = DateTime.Now;

                        //Insert the time when u punch 插入打卡時的時間
                        checkdetail.CheckInDateTime = today_date;
                        DateTime t1 = Convert.ToDateTime("9:00:00 AM");
                        int attend = DateTime.Compare(today_date, t1);
                        if (attend <= 0)
                        {
                            checkdetail.Emp_AttendacyID = 1;  // On time 準時
                        }
                        else
                        {
                            checkdetail.Emp_AttendacyID = 2; // Be late 遲到
                        }

                        // punch my Latitude and Longitude  存入員工目前經緯度
                        checkdetail.CheckLongitude = my_Location[0].ToString();
                        checkdetail.CheclInLatitude = my_Location[1].ToString();

                        //Do we have first punch today  今天打過卡了嗎?
                        int recordid = 0;
                        CheckInDetail previous_record;

                        //select the EmployeeID 抓出這個員工的ID
                        var checking = (from n in db.CheckInDetail
                                        where n.EmployeeID == id
                                        select n);//.OrderByDescending(n=>n.CheckInRecordID).First();

                        double distio = 0;
                        if (checking != null)
                        {
                            foreach (var item in checking)
                            {
                                //check the detail that have today's record or not 確認這個月是否打過卡
                                if (item.CheckInDateTime.Value.Date == today_date.Date)
                                {
                                    //找出上一筆資料,並且抓出經緯度,然後做計算,並跳出
                                    recordid = checking.Max(n => n.CheckInRecordID);
                                    previous_record = CheckDetailRepository.GetByID(recordid);

                                    //my location between previous location 與上一個點的距離
                                    double prev_lng = double.Parse(previous_record.CheckLongitude);
                                    double prev_lat = double.Parse(previous_record.CheclInLatitude);
                                    distio = dist_count(my_Location[1], my_Location[0], prev_lng, prev_lat);

                                    checkdetail.Dis_Between_Previous = (Convert.ToInt32(distio)).ToString();

                                    in_flag = true;
                                    break;
                                }
                            }
                            CheckDetailRepository.Create(checkdetail);   //問題在這裡
                        }
                    }
                    if (in_flag)
                    {
                        break;
                    }
                }
            }
            return myplace;
        }

        public double dist_count(double now_lat, double now_lng, double other_lat, double other_lng)
        {
            double vale2 = Math.Pow(4, 2);
            double vale = Math.Sqrt(Math.Pow(now_lat - other_lat, 2) + Math.Pow(now_lng - other_lng, 2)) * 111.1;
            return vale;
        }


        #endregion
        //====================================================================
        #region 請假
       
        //查詢假單
        public ActionResult AbsentBrowse(int? page) //查詢自己的請假審核狀態
        {
            Session["name"] = EmpUser.Emp_Name;
            ViewBag.SortAbsenceTableNo = "AbsenceTableNo";
            ViewBag.SortAbsenceTableNoDesc = "AbsenceTableNodesc";
            ViewBag.SortAbsence_Type = "Absence_Type";
            ViewBag.SortAbsence_TypeDesc = "Absence_Typedesc";
            ViewBag.SortStartDate = "StartDate";
            ViewBag.SortStartDateDesc = "StartDatedesc";
            ViewBag.SortStatus = "Status";
            ViewBag.SortStatusDesc = "Statusdesc";

            absenceTable = EmployeeBrowse(EmpOccu);
            return View(absenceTable.ToPagedList(page ?? 1, 5));
        }

        public ActionResult AbsentPager(int? page, int Dept = 0, int Occu = 0)
        {
            //判斷權限
            absenceTable = EmployeeBrowse(EmpOccu, Dept, Occu);

            page = SetPage(page, absenceTable.Count());
            return PartialView(absenceTable.ToPagedList(page ?? 1, 5));
        }

        public ActionResult LoadAbsent(int? page, string sortBy, int Dept = 0, int Occu = 0)
        {
            switch (sortBy)
            {
                case "AbsenceTableNo":
                    absenceTable = absenceTable.OrderBy(p => p.Absence_No);
                    break;
                case "AbsenceTableNodesc":
                    absenceTable = absenceTable.OrderByDescending(p => p.Absence_No);
                    break;
                case "Absence_Type":
                    absenceTable = absenceTable.OrderBy(p => p.Absence_Type.Absence_Type1);
                    break;
                case "Absence_Typedesc":
                    absenceTable = absenceTable.OrderByDescending(p => p.Absence_Type.Absence_Type1);
                    break;
                case "StartDate":
                    absenceTable = absenceTable.OrderBy(p => p.StartDate.Date);
                    break;
                case "StartDatedesc":
                    absenceTable = absenceTable.OrderByDescending(p => p.StartDate.Date);
                    break;
                case "Status":
                    absenceTable = absenceTable.OrderBy(p => p.ChechStatus.Status);
                    break;
                case "Statusdesc":
                    absenceTable = absenceTable.OrderByDescending(p => p.ChechStatus.Status);
                    break;
                default:
                    absenceTable = absenceTable.OrderBy(p => p.Absence_No);
                    break;
            }

            //判斷權限
            absenceTable = EmployeeBrowse(EmpOccu, Dept, Occu);

            page = SetPage(page, absenceTable.Count());
            return PartialView(absenceTable.ToPagedList(page ?? 1, 5));
        }

        public ActionResult LoadAbsentChart()
        {
            //該員工 審核通過的請假單
            var chart = absenceTableRepository.GetAll().Where(n => n.EmployeeID == EmpUser.EmployeeID);
            return PartialView(chart);
        }

        public ActionResult LoadAbsentStatusChart()
        {
            //該員工 所有審核狀態的請假單
            var chart = absenceTableRepository.GetAll().Where(n => n.EmployeeID == EmpUser.EmployeeID);
            return PartialView(chart);
        }

        //假單審核
        public ActionResult AbsentCheck(int? page) //高權限 + 總經理   審核請假單
        {
            Session["name"] = EmpUser.Emp_Name;
            ViewBag.SortAbsenceTableNo = "AbsenceTableNo";
            ViewBag.SortAbsenceTableNoDesc = "AbsenceTableNodesc";
            ViewBag.SortAbsence_Type = "Absence_Type";
            ViewBag.SortAbsence_TypeDesc = "Absence_Typedesc";
            ViewBag.SortStartDate = "StartDate";
            ViewBag.SortStartDateDesc = "StartDatedesc";
            ViewBag.SortStatus = "Status";
            ViewBag.SortStatusDesc = "Statusdesc";

            absenceTable = EmployeeCheck();
            return View(absenceTable.ToPagedList(page ?? 1, 5));
        }

        public ActionResult AbsentCheckPager(int? page, int Dept = 0, int Occu = 0)
        {
            //判斷權限
            absenceTable = EmployeeCheck(Dept, Occu);

            page = SetPage(page, absenceTable.Count());
            return PartialView(absenceTable.ToPagedList(page ?? 1, 5));
        }

        public ActionResult LoadAbsentCheck(int? page, string sortBy, int Dept = 0, int Occu = 0)
        {

            switch (sortBy)
            {
                case "AbsenceTableNo":
                    absenceTable = absenceTable.OrderBy(p => p.Absence_No);
                    break;
                case "AbsenceTableNodesc":
                    absenceTable = absenceTable.OrderByDescending(p => p.Absence_No);
                    break;
                case "Absence_Type":
                    absenceTable = absenceTable.OrderBy(p => p.Absence_Type.Absence_Type1);
                    break;
                case "Absence_Typedesc":
                    absenceTable = absenceTable.OrderByDescending(p => p.Absence_Type.Absence_Type1);
                    break;
                case "StartDate":
                    absenceTable = absenceTable.OrderBy(p => p.StartDate.Date);
                    break;
                case "StartDatedesc":
                    absenceTable = absenceTable.OrderByDescending(p => p.StartDate.Date);
                    break;
                case "Status":
                    absenceTable = absenceTable.OrderBy(p => p.ChechStatus.Status);
                    break;
                case "Statusdesc":
                    absenceTable = absenceTable.OrderByDescending(p => p.ChechStatus.Status);
                    break;
                default:
                    absenceTable = absenceTable.OrderBy(p => p.Absence_No);
                    break;
            }


            List<int> holidays = new List<int>();
            holidays.Add(new DateTime(2016, 1, 1).DayOfYear);
            holidays.Add(new DateTime(2016, 2, 8).DayOfYear);
            holidays.Add(new DateTime(2016, 2, 9).DayOfYear);
            holidays.Add(new DateTime(2016, 2, 10).DayOfYear);
            holidays.Add(new DateTime(2016, 2, 11).DayOfYear);
            holidays.Add(new DateTime(2016, 2, 12).DayOfYear);
            holidays.Add(new DateTime(2016, 2, 29).DayOfYear);
            holidays.Add(new DateTime(2016, 4, 4).DayOfYear);
            holidays.Add(new DateTime(2016, 4, 5).DayOfYear);
            holidays.Add(new DateTime(2016, 6, 9).DayOfYear);
            holidays.Add(new DateTime(2016, 6, 10).DayOfYear);
            holidays.Add(new DateTime(2016, 9, 15).DayOfYear);
            holidays.Add(new DateTime(2016, 9, 16).DayOfYear);
            holidays.Add(new DateTime(2016, 10, 10).DayOfYear);
            for (int i = 1, max = new DateTime(2016, 12, 31).DayOfYear; i <= max; i++)
            {
                if (i % 7 == 2 || i % 7 == 3)
                {
                    holidays.Add(i);
                }
            }
            ViewBag.Holiday = holidays;
            absenceTable = EmployeeCheck(Dept, Occu);


            page = SetPage(page, absenceTable.Count());
            return PartialView(absenceTable.ToPagedList(page ?? 1, 5));
        }

        public ActionResult LoadAbsentCheckChart(string name = "")
        {           
            //傳回該員工的所有請假狀況
            var chart = absenceTableRepository.GetAll().Where(n => n.Emp_Information.Emp_Name == name);
            return PartialView(chart);
        }

        //CRUD
        public ActionResult AbsentCreate(Absence_Table _absenceTable)
        {
            ViewBag.AbsenceType = new SelectList(absenceTypeRepository.GetAll().ToList(), "Absence_ID", "Absence_Type1");
            if (ModelState.IsValid)
            {
                _absenceTable.EmployeeID = EmpUser.EmployeeID;
                _absenceTable.Check_ID = 1;
                _absenceTable.Dispose = false;
                _absenceTable.StartDate = Convert.ToDateTime(_absenceTable.StartDate);
                _absenceTable.EndDate = Convert.ToDateTime(_absenceTable.EndDate);
                HttpPostedFileBase file = Request.Files["CertificateDoc"];
                if (file != null && file.ContentLength > 0)
                {
                    var imgSize = file.ContentLength;
                    byte[] imgByte = new Byte[imgSize];
                    file.InputStream.Read(imgByte, 0, imgSize);
                    _absenceTable.Certificate_Doc = imgByte;
                }

                absenceTableRepository.Create(_absenceTable);
                return RedirectToAction("AbsentBrowse");
            }
            return View();
        }

        public ActionResult AbsentDelete(int id)
        {
            var delete = absenceTableRepository.GetByID(id);
            if (delete.Check_ID != 2)
                absenceTableRepository.Delete(delete);

            return RedirectToAction("AbsentBrowse");
        }

        [HttpGet]
        public ActionResult AbsentEdit(int id)
        {
            ViewBag.AbsenceType = new SelectList(absenceTypeRepository.GetAll().ToList(), "Absence_ID", "Absence_Type1");
            var editAbsence = absenceTableRepository.GetByID(id);

            return View(editAbsence);
        }

        [HttpPost]
        public ActionResult AbsentEdit(Absence_Table _absenceTable)
        {
            Absence_Table ab = absenceTableRepository.GetByID(_absenceTable.Absence_No);
            ab.Absence_ID = _absenceTable.Absence_ID;
            ab.StartDate = _absenceTable.StartDate;
            ab.EndDate = _absenceTable.EndDate;
            ab.Reason = _absenceTable.Reason;
            ab.Check_ID = 1;
            HttpPostedFileBase file = Request.Files["CertificateDoc"];
            if (file.ContentLength > 0)
            {
                var imgSize = file.ContentLength;
                byte[] imgByte = new Byte[imgSize];
                file.InputStream.Read(imgByte, 0, imgSize);
                ab.Certificate_Doc = imgByte;
            }
            absenceTableRepository.Update(ab);
            return RedirectToAction("AbsentBrowse");
        }

        public ActionResult AbsentAccept(int id)
        {
            var accept = absenceTableRepository.GetByID(id);
            accept.Check_ID = 2;
            absenceTableRepository.Update(accept);

            return RedirectToAction("AbsentCheck");
        }

        [HttpPost]
        public ActionResult AbsentAcceptMany(List<int> Data)
        {
            if (Data != null)
            {
                foreach (int item in Data)
                {
                    if(item!=0)
                    {                    
                    var accept = absenceTableRepository.GetByID(item);
                    accept.Check_ID = 2;
                    absenceTableRepository.Update(accept);
                    }
                }
            }
            return RedirectToAction("AbsentCheck");
        }

        public ActionResult AbsentReject(int id)
        {
            var accept = absenceTableRepository.GetByID(id);
            accept.Check_ID = 3;
            absenceTableRepository.Update(accept);

            return RedirectToAction("AbsentCheck");
        }

        [HttpPost]
        public ActionResult AbsentRejectMany(List<int> Data)
        {
            if (Data != null)
            {
                foreach (int item in Data)
                {
                    if(item!=0)
                    {
                    var accept = absenceTableRepository.GetByID(item);
                    accept.Check_ID = 3;
                    absenceTableRepository.Update(accept);
                    
                    }
                }
            }
            return RedirectToAction("AbsentCheck");
        }

        public ActionResult GetImage(int id)
        {
            Absence_Table certificate_Doc = absenceTableRepository.GetByID(id);
            byte[] img = certificate_Doc.Certificate_Doc;
            if (img == null)
                return File(@"~\Content\img\oppos.jpg", "image/jpeg");
            else
                return new FileContentResult(img, "image/jpeg");

        }

        //邏輯判斷
        int? SetPage(int? page = 1, int data = 0)
        {
            if (data == 0)
                return page = 1;
            else if (data / 5 < page && data % 5 == 0)
                return page = data / 5;
            else if (data / 5 < page && data % 5 > 0)
                return page = data / 5 + 1;
            else
                return page;
        }

        IEnumerable<Absence_Table> EmployeeBrowse(int empOccu, int Dept = 0, int Occu = 0)//查詢    只會看到自己的請假單
        {
            absenceTable = absenceTable.Where(p => p.EmployeeID == EmpUser.EmployeeID);

            if (Dept != 0 && Occu != 0)
                absenceTable = absenceTable.Where(p => p.Emp_Information.Emp_DeptID == Dept && p.Emp_Information.Emp_OccupationID == Occu).ToList();
            else if (Dept != 0 && Occu == 0)
                absenceTable = absenceTable.Where(p => p.Emp_Information.Emp_DeptID == Dept).ToList();
            else if (Occu != 0 && Dept == 0)
                absenceTable = absenceTable.Where(p => p.Emp_Information.Emp_OccupationID == Occu).ToList();
            else
                absenceTable = absenceTable.ToList();


            return absenceTable;
        }

        IEnumerable<Absence_Table> EmployeeCheck(int Dept = 0, int Occu = 0)//審核     只會看到 "report"是自己EmpID的請假單
        {
            absenceTable = absenceTable.Where(p => p.Check_ID == 1 && p.Emp_Information.Emp_Report == EmpUser.EmployeeID);

            if (Dept != 0 && Occu != 0)
                absenceTable = absenceTable.Where(p => p.Emp_Information.Emp_DeptID == Dept && p.Emp_Information.Emp_OccupationID == Occu).ToList();
            else if (Dept != 0 && Occu == 0)
                absenceTable = absenceTable.Where(p => p.Emp_Information.Emp_DeptID == Dept).ToList();
            else if (Occu != 0 && Dept == 0)
                absenceTable = absenceTable.Where(p => p.Emp_Information.Emp_OccupationID == Occu).ToList();
            else
                absenceTable = absenceTable.ToList();


            return absenceTable;
        }


        #endregion
        //====================================================================
        #region 排程


        public ActionResult PartialSchedule(int? page, string sortBy, int? id)
        {
            //切換排序狀態
            ViewBag.sortModelNumber = string.IsNullOrEmpty(sortBy) ? "ModelNumber desc" : "";
            ViewBag.sortModelName = sortBy == "ModelName" ? "ModelName desc" : "ModelName";
            ViewBag.sortUnitCost = sortBy == "UnitCost" ? "UnitCost desc" : "UnitCost";



            var products = ScheduleRepository.GetAll().Where(p => p.Emp_Information.Dept_Information.Emp_DeptID == id).AsQueryable();
            //設定預設排序的欄位 ModelNumber
            switch (sortBy)
            {
                case "ModelNumber desc":
                    products = products.OrderByDescending(p => p.Emp_Information.Emp_Name);
                    break;
                case "ModelName desc":
                    products = products.OrderByDescending(p => p.Location.Location_Name);
                    break;
                case "ModelName":
                    products = products.OrderBy(p => p.Location.Location_Name);
                    break;
                case "UnitCost desc":
                    products = products.OrderByDescending(p => p.Emp_Information.Dept_Information.Dept_Name);
                    break;
                case "UnitCost":
                    products = products.OrderBy(p => p.Emp_Information.Dept_Information.Dept_Name);
                    break;
                default:
                    products = products.OrderBy(p => p.Emp_Information.Emp_Name);
                    break;
            }
            if (id == null)
            {
                if (Request.UrlReferrer.Query != "" && Request.UrlReferrer.Query.Split('=')[1] != "0")
                {
                    page = Convert.ToInt32(Request.UrlReferrer.Query.Split('=')[1]);
                    return PartialView(db.Schedule.ToList().ToPagedList(page ?? 1, 10));
                }
                else
                {
                    return PartialView(db.Schedule.ToList().ToPagedList(page ?? 1, 10));
                }
            }

            else
            {
                return View(db.Schedule.Where(p => p.Emp_Information.Emp_DeptID == id).ToList().ToPagedList(page ?? 1, 3));
            }
        }
        public ActionResult JsonSchedule()
        {
            var categories = DepRepository.GetAll().Select(c => new
            {
                CategoryID = c.Emp_DeptID,
                CategoryName = c.Dept_Name
            });

            return Json(categories, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ScheduleEdit(int id = 20)
        {
            var category = ScheduleRepository.GetAll().Where(p => p.EmployeeID == id);
            ViewBag.Location = LocRepository.GetAll();
            ViewBag.state = CheckStatusRepository.GetAll();
            return PartialView(category.FirstOrDefault());
        }

        // GET: Schedule/EmpSchedule
        public ActionResult Scheduletable()
        {
            Session["name"] = EmpUser.Emp_Name.ToString();
            ViewBag.empid = EmpUser.EmployeeID;
            return View(RepositorySchedule.GetAll());
        }
        
        public ActionResult reload()
        {
            var position = Request.QueryString["position"];

            if (position == "monthView")
            {
                ViewBag.position = "monthView";
            }
            else if (position == "agendaView")
            {
                ViewBag.position = "agendaView";
            }

            //判斷三種權限老闆(網管人員)、各部門主管、員工
            ViewBag.empid = EmpUser.EmployeeID;
            int Ocup_ID = EmpRepository.GetByID(EmpUser.EmployeeID).Emp_OccupationID;
            ViewBag.ocup_id = Ocup_ID;
            int dep_ID = (int)EmpRepository.GetByID(EmpUser.EmployeeID).Emp_DeptID;
            switch (Ocup_ID)
            {
                case 1:
                case 4:
                    {
                        var data = RepositorySchedule.GetAll();
                        return PartialView(data.AsEnumerable());
                    }
                case 2:
                    {
                        var data = RepositorySchedule.GetAll().Where(p => p.Emp_Information.Emp_DeptID == dep_ID);
                        return PartialView(data.AsEnumerable());
                    }
                case 3:
                    {
                        var data = RepositorySchedule.GetAll().Where(p => p.Emp_Information.EmployeeID == EmpUser.EmployeeID);
                        return PartialView(data.AsEnumerable());
                    }
                default:
                    {

                        return PartialView();
                    }
            }
        }
        [HttpPost]
        public ActionResult Test()
        {


            if (Request.Form["S_hour"] == null)
            {
                return null;
            }
            var Application_Reason = Request.Form["Application_Reason"];
            var S_hour = Request.Form["S_hour"];
            var S_min = Request.Form["S_min"];
            var E_hour = Request.Form["E_hour"];
            var E_min = Request.Form["E_min"];
            var EmployeeID = Request.Form["EmployeeID"];
            var Check_ID = Request.Form["Check_ID"];
            var LocationID = Request.Form["LocationID"];
            var Day = Request.Form["Day"];
            var Month = Request.Form["Month"];


            int I_month = 1;
            if (Month != null)
            {
                switch (Month)
                {
                    case "January":
                        I_month = (int)month.January;
                        break;
                    case "February":
                        I_month = (int)month.February;
                        break;
                    case "March":
                        I_month = (int)month.March;
                        break;
                    case "April":
                        I_month = (int)month.April;
                        break;
                    case "May":
                        I_month = (int)month.May;
                        break;
                    case "June":
                        I_month = (int)month.June;
                        break;
                    case "July":
                        I_month = (int)month.July;
                        break;
                    case "August":
                        I_month = (int)month.August;
                        break;
                    case "September":
                        I_month = (int)month.September;
                        break;
                    case "October":
                        I_month = (int)month.October;
                        break;
                    case "November":
                        I_month = (int)month.November;
                        break;
                    case "December":
                        I_month = (int)month.December;
                        break;
                }
            }
            I_month--;





            global::BookStore_Web_.Models.Schedule C_Schedule = new Models.Schedule();
            C_Schedule.EmployeeID = Convert.ToInt32(EmployeeID);
            //C_Schedule.Emp_Information = EmpRepository.GetByID(C_Schedule.EmployeeID);
            C_Schedule.Check_ID = Convert.ToInt32(Check_ID);
            C_Schedule.Application_Reason = Application_Reason;
            C_Schedule.ScheduleDateTime = new DateTime(2016, I_month, Convert.ToInt32(Day), Convert.ToInt32(S_hour), Convert.ToInt32(S_min), 0);
            C_Schedule.LocationID = Convert.ToInt32(LocationID);
            C_Schedule.Application_DateTime = new DateTime(2016, I_month, Convert.ToInt32(Day), Convert.ToInt32(E_hour), Convert.ToInt32(E_min), 0);

            RepositorySchedule.Create(C_Schedule);
            //儲存玩重抓一次資料庫把EmpID丟回去
            var ReScheduleID = RepositorySchedule.GetAll().Where(p => p.Application_Reason == Application_Reason).Select(p => new
            {
                ScheduleID = p.ScheduleID
            }
            );


            return Json(ReScheduleID, JsonRequestBehavior.AllowGet);

        }

        //抓地點資料傳回去放回select裡
        public ActionResult Location()
        {

            var location = RepositoryLocation.GetAll().Select(p => new
            {
                Location_Name = p.Location_Name,
                LocationID = p.LocationID,
            });

            return Json(location, JsonRequestBehavior.AllowGet);
        }

        //抓審核狀態資料傳回去select裡
        public ActionResult Status()
        {
            var status = RepositoryStatus.GetAll().Select(p => new
            {
                StatusID = p.Check_ID,
                StatusName = p.Status
            }
            );
            return Json(status, JsonRequestBehavior.AllowGet);
        }
        //依照內容名稱去搜尋做編輯

        #region
        [HttpPost]
        public ActionResult Edit(string searchText)
        {
            //新創造出來的屬性不可以有關連性的問題，不然呼叫時會失敗EmpInformation=p.Emp_Information,
            //當我要去抓關聯表的屬性時要怎麼抓

            var Edit_schedule = RepositorySchedule.GetAll().Where(p => p.Application_Reason == searchText).Select(p => new
            {

                ScheduleDateTimehour = p.ScheduleDateTime.Hour,
                ScheduleDateTimeMinute = p.ScheduleDateTime.Minute,
                Application_DateTimehour = p.Application_DateTime.Value.Hour,
                Application_DateTimeMinute = p.Application_DateTime.Value.Minute,
                Application_Reason = p.Application_Reason,
                Check_ID = p.Check_ID,
                LocationID = p.LocationID,
                EmployeeID = p.EmployeeID,
                ScheduleID = p.ScheduleID,


            }
            );
            var json = Json(Edit_schedule, JsonRequestBehavior.AllowGet);
            return json;
        }
        //額外加的部分結束    
        [HttpPost]
        public ActionResult ScheduleTableEdit()
        {
            var Application_Reason = Request.Form["Application_Reason"];
            //var scheduleID;抓傳過來的ID
            var S_hour = Request.Form["S_hour"];
            var S_min = Request.Form["S_min"];
            var E_hour = Request.Form["E_hour"];
            var E_min = Request.Form["E_min"];
            var EmployeeID = Request.Form["EmployeeID"];
            var Check_ID = Request.Form["Check_ID"];
            var LocationID = Request.Form["LocationID"];
            var Day = Request.Form["Day"];
            var Month = Request.Form["Month"];
            var ScheduleID = Request.Form["ScheduleID"];
            int I_month = 1;
            if (Month != null)
            {
                switch (Month)
                {
                    case "January":
                        I_month = (int)month.January;
                        break;
                    case "February":
                        I_month = (int)month.February;
                        break;
                    case "March":
                        I_month = (int)month.March;
                        break;
                    case "April":
                        I_month = (int)month.April;
                        break;
                    case "May":
                        I_month = (int)month.May;
                        break;
                    case "June":
                        I_month = (int)month.June;
                        break;
                    case "July":
                        I_month = (int)month.July;
                        break;
                    case "August":
                        I_month = (int)month.August;
                        break;
                    case "September":
                        I_month = (int)month.September;
                        break;
                    case "October":
                        I_month = (int)month.October;
                        break;
                    case "November":
                        I_month = (int)month.November;
                        break;
                    case "December":
                        I_month = (int)month.December;
                        break;
                }
            }
            global::BookStore_Web_.Models.Schedule C_Schedule = new Models.Schedule();
            C_Schedule.EmployeeID = Convert.ToInt32(EmployeeID);
            C_Schedule.ScheduleID = Convert.ToInt32(ScheduleID);
            C_Schedule.Check_ID = Convert.ToInt32(Check_ID);
            C_Schedule.Application_Reason = Application_Reason;
            C_Schedule.ScheduleDateTime = new DateTime(2016, 1, 1, 1, 1, 1);
            C_Schedule.ScheduleDateTime = new DateTime(2016, I_month - 1, Convert.ToInt32(Day), Convert.ToInt32(S_hour), Convert.ToInt32(S_min), 0);
            C_Schedule.LocationID = Convert.ToInt32(LocationID);
            C_Schedule.Application_DateTime = new DateTime(2016, I_month - 1, Convert.ToInt32(Day), Convert.ToInt32(E_hour), Convert.ToInt32(E_min), 0);
            RepositorySchedule.Update(C_Schedule);

            //寫一個json格式，一個arraylist裝用var 宣告的new {}物件
            //var abc = new { a = 3, b = new { c = 4, d = 5 } };
            //var ass=new { d = 5, e = new { c = 4, d = 5 } };
            var LocationName = RepositoryLocation.GetAll().Where(p => p.LocationID == Convert.ToInt32(LocationID)).FirstOrDefault().Location_Name;
            var Check_Name = CheckStatusRepository.GetAll().Where(p => p.Check_ID == Convert.ToInt32(Check_ID)).FirstOrDefault().Status;
            var emp_Name = EmpRepository.GetByID(Convert.ToInt32(EmployeeID)).Emp_Name;
            var email = EmpRepository.GetByID(Convert.ToInt32(EmployeeID)).Emp_E_Mail;
            //傳一個資料可以傳過去Email
            System.Collections.ArrayList arr = new System.Collections.ArrayList();
            var time = new { Year = "2016", Month = Month, Day = Day, S_hour = S_hour, S_min = S_min, E_hour = E_hour, E_min = E_min, LocationName = LocationName, Check_Name = Check_Name, emp_Name = emp_Name, Application_Reason = Application_Reason, Check_ID = Check_ID, email = email };
            arr.Add(time);


            return Json(arr, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult Email()
        {
            //Request.Form.AllKeys
            System.Collections.ArrayList R_data = new System.Collections.ArrayList();
            var Application_Reason = Request.Form["data[0][Application_Reason]"];
            var Check_ID = Request.Form["data[0][Check_ID]"];
            var Check_Name = Request.Form["data[0][Check_Name]"];
            var Day = Request.Form["data[0][Day]"];
            var E_hour = Request.Form["data[0][E_hour]"];
            var E_min = Request.Form["data[0][E_min]"];
            var LocationName = Request.Form["data[0][LocationName]"];
            var Month = Request.Form["data[0][Month]"];
            var S_hour = Request.Form["data[0][S_hour]"];
            var S_min = Request.Form["data[0][S_min]"];
            var Year = Request.Form["data[0][Year]"];
            var emp_Name = Request.Form["data[0][emp_Name]"];
            var email = Request.Form["data[0][email]"];


            MailAddress MessageFrom = new MailAddress("Eurasiaverify@gmail.com"); //發件人郵箱地址 
            string MessageTo = "p88204@gmail.com"; //收件人郵箱地址 
            string MessageSubject = "排班審核通知"; //郵件主題 
            string MessageBody = string.Format("{0}你好:<br><br>申請開始時間:<strong>{1}年{2}月{3}號{4}時{5}分</strong><br><br>申請結束時間:<strong>{6}年{7}月{8}號{9}時{10}分</strong><br><br>申請地點:<strong>{11}</strong><br><br>申請理由:<strong>{12}</strong><br><br>申請狀態:<strong>{13}</strong>", emp_Name, Year, Month, Day, S_hour, S_min, Year, Month, Day, E_hour, E_min, LocationName, Application_Reason, Check_Name);



            if (SendMail(MessageFrom, MessageTo, MessageSubject, MessageBody))
            {
                Response.Write("<script type='text/javascript'>alert('申請成功');</script>");
            }
            else
            {
                Response.Write("<script type='text/javascript'>alert('申請失敗');</script>");
            }
            return null;


        }

        public bool SendMail(MailAddress MessageFrom, string MessageTo, string MessageSubject, string MessageBody)   //發送驗證郵件
        {
            MailMessage message = new MailMessage();
            message.To.Add(MessageTo);
            message.From = MessageFrom;
            message.Subject = MessageSubject;
            message.SubjectEncoding = System.Text.Encoding.UTF8;
            message.Body = MessageBody;
            message.BodyEncoding = System.Text.Encoding.UTF8;
            message.IsBodyHtml = true; //是否為html格式 
            message.Priority = MailPriority.High; //發送郵件的優先等級             
            SmtpClient sc = new SmtpClient();
            sc.EnableSsl = true;//是否SSL加密
            sc.Host = "smtp.gmail.com"; //指定發送郵件的伺服器地址或IP 
            sc.Port = 587; //指定發送郵件埠 
            //sc.Credentials = new System.Net.NetworkCredential("Eurasiaverify@gmail.com", "Eurasia305");
            sc.Credentials = new System.Net.NetworkCredential("Eurasiaverify@gmail.com", "Eurasia305");
            try
            {
                sc.Send(message); //發送郵件 
            }
            catch (Exception e)
            {
                Response.Write(e.Message);
                return false;
            }
            return true;
        }


        public ActionResult ScheduleDetail()
        {
            //var a=Request.QueryString["abc"];load讀Request.form
            var EmployeeID = Convert.ToInt32(Request.Form["abc[0][EmployeeID]"]);
            var ScheduleID = Convert.ToInt32(Request.Form["abc[0][ScheduleID]"]);

            //底下出問題
            var SHDID = RepositorySchedule.GetByID(ScheduleID);
            BookStore_Web_.Models.VM.UserSchedule q = new Models.VM.UserSchedule();
            q.locationName = RepositoryLocation.GetByID(Convert.ToInt32(SHDID.LocationID)).Location_Name;
            q.status = RepositoryStatus.GetByID(SHDID.Check_ID).Status;
            q.name = EmpRepository.GetByID(EmployeeID).Emp_Name;
            q.Application_Reason = SHDID.Application_Reason;
            q.ScheduleID = SHDID.ScheduleID;
            q.Application_DateTime = SHDID.Application_DateTime.Value.AddMonths(1);
            q.ScheduleDateTime = SHDID.ScheduleDateTime.AddMonths(1);
            return View(q);
        }

        #endregion

        #endregion
        //====================================================================
        #region 油料補助
        public ActionResult reimbursment(int? page)
        {
            Session["name"] = EmpUser.Emp_Name.ToString();
            //只能看到同部門員工
            ViewBag.E = EmpUser.EmployeeID;
            var emp = db.Salary.Where(p => p.Emp_Information.Emp_Report == EmpUser.EmployeeID);
                return View(emp.ToList().ToPagedList(page ?? 1, 8));              
        }

        public ActionResult petrol()
        {
            Session["name"] = EmpUser.Emp_Name.ToString();
            ViewBag.E = EmpUser.EmployeeID;
            ViewBag.name = EmpUser.Emp_Name;
            var a = db.CheckInDetail.ToList();
            return View(a);
        }

        public ActionResult SalaryBrowse(string name)
        {
            Session["name"] = EmpUser.Emp_Name.ToString();
            ViewBag.search = name;
            var emp=db.Emp_Information.Where(p=>p.Emp_Name==name);
            if(emp!=null)
            {
                foreach( var i in emp)
                {
                  var  Emp = db.Salary.Where(p => p.EmployeeID == i.EmployeeID);
                  ViewBag.name = emp.First().Emp_Name+"的薪資詳細資料";
                  return View(Emp.ToList());
                }
            }
              return View();    
        }

        public ActionResult partialtable(string name)
        {
            var emp = db.CheckInDetail.Where(p => p.Emp_Information.Emp_Name == name);
       
            return PartialView(emp.ToList());
        }

        private int dist_count(int ID, int month)
        {
            return db.CheckInDetail.ToList().Where(p => p.EmployeeID == ID && p.CheckInDateTime.Value.Month == month && p.Dis_Between_Previous!=null).Sum(p => Convert.ToInt32(p.Dis_Between_Previous));
        }

        public ActionResult PartialLine()
        {
            return PartialView();
        }

        public ActionResult Partialpetrol()
        {
            ViewBag.E = EmpUser.EmployeeID;
            var a = db.CheckInDetail.ToList();
            return PartialView(a);
        }

        public ActionResult Allsalary(string name="")
        {
            allsalary p;
            List<allsalary> tt = new List<allsalary>();
            var oil = db.CheckInDetail.Where(n => n.Emp_Information.Emp_Name == name);
            var sa = db.Salary.Where(n => n.Emp_Information.Emp_Name == name);
            foreach (var item in sa)
            {
                for (int i = 1; i <= 12; i++)
                {
                    p = new allsalary();
                    p.Date = i;
                    if (item.DateTime == i)
                    {
                        var oilMoney = oil.Where(n => n.CheckInDateTime.Value.Month == i);
                        int totalMoney = 0;
                        foreach (var Money in oilMoney)
                        {
                            totalMoney += Convert.ToInt32(Money.Dis_Between_Previous);
                        }
                        p.Salary = item.Emp_Salary + totalMoney * 5;
                    }
                    p.Salary = p.Salary == 0 ? 0 : p.Salary;
                    tt.Add(p);
                }
            }
            return PartialView(tt);
        }

        public ActionResult partialpet(string name="")
        {
            ViewBag.name = name;
            var a = db.CheckInDetail.ToList();
            return PartialView(a);
        }

        #endregion
        //====================================================================
        #region 網管功能
        public ActionResult validIT(string password="", int ID=0)
        {
            var user = EmpRepository.GetByID(ID);
            if (user != null)
            {
                if (user.Emp_Password == password)
                {
                    if (user.Emp_DeptID == 1 || user.Emp_DeptID == null)
                    {
                        //比對無誤
                        return RedirectToAction("MIS", "IT");
                    }
                    else
                    {
                        return RedirectToAction("error");
                    }
                }
                else
                { 
                  //密碼比對錯誤
                    return RedirectToAction("error");
                }
            }
            else
            { 
              //查無此員工
            return RedirectToAction("error");
            }
        }

        public ActionResult error()
        {
            return View();
        }

        #endregion
    }
}