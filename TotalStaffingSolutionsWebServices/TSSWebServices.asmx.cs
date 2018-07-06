using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using TotalStaffingSolutionsWebServices.Model;

namespace TotalStaffingSolutionsWebServices
{
    /// <summary>
    /// Summary description for TSSWebServices
    /// </summary>
    [WebService(Namespace = "http://www.totalstaffingsolutions.com/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class TSSWebServices : System.Web.Services.WebService
    {
       // private System.Windows.Forms.DataGridView dataGridView1;

        [WebMethod]
        public List<Cusmast> GetCustomersData()
        {
            var db = new ULTRAEntities();
            var custList = db.Cusmasts.ToList();
            List<Cusmast> customerList = new List<Cusmast>();
            foreach (var item in custList)
            {
                var userobj = new Cusmast();
                userobj.Cucname = item.Cucname;
                userobj.Cuccustid = item.Cuccustid;
                userobj.Cucaddr1 = item.Cucaddr1;
                userobj.Cucaddr2 = item.Cucaddr2;
                userobj.Cuccity = item.Cuccity;
                userobj.Cucstate = item.Cucstate;
                //userobj.CustomerAdded = DateTime.Now;
                userobj.Cuczipcode = item.Cuczipcode;
                userobj.Cuccountry = item.Cuccountry;
                //userobj.Created_at = DateTime.Now;
                userobj.Cucpono = item.Cucpono;
                userobj.Cucstatus = item.Cucstatus;
                customerList.Add(userobj);
            }

            return customerList;
        }

        [WebMethod]
        public List<Customer_contact> GetCustomerContacts()
        {
            var db = new ULTRAEntities();
            var CusContList = db.Cusconts.ToList();
            List<Customer_contact> customersContactsList = new List<Customer_contact>();
            foreach (var item in CusContList)
            {
                Customer_contact customerContactObj = new Customer_contact();
                customerContactObj.Ccccontact = item.Ccccontact;
                customerContactObj.Ccccontcde = item.Ccccontcde;
                customerContactObj.Ccccontmsc = item.Ccccontmsc;
                customerContactObj.Ccccuscont = item.Ccccuscont;
                customerContactObj.Ccccusmast = item.Ccccusmast;
                customerContactObj.Cccemail = item.Cccemail;
                customerContactObj.Cccemlbody = item.Cccemlbody;
                customerContactObj.Cccfname = item.Cccfname;
                customerContactObj.Ccclname = item.Ccclname;
                customerContactObj.Cccmname = item.Cccmname;
                customerContactObj.Cccphone1 = item.Cccphone1;
                customerContactObj.Cccphone2 = item.Cccphone2;
                customerContactObj.Cccphone3 = item.Cccphone3;
                customerContactObj.Cccsalcde = item.Cccsalcde;
                customerContactObj.Ccmnotes = item.Ccmnotes;
                customerContactObj.Customer_id = item.Cusmast.Cuccustid;

                customersContactsList.Add(customerContactObj);
            }
            return customersContactsList;
        }

        [WebMethod]
        public List<Empmast> GetEmployeesData()
        {
            var db = new ULTRAEntities();
            var list2 = db.Empmasts.Where(s => 1 == 1);
            List<Empmast> list = new List<Empmast>();

            foreach (var item in list2.ToList())
            {
                Empmast employeeObj = new Empmast();

                employeeObj.Emcempid = item.Emcempid;
                employeeObj.Emcfname = item.Emcfname;
                employeeObj.Emclname = item.Emclname;
                employeeObj.Emcmname = item.Emcmname;
                //employeeObj.Created_at = DateTime.Now;
                employeeObj.Emcaddr1 = item.Emcaddr1;
                employeeObj.Emcaddr2 = item.Emcaddr2;
                employeeObj.Emccity = item.Emccity;
                employeeObj.Emcstate = item.Emcstate;
                employeeObj.Emczipcode = item.Emczipcode;
                employeeObj.Emccountry = item.Emccountry;
                employeeObj.Emcmarital = item.Emcmarital;
                employeeObj.Emcsex = item.Emcsex;
                employeeObj.Emtbirth = item.Emtbirth;
                employeeObj.Emthire = item.Emthire;
                employeeObj.Emtrehire = item.Emtrehire;
                employeeObj.Emcemptype = item.Emcemptype;
                //employeeObj.Updated_at = DateTime.Now;


                list.Add(employeeObj);
            }
            return list;
        }

    }
}
