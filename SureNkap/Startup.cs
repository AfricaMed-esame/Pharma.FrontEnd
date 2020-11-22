using System;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.Owin;
using Owin;
using SureNkap.Models;

[assembly: OwinStartup(typeof(SureNkap.Startup))]

namespace SureNkap
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Pour plus d'informations sur la configuration de votre application, visitez https://go.microsoft.com/fwlink/?LinkID=316888
            app.MapSignalR();
            GlobalConfiguration.Configuration.UseSqlServerStorage("DefaultConnection");
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new AuthorizationFilter() }
            });
            app.UseHangfireServer();
        }
    }
}
