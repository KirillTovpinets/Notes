using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ForteGroupTest.Models;
using ForteGroupTest.Tools;
using System.Drawing.Imaging;

namespace ForteGroupTest.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(LogedUser user)
        {
            ForteGroupTestEntities dc = new ForteGroupTestEntities();

            if (ModelState.IsValid) { 
                foreach (Users record in dc.Users)
                {
                    if (record.Login.ToString().Equals(user.Login.ToString()) &&
                        record.Password.ToString().Equals(user.Password.ToString()))
                    {
                        Session["LogedUserId"] = record.UserId;
                        Session["LogedUserName"] = record.Name;
                        Session["LogedUserSirname"] = record.Sirname;
                        Session["LogedUserPatername"] = record.Patername;
                        Session["isAdmin"] = record.isAdmin;
                        return RedirectToActionPermanent("LoadProfile");
                    }
                }
            }
            ViewBag.Message = "Incorrect login or password";
            return View(user);
        }
        public ActionResult LoadProfile()
        {
            //Create the html of the notification about the notes to be removed by administators
            if (Session["isAdmin"].ToString().Equals("0"))
            {
                ForteGroupTestEntities dc = new ForteGroupTestEntities();
                string SmessageHtml = "<ul>";
                bool hasNotif = false;
                foreach (Notifications item in dc.Notifications)
                {
                    if (item.RecieverId == Convert.ToInt32(Session["LogedUserId"]) &&
                        item.isRead == 0)
                    {
                        hasNotif = true;
                        SmessageHtml += "<li>" + item.Records.Content + "<div class='date'>" + item.Records.Date.ToShortDateString() + "</div></li>";
                    }
                }

                SmessageHtml += "</ul>";

                MvcHtmlString messageHtml = MvcHtmlString.Create(SmessageHtml);
                if (hasNotif)
                {
                    ViewBag.Message = messageHtml;
                }
            }
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(RegUser NewUser)
        {
            if (NewUser.Captcha != (string)Session[CaptchaImage.CaptchaValueKey])
            {
                ModelState.AddModelError("Captcha", "Incorrect input of number");
                return View(NewUser);
            }
            ForteGroupTestEntities dc = new ForteGroupTestEntities();
            Users UserToRec = new Users();

            UserToRec.Login = NewUser.Login;
            UserToRec.Password = NewUser.Password;
            UserToRec.Name = NewUser.Name;
            UserToRec.Patername = NewUser.Patername;
            UserToRec.Sirname = NewUser.Sirname;
            
            if (NewUser.AdminKey != null)
            {
                if (!NewUser.AdminKey.Equals("SystemAdmin"))
                {
                    ModelState.AddModelError("AdminKey", "Incorrect admin key");
                    return View(NewUser);
                }
                UserToRec.isAdmin = 1;
            }
            else
            {
                UserToRec.isAdmin = 0;
            }
            

            dc.Users.Add(UserToRec);
            dc.SaveChanges();
            return View("Index");
        }

        public ActionResult Captcha()
        {
            Session[CaptchaImage.CaptchaValueKey] = new Random(DateTime.Now.Millisecond).Next(1111, 9999).ToString();
            var ci = new CaptchaImage(Session[CaptchaImage.CaptchaValueKey].ToString(), 211, 50, "Arial");
            this.Response.Clear();
            this.Response.ContentType = "image/jpeg";

            ci.Image.Save(this.Response.OutputStream, ImageFormat.Jpeg);

            ci.Dispose();
            return null;
        }
        public ActionResult CreateNote()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateNote(UserRecord newRec)
        {
            ForteGroupTestEntities dc = new ForteGroupTestEntities();
            Records RecToSave = new Records();

            RecToSave.AuthorId = Convert.ToInt32(Session["LogedUserId"].ToString());
            RecToSave.Content = newRec.Content;
            RecToSave.Date = DateTime.Now;
            RecToSave.isRemoved = 0;

            dc.Records.Add(RecToSave);
            dc.SaveChanges();
            ViewBag.Message = "Note have been saved successfuly";
            return View();
        }

        [HttpGet]
        public ActionResult DeleteNoteUser()
        {
            ForteGroupTestEntities dc = new ForteGroupTestEntities();
            int RecId = Convert.ToInt32(Request.QueryString["id"]);
            var RecToDelete = dc.Records.Where(a => a.RecordId.Equals(RecId)).FirstOrDefault();
            if(Session["isAdmin"].ToString().Equals("1"))
            { 
                if(RecToDelete.isRemoved == 1)
                { 
                    dc.Records.Remove(RecToDelete);
                }
                else
                {
                    Notifications newNotif = new Notifications();
                    newNotif.isConfirmed = 0;
                    newNotif.isRead = 0;
                    newNotif.RecId = RecId;
                    newNotif.RecieverId = RecToDelete.AuthorId;
                    dc.Notifications.Add(newNotif);
                    ViewBag.Message = "The author of the note have been sent a notification." +
                        " The note will be deleted after the author's confirmation.";
                }
                dc.SaveChanges();
                return View("Admin");
            }
            else
            {
                RecToDelete.isRemoved = 1;
                dc.SaveChanges();
                return View("LoadProfile");
            }
        }

        public ActionResult EditNoteUser()
        {
            int id = Convert.ToInt32(Request.QueryString["id"]);
            ViewBag.Edit = true;
            ViewBag.NoteId = id;
            if (Session["isAdmin"].ToString().Equals("1"))
            {
                return View("Admin");
            }
            return View("LoadProfile");
        }

        [HttpPost]
        public ActionResult EditNoteUser(Records EditRec)
        {
            ForteGroupTestEntities dc = new ForteGroupTestEntities();
            var DbRec = dc.Records.Where(a => a.RecordId.Equals(EditRec.RecordId)).FirstOrDefault();
            DbRec.Content = EditRec.Content;
            dc.SaveChanges();
            ViewBag.Edit = null;
            if (Session["isAdmin"].ToString().Equals("1"))
            {
                return View("Admin");
            }
            return View("LoadProfile");
        }

        public ActionResult Admin()
        {
            return View();
        }

        [HttpPost]
        //Axaj function
        public ActionResult ConfirmDelete()
        {
            ForteGroupTestEntities dc = new ForteGroupTestEntities();

            foreach (Records item in dc.Records)
            {
                if (item.AuthorId == Convert.ToInt32(Session["LogedUserId"]))
                {
                    var DelRec = dc.Notifications.Where(a => a.RecId.Equals(item.RecordId)).FirstOrDefault();
                    if (DelRec != null)
                    {
                        dc.Notifications.Remove(DelRec);
                        dc.Records.Remove(item);
                    }
                }
            }
            return View("LoadProfile");
        }
    }
}
