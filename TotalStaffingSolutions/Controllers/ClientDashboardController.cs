using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TotalStaffingSolutions.Models;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using System.IO;
using System.Net.Mail;
using System.Net;
using System.Web.Configuration;

namespace TotalStaffingSolutions.Controllers
{
    public class ClientDashboardController : Controller
    {

        private static string SenderEmailId = WebConfigurationManager.AppSettings["DefaultEmailId"];
        private static string SenderEmailPassword = WebConfigurationManager.AppSettings["DefaultEmailPassword"];
        private static int SenderEmailPort = Convert.ToInt32(WebConfigurationManager.AppSettings["DefaultEmailPort"]);
        private static string SenderEmailHost = WebConfigurationManager.AppSettings["DefaultEmailHost"];

        private static string TSSLiveSiteURL = WebConfigurationManager.AppSettings["TSSLiveSiteURL"];
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
                ExceptionHandlerController.infoMessage(ex.Message);
                ExceptionHandlerController.writeErrorLog(ex);
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
            var TimeSheets = db.Timesheets.Where(s => s.Customer_Id_Generic == userObj.Customer_id && s.Status_id != 1).ToList();

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

        [Authorize(Roles="User")]
        public JsonResult RejectTimeSheet(int timesheetId, string reason)
        {
            try
            {
                var db = new TSS_Sql_Entities();
                var userid = User.Identity.GetUserId();
                var user = db.AspNetUsers.FirstOrDefault(s => s.Id == userid);
                var rejectionObj = new RejectedTimesheet();
                rejectionObj.TimeSheetId = timesheetId;
                rejectionObj.RejectionReason = reason;
                rejectionObj.RejectedAt = DateTime.Now;
                rejectionObj.IsUpdatedOrDeleted = false;
                rejectionObj.RejecetedByCustomerId = Convert.ToInt32(user.Customer_id);
                db.RejectedTimesheets.Add(rejectionObj);
                var timesheet = db.Timesheets.FirstOrDefault(s => s.Id == timesheetId);
                timesheet.Status_id = 4;
                db.SaveChanges();

                ///////////////////////////////////ADMIN EMAIL UPDATE/////////////////////////////////////
                #region ADMIN EMAIL UPDATE
                if (User.IsInRole("User"))
                {
                    try
                    {
                        var AdminId = timesheet.Created_By;
                        var admin = db.AspNetUsers.FirstOrDefault(s => s.Id == AdminId);
                        var fromAddress = new MailAddress(SenderEmailId, "Total Staffing Solution");
                        var toAddress = new MailAddress("sazhar@viretechnologies.com", admin.Email);
                        string fromPassword = SenderEmailPassword;
                        string subject = "Total Staffing Solution: Timesheet Update";
                        string body = "<b>Hello " + admin.UserName + "!</b><br />Client has Rejected the timesheet<br /> <a href='" + TSSLiveSiteURL + "/TSSManage/TimeSheetDetails/" + timesheet.Id + "'>Timesheet Link</a><br />";

                        var smtp = new SmtpClient
                        {
                            Host = SenderEmailHost,
                            Port = SenderEmailPort,
                            EnableSsl = false,
                            DeliveryMethod = SmtpDeliveryMethod.Network,
                            Credentials = new NetworkCredential(fromAddress.Address, fromPassword),
                            Timeout = 20000
                        };
                        using (var message = new MailMessage(fromAddress, toAddress)
                        {
                            IsBodyHtml = true,
                            Subject = subject,
                            Body = body,


                        })
                        {
                            //message.CC.Add("jgallelli@4tssi.com");
                            //message.CC.Add("payroll@4tssi.com");
                            smtp.Send(message);
                        }
                        ///


                    }
                    catch (Exception ex)
                    {
                        ExceptionHandlerController.infoMessage(ex.Message);
                        ExceptionHandlerController.writeErrorLog(ex);

                    }
                }
                #endregion
                //////////////////////////////////////////////////////////////////////////////////////////
                return Json("Rejected Successfully!", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ExceptionHandlerController.infoMessage(ex.Message);
                ExceptionHandlerController.writeErrorLog(ex);
                return Json("Something went wrong",JsonRequestBehavior.AllowGet);
            }
        }
    }
}
