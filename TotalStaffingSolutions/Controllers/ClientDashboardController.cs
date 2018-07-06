using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TotalStaffingSolutions.Models;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using System.IO;

namespace TotalStaffingSolutions.Controllers
{
    public class ClientDashboardController : Controller
    {
        [AllowAnonymous]
        public ActionResult ConfirmAccount(string token)
        {
            try
            {
                var db = new TSS_Sql_Entities();
                if (token != null && token != "")
                {
                    var checkToken = db.ContactConfirmations.FirstOrDefault(s => s.ConfirmationToken == token);
                    if (checkToken != null) 
                    {
                        var datetimeCheck = DateTime.Compare(Convert.ToDateTime(checkToken.TokenExpiryTime), DateTime.Now);
                        if(datetimeCheck > 0)
                        {
                            checkToken.ConfirmationStatusId = 1;
                            db.SaveChanges();
                            ViewBag.Token = token;
                            return View();
                        }
                        else
                        {
                            checkToken.ConfirmationStatusId = 4;
                            db.SaveChanges();
                            return RedirectToAction("TokenExpired");
                        }
                    }
                    
                }
                return RedirectToAction("TokenNotFound");
            }
            catch (Exception ex)
            {
                return RedirectToAction("TokenNotFound");
            }
        }

        public ActionResult TokenExpired()
        {
            return View();
        }

        public ActionResult TokenNotFound()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ConfirmEmail(FormCollection fc)
        {
            var emailId = fc["Email"];
            var token = fc["Token"];

            var db = new TSS_Sql_Entities();
            var checkEmail = db.ContactConfirmations.FirstOrDefault(s => s.ConfirmationToken == token && s.CustomerContact.Email_id == emailId);
            if (checkEmail != null)
            {
                return RedirectToAction("RegisterUser", "Account",new { emailId , token});
            }
            return RedirectToAction("EmailConfirmationFailed");
        }

        public ActionResult EmailConfirmationFailed()
        {
            return View();
        }

        [Authorize(Roles ="User")]
        public ActionResult UserProfile()
        {
            var db = new TSS_Sql_Entities();
            var userId = User.Identity.GetUserId();
            var user = db.AspNetUsers.FirstOrDefault(s => s.Id == userId);
            var customercontactDetail = db.CustomerContacts.Where(s => s.Customer_id == user.Customer_id).ToList();
            ViewBag.ContactList = customercontactDetail;
            int? TotalTimeSheets = db.Timesheets.Where(s => s.Customer_Id_Generic == user.Customer_id).Count();
            ViewBag.TotalTimeSheets = (TotalTimeSheets == null) ? 0 : TotalTimeSheets;
            ViewBag.Customer = db.Customers.FirstOrDefault(s => s.Customer_id == user.Customer_id);
            return View(user);
        }

        [Authorize(Roles = "User")]
        public ActionResult AllTimeSheets()
        {
            var db = new TSS_Sql_Entities();
            var userId = User.Identity.GetUserId();
            var userObj = db.AspNetUsers.Find(userId);
            var TimeSheets = db.Timesheets.Where(s => s.Customer_Id_Generic == userObj.Customer_id).ToList();

            return View(TimeSheets);
        }


        public ActionResult EditProfile()
        {

            var userId = User.Identity.GetUserId();
            var db = new TSS_Sql_Entities();
            var user = db.AspNetUsers.Find(userId);
            return View(user);
        }

        [HttpPost]
        public ActionResult EditProfile(AspNetUser user, HttpPostedFileBase DisplayPicture)
        {
            var db = new TSS_Sql_Entities();
            var userObj = db.AspNetUsers.Find(user.Id);
            if (DisplayPicture != null)
            {
                string pic = System.IO.Path.GetFileName(DisplayPicture.FileName);
                string path = System.IO.Path.Combine(
                                       Server.MapPath("~/ProfileImages"), pic);

                DisplayPicture.SaveAs(path);


                using (MemoryStream ms = new MemoryStream())
                {
                    DisplayPicture.InputStream.CopyTo(ms);
                    byte[] array = ms.GetBuffer();
                }
                userObj.DisplayPicture = DisplayPicture.FileName;

            }

            return RedirectToAction("UserProfile", "ClientDashboard");
        }
    }
}
