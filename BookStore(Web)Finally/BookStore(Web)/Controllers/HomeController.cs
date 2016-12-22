using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BookStore_Web_.Models;
using BookStore_Web_.Models.VM;
using System.Net.Mail;

namespace BookStore_Web_.Controllers
{
    public class HomeController : Controller
    {
        Repository<Emp_Information> EmpRepository = new Repository<Emp_Information>(new BookStoreObject_Entities());
        BookStoreObject_Entities db = new BookStoreObject_Entities();
        // GET: Home
       
        internal static int EmpID;
        internal static Emp_Information User = null;

        public ActionResult Index(Emp_Information emp, string EmpPassword)
        {

            if (EmpRepository.GetByID(emp.EmployeeID) != null)
                {
                    var q = EmpRepository.GetByID(emp.EmployeeID);
                    if (q.Emp_Password == EmpPassword)
                    {              
                        User = EmpRepository.GetByID(emp.EmployeeID);
                        EmpID = q.EmployeeID;
                        if (q.Emp_OccupationID == 1 || q.Emp_OccupationID == 2)
                        {
                            return RedirectToAction("Index", "Admin");
                        }
                        else
                        {
                            return RedirectToAction("Index", "Employee");
                        }
                    }
                    else
                    {
                        return RedirectToAction("login");
                    }
                }
                else
                {
                    return RedirectToAction("login");
                }
        }

        public ActionResult Login()
        {
            return PartialView();
        }

        public ActionResult lo()
        {
            return View();
        }

        public ActionResult Desk()
        {
            return View();
        }

        public ActionResult forgetpassword()
        {
            return PartialView();
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

        [HttpPost]
        public ActionResult Reset(Emp_Information info)
        {
            Emp_Information c = EmpRepository.GetByID(info.EmployeeID);
            if (c != null)
            {
                if (c.Emp_Identity == info.Emp_Identity)
                {
                    c.Emp_Password = info.Emp_Password;
                    EmpRepository.Update(c);

                    //傳送信件
                    Emp_Information q = EmpRepository.GetByID(info.EmployeeID);

                    MailAddress MessageFrom = new MailAddress("Eurasiaverify@gmail.com"); //發件人郵箱地址 
                    string MessageTo = "p88204@gmail.com"; //收件人郵箱地址 
                    string MessageSubject = "基本資料修改"; //郵件主題 
                    string MessageBody = "您的密碼為" + c.Emp_Password + "," + "請進行信箱驗證來完成您修改密碼的最後一步,點擊下面的連結返回登入頁面：<br><a href='http://localhost:49330/Home/Login/' class='btn btn-info'>返回登入頁面</a>"; //郵件內容 （一般是一個網址鏈接，生成隨機數加驗證id參數，點擊去網站驗證。）

                    if (SendMail(MessageFrom, MessageTo, MessageSubject, MessageBody))
                    {
                        return RedirectToAction("Login", "Home");
                    }
                    else
                    {
                        return RedirectToAction("Login", "Home");
                    }
                    if (c != null)
                    {
                        return Content("成功");
                    }
                    else
                    {
                        return Content("失敗");
                    }

                }
                return RedirectToAction("Login", "Home");

            }
            else
            {
                ViewBag.msg = "查無此員工";
                return PartialView("Reset", "Home");
            }
        }

        static ChatModel chatModel;
        public ActionResult Chatroom(string user, bool? logOn, bool? logOff, string chatMessage)
        {
            try
            {
                if (chatModel == null) chatModel = new ChatModel();

                //trim chat history if needed
                if (chatModel.ChatHistory.Count > 100)
                    chatModel.ChatHistory.RemoveRange(0, 90);

                if (!Request.IsAjaxRequest())
                {
                    //first time loading
                    return View(chatModel);
                }
                else if (logOn != null && (bool)logOn)
                {
                    //check if nickname already exists
                    if (chatModel.Users.FirstOrDefault(u => u.NickName == user) != null)
                    {
                        throw new Exception("This nickname already exists");
                    }
                    else if (chatModel.Users.Count > 10)
                    {
                        throw new Exception("The room is full!");
                    }
                    else
                    {
                        #region create new user and add to lobby
                        chatModel.Users.Add(new ChatModel.ChatUser()
                        {
                            NickName = user,
                            LoggedOnTime = DateTime.Now,
                            LastPing = DateTime.Now
                        });

                        //inform lobby of new user
                        chatModel.ChatHistory.Add(new ChatModel.ChatMessage()
                        {
                            Message = "User '" + user + "' logged on.",
                            When = DateTime.Now
                        });
                        #endregion

                    }

                    return PartialView("Lobby", chatModel);
                }
                else if (logOff != null && (bool)logOff)
                {
                    LogOffUser(chatModel.Users.FirstOrDefault(u => u.NickName == user));
                    return PartialView("Lobby", chatModel);
                }
                else
                {

                    ChatModel.ChatUser currentUser = chatModel.Users.FirstOrDefault(u => u.NickName == user);

                    //remember each user's last ping time
                    currentUser.LastPing = DateTime.Now;

                    #region remove inactive users
                    List<ChatModel.ChatUser> removeThese = new List<ChatModel.ChatUser>();
                    foreach (Models.ChatModel.ChatUser usr in chatModel.Users)
                    {
                        TimeSpan span = DateTime.Now - usr.LastPing;
                        if (span.TotalSeconds > 15)
                            removeThese.Add(usr);
                    }
                    foreach (ChatModel.ChatUser usr in removeThese)
                    {
                        LogOffUser(usr);
                    }
                    #endregion

                    #region if there is a new message, append it to the chat
                    if (!string.IsNullOrEmpty(chatMessage))
                    {
                        chatModel.ChatHistory.Add(new ChatModel.ChatMessage()
                        {
                            ByUser = currentUser,
                            Message = chatMessage,
                            When = DateTime.Now
                        });
                    }
                    #endregion

                    return PartialView("ChatHistory", chatModel);
                }
            }
            catch (Exception ex)
            {
                //return error to AJAX function
                Response.StatusCode = 500;
                return Content(ex.Message);
            }
        }

         public void LogOffUser(ChatModel.ChatUser user)
            {
                chatModel.Users.Remove(user);
                chatModel.ChatHistory.Add(new ChatModel.ChatMessage()
                {
                    Message = "User '" + user.NickName + "' logged off.",
                    When = DateTime.Now
                });
            }

         public ActionResult Lobby()
         {
             return PartialView("Lobby");
         }

         public ActionResult ChatHistory()
         {
             return PartialView("ChatHistory");
         }
    }
}