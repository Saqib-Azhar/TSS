using Hangfire;
using Microsoft.Owin;
using Owin;
using TotalStaffingSolutions.Controllers;

[assembly: OwinStartupAttribute(typeof(TotalStaffingSolutions.Startup))]
namespace TotalStaffingSolutions
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            //GlobalConfiguration.Configuration.UseSqlServerStorage("DefaultConnection");
            //app.UseHangfireDashboard();
            //HomeController homeController = new Controllers.HomeController();
            //RecurringJob.AddOrUpdate(() => homeController.UpdateCustomerContacts(), Cron.Daily(0, 0));
            //RecurringJob.AddOrUpdate(() => homeController.UpdateCustomers(), Cron.Daily(0, 0));
            //RecurringJob.AddOrUpdate(() => homeController.UpdateEmployees(), Cron.Daily(0, 0));
            //app.UseHangfireServer();
        }
    }
}
