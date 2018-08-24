using System.Data.Entity;
using DeaconCCGManagement.DAL;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DeaconCCGManagement.Startup))]
namespace DeaconCCGManagement
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // No longer needed. We're using migration now.
            //
            // Set up the initializer to run based on the dependency
            //
            //Database.SetInitializer(new CcgDbContextInitializer());

            ConfigureAuth(app);
            app.MapSignalR();
        }
    }
}
