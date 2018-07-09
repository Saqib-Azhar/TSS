﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Security;
using TotalStaffingSolutions.Models;
//using System.Text.RegularExpressions;
//using System.Threading.Tasks;

namespace TotalStaffingSolutions.Controllers
{

    public class TSSManageController : Controller
    {
        private static List<Employee> EmployeesStaticList = null;

        private static string SenderEmailId = WebConfigurationManager.AppSettings["DefaultEmailId"];
        private static string SenderEmailPassword = WebConfigurationManager.AppSettings["DefaultEmailPassword"];
        private static int SenderEmailPort = Convert.ToInt32(WebConfigurationManager.AppSettings["DefaultEmailPort"]);
        private static string SenderEmailHost = WebConfigurationManager.AppSettings["DefaultEmailHost"];

        private static string TSSLiveSiteURL = WebConfigurationManager.AppSettings["TSSLiveSiteURL"];
        // GET: Manage
        [Authorize(Roles = "Admin")]
        public ActionResult Dashboard()
        {
            try
            {
                var db = new TSS_Sql_Entities();
                return View(db.Timesheets.ToList()); 
            }
            catch (Exception ex)
            {
                infoMessage(ex.Message);
                writeErrorLog(ex);
            }
            var list = new List<Timesheet>();

            return View(list);
        }




        public static void infoMessage(string _message)
        {
            StreamWriter sw = null;
            try
            {
                sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\LogFile.txt", true);
                sw.WriteLine(DateTime.Now.ToString() + " " + _message);
                sw.Flush();
                sw.Close();
            }
            catch (Exception)
            {

                throw;
            }
        }
        public static void writeErrorLog(Exception ex)
        {
            StreamWriter sw = null;
            try
            {
                sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\LogFile.txt", true);
                sw.WriteLine(DateTime.Now.ToString() + " " + ex.Source.ToString().Trim() + " " + ex.Message.ToString().Trim());
                sw.Flush();
                sw.Close();
            }
            catch (Exception exp)
            {

                throw exp;
            }
        }










        [Authorize(Roles = "Admin")]
        public ActionResult Branches()
        {
            var db = new TSS_Sql_Entities();

            return View(db.Branches.ToList());
        }

        [Authorize(Roles = "Admin")]
        public ActionResult AddBranch()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        public ActionResult AddNewBranch(Branch branch)
        {
            try
            {
                var db = new TSS_Sql_Entities();
                branch.Created_at = DateTime.Now;
                branch.Updated_at = DateTime.Now;
                db.Branches.Add(branch);
                db.SaveChanges();
            }
            catch (Exception)
            {
            }
            return RedirectToAction("Branches", "TSSManage");
        }

        [Authorize(Roles = "Admin")]
        public ActionResult EditBranch(int Id)
        {
            try
            {
                var db = new TSS_Sql_Entities();
                return View(db.Branches.Find(Id));
            }
            catch (Exception)
            {
                return RedirectToAction("Branches", "TSSManage");
            }
        }

        [Authorize(Roles = "Admin")]
        public ActionResult EditExistingBranch(Branch branch)
        {
            try
            {
                var db = new TSS_Sql_Entities();
                var findBranch = db.Branches.FirstOrDefault(s => s.Id == branch.Id);
                findBranch.Name = branch.Name;
                findBranch.Organization_id = branch.Organization_id;
                findBranch.Updated_at = DateTime.Now;
                findBranch.Email = branch.Email;
                findBranch.Created_at = DateTime.Now;
                findBranch.Branch_Manager = branch.Branch_Manager;
                findBranch.Phone = branch.Phone;
                findBranch.Address = branch.Address;
                db.SaveChanges();

            }
            catch (Exception)
            {
            }
            return RedirectToAction("Branches", "TSSManage");
        }

        [Authorize(Roles = "Admin")]
        public ActionResult AddClient()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        public ActionResult AddNewCompany(Company company)
        {
            try
            {
                var db = new TSS_Sql_Entities();
                company.Created_at = DateTime.Now;
                db.Companies.Add(company);
                db.SaveChanges();
            }
            catch (Exception)
            {

                throw;
            }
            return RedirectToAction("Dashboard", "TSSManage");
        }

        [Authorize(Roles = "Admin")]
        public ActionResult AllClients()
        {
            var db = new TSS_Sql_Entities();

            return View(db.Customers.ToList());
        }

        [Authorize(Roles = "Admin")]
        public ActionResult ClientDetails(int Id)
        {
            var db = new TSS_Sql_Entities();
            try
            {
                var a = db.Customers.FirstOrDefault(x => x.Id == Id);
                var userObject = db.AspNetUsers.FirstOrDefault(s => s.Customer_id == a.Customer_id);
                ViewBag.ClientContacts = db.CustomerContacts.Where(s => s.Customer_id == a.Customer_id).ToList();
                ViewBag.DisplayPicture = userObject.DisplayPicture;
                var timeSheets = db.Timesheets.Where(s => s.Customer_id == Id).ToList();
                if (timeSheets.Count == 0)
                {
                    var tempTimeSheet = new Timesheet();
                    tempTimeSheet.Customer = db.Customers.FirstOrDefault(s => s.Id == Id);
                    timeSheets.Add(tempTimeSheet);
                }
                return View(timeSheets);
            }
            catch
            {
                var timeSheets = new List<Timesheet>();
                var tempTimeSheet = new Timesheet();
                tempTimeSheet.Customer = db.Customers.FirstOrDefault(s => s.Id == Id);
                timeSheets.Add(tempTimeSheet);
                return View(timeSheets);
            }
        }

        [Authorize(Roles = "Admin")]
        public ActionResult AddTimeSheet(Int32 CustomerId)
        {
            var db = new TSS_Sql_Entities();

            if(EmployeesStaticList == null)
            {
                #region commentedPart
                //var stands = db.Employees.ToList();
                //IEnumerable<SelectListItem> selectList = from s in stands
                //                                         select new SelectListItem
                //                                         {
                //                                             Value = s.Id.ToString(),
                //                                             Text = s.First_name + " " + s.Last_name
                //                                         };
                //ViewBag.Employees = new SelectList(selectList, "Value", "Text");

                //string html = "<select class='form-control js-example-basic-single' id='dropdownid' name='Employees'>";
                //foreach (var item in stands)
                //{
                //    html = html + "<option value='" + item.Id + "'>" + item.First_name + " " + item.Last_name + "</option>";

                //}
                //html = html + "</select>";

                //ViewBag.DropDownHtml = html;
                #endregion



                EmployeesStaticList = db.Employees.ToList();





            }
            return View(db.Customers.FirstOrDefault(a=>a.Id == CustomerId));
        }

        [Authorize]
        public JsonResult AddTimeSheetDetails(Timesheet timesheet, List<Timesheet_summaries> timeSheet_summary,List<Timesheet_details> timeSheet_DetailsList)
        {
            try
            {
                    timesheet.Created_at = DateTime.Now;
                
                var db = new TSS_Sql_Entities();
                var NewTimeSheet = new Timesheet();
                NewTimeSheet.Created_at = timesheet.Created_at;
                NewTimeSheet.Customer_id = timesheet.Customer_id;
                NewTimeSheet.End_date = timesheet.End_date;
                NewTimeSheet.For_internal_employee = timesheet.For_internal_employee;
                NewTimeSheet.Note = timesheet.Note;
                NewTimeSheet.Organization_id = timesheet.Organization_id;
                NewTimeSheet.Po_number = timesheet.Po_number;
                NewTimeSheet.Sent = timesheet.Sent;
                NewTimeSheet.Signature = timesheet.Signature;
                NewTimeSheet.Total_employees = timesheet.Total_employees;
                NewTimeSheet.Total_hours = timesheet.Total_hours;
                NewTimeSheet.Updated_at = timesheet.Updated_at;
                NewTimeSheet.Submit_by_client = false;
                NewTimeSheet.Sent = false;
                var customer = db.Customers.FirstOrDefault(s => s.Id == timesheet.Customer_id);
                if(customer != null)
                    NewTimeSheet.Customer_Id_Generic = customer.Customer_id;
                NewTimeSheet.Status_id = 1;

                db.Timesheets.Add(NewTimeSheet);
                db.SaveChanges();

                //var timesheetObj = db.Timesheets.FirstOrDefault(s => s.Created_at == timesheet.Created_at && s.Customer_id == timesheet.Customer_id);
                var timesheetObj = db.Timesheets.OrderByDescending(s=>s.Created_at).FirstOrDefault(s=>s.Customer_id == timesheet.Customer_id);

                foreach (var item in timeSheet_summary)
                {
                    item.Created_at = DateTime.Now;
                    var NewTimeSheetSummary = new Timesheet_summaries();
                    //NewTimeSheetSummary.Created_at = timeSheet_summary.Created_at;
                    NewTimeSheetSummary.Employee_id = item.Employee_id;
                    NewTimeSheetSummary.Enitial = item.Enitial;
                    NewTimeSheetSummary.Timesheet_id = timesheetObj.Id;
                    NewTimeSheetSummary.Rate = item.Rate;
                    NewTimeSheetSummary.Enitial = item.Enitial;
                    NewTimeSheetSummary.Total_hours = item.Total_hours;
                    NewTimeSheetSummary.Created_at = DateTime.Now;
                    NewTimeSheetSummary.Updated_at = DateTime.Now;
                    NewTimeSheetSummary.Rating_by_client = 0;
                    //NewTimeSheetSummary.Updated_at = timeSheet_summary.Updated_at;

                    db.Timesheet_summaries.Add(NewTimeSheetSummary);
                    db.SaveChanges();
                }
                
                foreach (var item in timeSheet_DetailsList)
                {
                        item.Created_at = DateTime.Now;
                   
                    var NewTimeSheetDetailsObj = new Timesheet_details();
                    NewTimeSheetDetailsObj.Created_at = item.Created_at;
                    NewTimeSheetDetailsObj.Updated_at = item.Created_at;
                    NewTimeSheetDetailsObj.Day = item.Day;
                    NewTimeSheetDetailsObj.Employee_id = item.Employee_id;
                    NewTimeSheetDetailsObj.Hours = item.Hours;
                    NewTimeSheetDetailsObj.Timesheet_id = timesheetObj.Id;
                    db.Timesheet_details.Add(NewTimeSheetDetailsObj);
                    db.SaveChanges();
                }



                return Json("success", JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json("failure", JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize(Roles = "Admin")]
        public ActionResult UpdateCustomerInfo(int id)
        {
            var db = new TSS_Sql_Entities();
            ViewBag.Branch_id = new SelectList(db.Branches, "Id", "Name");
            return View(db.Customers.Find(id));
        }

        [Authorize(Roles = "Admin")]
        public ActionResult UpdateCustomerInfoFunc(Customer customer)
        {
            var db = new TSS_Sql_Entities();

            var customerExistingObj = db.Customers.Find(customer.Id);
            customerExistingObj.Email = customer.Email;
            customerExistingObj.PhoneNumber = customer.PhoneNumber;
            customerExistingObj.Branch_id = customer.Branch_id;
            db.SaveChanges();
            return RedirectToAction("AllClients");
        }

        [Authorize]
        public ActionResult TimeSheetDetails(int id)
        {
            var db = new TSS_Sql_Entities();
            TimeSheetTuple timeSheetDetailsTuple = new TimeSheetTuple();
            timeSheetDetailsTuple.TimeSheetGeneralDetails = db.Timesheets.Find(id);
            timeSheetDetailsTuple.TimeSheetSummary = db.Timesheet_summaries.Where(s=>s.Timesheet_id == id).ToList();
            var timeSheetDetailsList = db.Timesheet_details.Where(x => x.Timesheet_id == id).ToList();
            timeSheetDetailsTuple.TimeSheetDetails = timeSheetDetailsList;
            var userObject = db.AspNetUsers.FirstOrDefault(s => s.Customer_id == timeSheetDetailsTuple.TimeSheetGeneralDetails.Customer_Id_Generic);
            ViewBag.DisplayPicture = userObject.DisplayPicture;
            return View(timeSheetDetailsTuple);
        }

   


        [System.Web.Mvc.HttpGet]
        public JsonResult SearchEmployees(String query)
        {
            var db = new TSS_Sql_Entities();

            if (EmployeesStaticList == null)
            {
                EmployeesStaticList = db.Employees.ToList();
                
            }
            var splitStr = Regex.Split(query, " ");
            if (splitStr.Count() > 1)
            {
                splitStr[0] = splitStr[0].First().ToString().ToUpper() + splitStr[0].Substring(1);
                splitStr[1] = splitStr[1].First().ToString().ToUpper() + splitStr[1].Substring(1);
                var results = (from obj in EmployeesStaticList where obj.First_name.Contains(splitStr[0]) || obj.Last_name.Contains(splitStr[1]) select new { Id = obj.Id, Name = obj.First_name + " " + obj.Last_name }).Take(25).ToList();
                return Json(results, JsonRequestBehavior.AllowGet);
            }

            else
            {
                query = query.First().ToString().ToUpper() + query.Substring(1);
                var results = (from obj in EmployeesStaticList where obj.First_name.Contains(query) select new { Id = obj.Id, Name = obj.First_name + " " + obj.Last_name }).Take(25).ToList();
                return Json(results, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize(Roles = "Admin")]
        public ActionResult AllEmployees()
        {
            var db = new TSS_Sql_Entities();
            var employeesList = db.Employees.ToList();
            return View(employeesList);
        }


        public JsonResult SubmitTimeSheetDetails(Timesheet timesheet, List<Timesheet_summaries> timeSheet_summary, List<Timesheet_details> timeSheet_DetailsList)
        {

            try
            {
                timesheet.Created_at = DateTime.Now;

                var db = new TSS_Sql_Entities();
                var NewTimeSheet = db.Timesheets.FirstOrDefault(s=>s.Id == timesheet.Id);
                //NewTimeSheet.Created_at = timesheet.Created_at;
                //NewTimeSheet.Customer_id = timesheet.Customer_id;
                //NewTimeSheet.End_date = timesheet.End_date;
                //NewTimeSheet.For_internal_employee = timesheet.For_internal_employee;
                //NewTimeSheet.Note = timesheet.Note;
                //NewTimeSheet.Organization_id = timesheet.Organization_id;
                //NewTimeSheet.Po_number = timesheet.Po_number;
                //NewTimeSheet.Sent = timesheet.Sent;
                //NewTimeSheet.Signature = timesheet.Signature;
                //NewTimeSheet.Total_employees = timesheet.Total_employees;
                //NewTimeSheet.Total_hours = timesheet.Total_hours;
                NewTimeSheet.Updated_at = DateTime.Now;
                NewTimeSheet.Submit_by_client = true;
                NewTimeSheet.Sent = true;
                //var customer = db.Customers.FirstOrDefault(s => s.Id == timesheet.Customer_id);
                //if (customer != null)
                //    NewTimeSheet.Customer_Id_Generic = customer.Customer_id;
                NewTimeSheet.Status_id = 3;

               // db.Timesheets.Add(NewTimeSheet);
                db.SaveChanges();

                //var timesheetObj = db.Timesheets.FirstOrDefault(s => s.Created_at == timesheet.Created_at && s.Customer_id == timesheet.Customer_id);
                var timesheetObj = db.Timesheets.OrderByDescending(s => s.Created_at).FirstOrDefault(s => s.Customer_id == timesheet.Customer_id);

                foreach (var item in timeSheet_summary)
                {
                    //item.Created_at = DateTime.Now;
                    var NewTimeSheetSummary = db.Timesheet_summaries.FirstOrDefault(s=>s.Id == item.Id);
                    //NewTimeSheetSummary.Created_at = timeSheet_summary.Created_at;
                    //NewTimeSheetSummary.Employee_id = item.Employee_id;
                    NewTimeSheetSummary.Enitial = item.Enitial;
                    //NewTimeSheetSummary.Timesheet_id = timesheetObj.Id;
                    NewTimeSheetSummary.Rate = item.Rate;
                    NewTimeSheetSummary.Total_hours = item.Total_hours;
                    //NewTimeSheetSummary.Created_at = item.Created_at;
                    NewTimeSheetSummary.Updated_at = DateTime.Now;
                    NewTimeSheetSummary.Rating_by_client = item.Rating_by_client;
                    //NewTimeSheetSummary.Updated_at = timeSheet_summary.Updated_at;

                    //db.Timesheet_summaries.Add(NewTimeSheetSummary);
                    db.SaveChanges();
                }

                foreach (var item in timeSheet_DetailsList)
                {
                    //item.Created_at = DateTime.Now;

                    var NewTimeSheetDetailsObj = db.Timesheet_details.FirstOrDefault(s=>s.Id == item.Id);
                    //NewTimeSheetDetailsObj.Created_at = item.Created_at;
                    NewTimeSheetDetailsObj.Updated_at = item.Created_at;
                    //NewTimeSheetDetailsObj.Day = item.Day;
                    //NewTimeSheetDetailsObj.Employee_id = item.Employee_id;
                    NewTimeSheetDetailsObj.Hours = item.Hours;
                    //NewTimeSheetDetailsObj.Timesheet_id = timesheetObj.Id;
                    //db.Timesheet_details.Add(NewTimeSheetDetailsObj);
                    db.SaveChanges();
                }



                return Json("success", JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json("failure", JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize(Roles ="Admin")]
        public ActionResult UpdateEmployee(int id)
        {
            var db = new TSS_Sql_Entities();
            ViewBag.Branch_id = new SelectList(db.Branches, "Id", "Name");
            var employee = db.Employees.Find(id);
            return View(employee);
        }

        public ActionResult UpdateEmployeeEntry(Employee employee)
        {
            var db = new TSS_Sql_Entities();
            var employeeObj = db.Employees.Find(employee.Id);
            employeeObj.Address1 = employee.Address1;
            employeeObj.Address2 = employee.Address2;
            employeeObj.Anniversary_date = employee.Anniversary_date;
            employeeObj.Branch_id = employee.Branch_id;
            employeeObj.City = employee.City;
            employeeObj.Country = employee.Country;
            employeeObj.DateOfBirth = employee.DateOfBirth;
            employeeObj.EmployeeType = employee.EmployeeType;
            employeeObj.First_name = employee.First_name;
            employeeObj.Gender = employee.Gender;
            employeeObj.Hire = employee.Hire;
            employeeObj.Last_name = employee.Last_name;
            employeeObj.Marital_status = employee.Marital_status;
            employeeObj.Middle_name = employee.Middle_name;
            employeeObj.ReHire = employee.ReHire;
            employeeObj.State = employee.State;
            employeeObj.Status = employee.Status;
            employeeObj.Updated_at = DateTime.Now;
            employeeObj.ZipCode = employee.ZipCode;
            db.SaveChanges();
            return RedirectToAction("AllEmployees", "TSSManage");
        }


        [Authorize(Roles = "Admin")]
        public ActionResult SendAccountEmail(int id)
        {
            var db = new TSS_Sql_Entities();

            var ConfirmationToken = Membership.GeneratePassword(10, 0);
            ConfirmationToken = Regex.Replace(ConfirmationToken, @"[^a-zA-Z0-9]", m => "9");
            var savedContactObj = db.CustomerContacts.OrderByDescending(s => s.Id).FirstOrDefault(s => s.Id == id);
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
                string body = "<b>Hello " + savedContactObj.Email_id + "!</b><br />Someone has invited you to http://total-staffing.raisenit.com/, you can accept it through the link below.<br /> <a href='" + TSSLiveSiteURL + "/ClientDashboard/ConfirmAccount?token=" + ConfirmationToken + "'>Accept invitation</a><br /><small style='text-align:center;'>(If you don't want to accept the invitation, please ignore this email.Your account won't be created until you access the link above and set your password.<br/><b>This Link is active for next 24 hours, Please make sure to Enable your account before " + DateTime.Now.AddHours(24) + "</b>)</small>";

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

            var clientContactObj = db.CustomerContacts.FirstOrDefault(s => s.Id == id);
            var clientObject = db.Customers.FirstOrDefault(s =>s.Customer_id == clientContactObj.Customer_id);
            return RedirectToAction("ClientDetails", "TSSManage", new { clientObject.Id });
        }


        public ActionResult TimeSheetSuccessSubmit()
        {
            if(User.IsInRole("Admin"))
            {
                return RedirectToAction("Dashboard", "TSSManage");
            }
            else
            {
                return RedirectToAction("AllTimeSheets", "ClientDashboard");
            }
        }

        public ActionResult EditTimeSheet(int id)
        {
            var db = new TSS_Sql_Entities();
            TimeSheetTuple timeSheetDetailsTuple = new TimeSheetTuple();
            timeSheetDetailsTuple.TimeSheetGeneralDetails = db.Timesheets.Find(id);
            timeSheetDetailsTuple.TimeSheetSummary = db.Timesheet_summaries.Where(s => s.Timesheet_id == id).ToList();
            var timeSheetDetailsList = db.Timesheet_details.Where(x => x.Timesheet_id == id).ToList();
            timeSheetDetailsTuple.TimeSheetDetails = timeSheetDetailsList;
            var userObject = db.AspNetUsers.FirstOrDefault(s => s.Customer_id == timeSheetDetailsTuple.TimeSheetGeneralDetails.Customer_Id_Generic);
            ViewBag.DisplayPicture = userObject.DisplayPicture;
            return View(timeSheetDetailsTuple);
        }

    }
}