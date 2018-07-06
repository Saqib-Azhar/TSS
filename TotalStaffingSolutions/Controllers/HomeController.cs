using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Security;
using TotalStaffingSolutions.Models;

namespace TotalStaffingSolutions.Controllers
{
    public class HomeController : Controller
    {
        

        private static string SenderEmailId = WebConfigurationManager.AppSettings["DefaultEmailId"];
        private static string SenderEmailPassword = WebConfigurationManager.AppSettings["DefaultEmailPassword"];
        private static int SenderEmailPort = Convert.ToInt32(WebConfigurationManager.AppSettings["DefaultEmailPort"]);
        private static string SenderEmailHost = WebConfigurationManager.AppSettings["DefaultEmailHost"];

        private static string TSSLiveSiteURL = WebConfigurationManager.AppSettings["TSSLiveSiteURL"];

        
        public ActionResult Index(int? a = 1)
        {
            switch (a)
            {
                case 0:
                    {
                        return View();
                        break;
                    }
                case 1:
                    {
                        return RedirectToAction("Dashboard", "TSSManage");
                        break;
                    }

            }
            return View();

        }



        public bool UpdateCustomers()
        {
            try
            {
                TSSServices.TSSWebServicesSoapClient srvDBObj = new TSSServices.TSSWebServicesSoapClient();
                var usersList = srvDBObj.GetCustomersData();
                var db = new TSS_Sql_Entities();

                foreach (var item in usersList)
                {
                    var custCheck = db.Customers.FirstOrDefault(s => s.Customer_id == item.Cuccustid && s.Name == item.Cucname);
                    if (custCheck == null)
                    {
                        var userobj = new TotalStaffingSolutions.Models.Customer();
                        userobj.Name = item.Cucname;
                        userobj.Customer_id = item.Cuccustid;
                        userobj.Address1 = item.Cucaddr1;
                        userobj.Address2 = item.Cucaddr2;
                        userobj.City = item.Cuccity;
                        userobj.State = item.Cucstate;
                        userobj.CustomerAdded = DateTime.Now;
                        userobj.ZipCode = item.Cuczipcode;
                        userobj.Country = item.Cuccountry;
                        userobj.Created_at = DateTime.Now;
                        userobj.PhoneNumber = item.Cucpono;
                        userobj.Status = item.Cucstatus;
                        userobj.ENTITY_ADDED_AT = DateTime.Now;
                        db.Customers.Add(userobj);
                        db.SaveChanges();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {

                return false;
            }

        }
        public bool UpdateEmployees()
        {
            try
            {
                TSSServices.TSSWebServicesSoapClient srvDBObj = new TSSServices.TSSWebServicesSoapClient();
                TSSServices.Empmast[] employeesList = srvDBObj.GetEmployeesData();
                var db = new TSS_Sql_Entities();

                foreach (var item in employeesList)
                {
                    var empCheck = db.Employees.FirstOrDefault(s => s.First_name == item.Emcfname && s.Last_name == item.Emclname);
                    if (empCheck == null && Convert.ToInt32(empCheck.User_id) != Convert.ToInt32(item.Emcempid))
                    {
                        var employeeObj = new TotalStaffingSolutions.Models.Employee();
                        employeeObj.User_id = Convert.ToInt32(item.Emcempid);
                        employeeObj.First_name = item.Emcfname;
                        employeeObj.Last_name = item.Emclname;
                        employeeObj.Middle_name = item.Emcmname;
                        employeeObj.Created_at = DateTime.Now;
                        employeeObj.Address1 = item.Emcaddr1;
                        employeeObj.Address2 = item.Emcaddr2;
                        employeeObj.City = item.Emccity;
                        employeeObj.State = item.Emcstate;
                        employeeObj.ZipCode = item.Emczipcode;
                        employeeObj.Country = item.Emccountry;
                        employeeObj.Marital_status = item.Emcmarital;
                        employeeObj.Gender = item.Emcsex;
                        //employeeObj.DateOfBirth = Convert.ToDateTime(item.Emtbirth);
                        //employeeObj.Hire = Convert.ToDateTime(item.Emthire);
                        //employeeObj.ReHire = Convert.ToDateTime(item.Emtrehire);
                        employeeObj.EmployeeType = item.Emcemptype;
                        employeeObj.ENTITY_ADDED_AT = DateTime.Now;
                        employeeObj.Updated_at = DateTime.Now;

                        db.Employees.Add(employeeObj);
                        db.SaveChanges();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {

                return false;
            }

        }

        public bool UpdateCustomerContacts()
        {
            try
            {
                TSSServices.TSSWebServicesSoapClient srvDBObj = new TSSServices.TSSWebServicesSoapClient();
                var contactsList = srvDBObj.GetCustomerContacts();
                var db = new TSS_Sql_Entities();
                foreach (var item in contactsList)
                {

                    var contactCheck = db.CustomerContacts.FirstOrDefault(s => s.Customer_key == item.Ccccusmast && s.Contact_key == item.Ccccuscont);
                    if (contactCheck == null)
                    {
                        var contactObj = new TotalStaffingSolutions.Models.CustomerContact();
                        contactObj.Contact_code = item.Ccccatcde;
                        contactObj.Contact_description = item.Ccccontcde;
                        contactObj.Contact_key = item.Ccccuscont;
                        contactObj.Contact_name = item.Ccccontact;
                        contactObj.Contact_notes = item.Ccmnotes;
                        contactObj.Customer_id = item.Customer_id;
                        contactObj.Customer_key = item.Ccccusmast;
                        contactObj.Email_id = item.Cccemail;
                        contactObj.Phone_1 = item.Cccphone1;
                        contactObj.Phone_2 = item.Cccphone2;
                        contactObj.Phone_3 = item.Cccphone3;
                        contactObj.ENTITY_ADDED_AT = DateTime.Now;
                        db.CustomerContacts.Add(contactObj);
                        db.SaveChanges();

                        if (item.Cccemail != null && item.Cccemail != "")
                        {
                            var ConfirmationToken = Membership.GeneratePassword(10, 0);
                            ConfirmationToken = Regex.Replace(ConfirmationToken, @"[^a-zA-Z0-9]", m => "9");
                            var savedContactObj = db.CustomerContacts.OrderByDescending(s => s.Id).FirstOrDefault(s => s.Customer_id == contactObj.Customer_id);
                            var contactStatusObj = new ContactConfirmation();
                            contactStatusObj.ContactId = savedContactObj.Id;
                            contactStatusObj.LastUpdate = DateTime.Now;
                            contactStatusObj.TokenCreationTime = contactStatusObj.LastUpdate;
                            contactStatusObj.TokenExpiryTime = DateTime.Now.AddHours(24);
                            contactStatusObj.ConfirmationToken = ConfirmationToken;
                            contactStatusObj.ConfirmationStatusId = 1;
                            db.ContactConfirmations.Add(contactStatusObj);
                            db.SaveChanges();
                            try
                            {
                                var fromAddress = new MailAddress(SenderEmailId, "Total Staffing Solution");
                                var toAddress = new MailAddress("saqibabdullahazhar@gmail.com", savedContactObj.Contact_name);
                                string fromPassword = SenderEmailPassword;
                                string subject = "Total Staffing Solution: Account Confirmation";
                                string body = "<b>Hi " + savedContactObj.Contact_name + "!</b><br />Please confirm your email by clicking on following link <br /> <a href='" + TSSLiveSiteURL + "/ClientDashboard/ConfirmAccount?token=" + ConfirmationToken + "'>Confirmation Link</a><br /><small style='text-align:center;'>(This Link is active for next 24 hours, Please make sure to Enable your account before " + DateTime.Now.AddHours(24) + ")</small>";

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
                                    smtp.Send(message);
                                }
                                ///
                                var savedContactConfirmationObj = db.ContactConfirmations.OrderByDescending(s => s.Id).FirstOrDefault(s => s.ContactId == contactStatusObj.ContactId);
                                savedContactConfirmationObj.ConfirmationStatusId = 2;
                                savedContactConfirmationObj.LastUpdate = DateTime.Now;
                                db.SaveChanges();

                            }
                            catch (Exception ex)
                            {
                                var savedContactConfirmationObj = db.ContactConfirmations.OrderByDescending(s => s.Id).FirstOrDefault(s => s.ContactId == contactStatusObj.ContactId);
                                savedContactConfirmationObj.ConfirmationStatusId = 4;
                                savedContactConfirmationObj.LastUpdate = DateTime.Now;
                                db.SaveChanges();
                            }
                        }


                    }
                }
                db.SaveChanges();
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }
        [Authorize(Roles = "Admin")]

        public bool Test()
        {
            try
             {
                var fromAddress = new MailAddress("tss@viretech.net", "Total Staffing Solution");
                var toAddress = new MailAddress("saqibabdullahazhar@gmail.com", "Saqib Abdullah Azhar");
                string fromPassword = SenderEmailPassword;
                string subject = "Total Staffing Solution: Account Confirmation";
                string body = "<b>Hi Saqib testing email</b>";

                var smtp = new SmtpClient
                {
                    Host = "mail.viretechnologies.com",
                    Port = SenderEmailPort,
                    EnableSsl = false,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = new NetworkCredential("tss@viretech.net", "P@ssw0rd1"),
                    Timeout = 20000
                };
                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    IsBodyHtml = true,
                    Subject = subject,
                    Body = body,

                })
                {
                    smtp.Send(message);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}