using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using DeaconCCGManagement.Infrastructure;
using Elmah;
using Elmah.Contrib.EntityFramework;

namespace DeaconCCGManagement
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            // Init the AutoMapper
            // AutoMapper maps the domain (data) models to the
            // view models and vice versa. e.g., Member ==> MemberViewModel
            AutoMapperBootstrapper.Initialize();          

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //Adding for AD authentication support
            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimsIdentity.DefaultNameClaimType;
        }

        private bool _sendEmail;
        void ErrorLog_Filtering(object sender, ExceptionFilterEventArgs e)
        {

            //TODO read in xml log files
            //Elmah.XmlFileErrorLog log = new XmlFileErrorLog("~/ElmahLog");

            ElmahError err = new ElmahError();


            //using (var context = new ElmahContext())
            //{
            //    //if the exception had the same message and was from
            //    //the last 10 min it means it's the same we dismiss it
            //    _sendEmail = true;
            //    var lastErr = context.ELMAH_Errors
            //    .OrderByDescending(m => m.TimeUtc).Take(1)
            //    .SingleOrDefault();
            //    if (lastErr != null &&
            //    (e.Exception.Message == lastErr.Message &&
            //    lastErr.TimeUtc > DateTime.UtcNow.AddMinutes(-10)))
            //    {
            //        e.Dismiss();
            //        _sendEmail = false;
            //    }
            //}

            if (e.Exception.GetBaseException() is HttpRequestValidationException) e.Dismiss();
        }

        void ErrorMail_Filtering(object sender, ExceptionFilterEventArgs e)
        {
            if (e.Exception.GetBaseException() is HttpRequestValidationException) e.Dismiss();
        }

        void ErrorLog_Logged(object sender, ErrorLoggedEventArgs args)
        {
            //keep the log form the last 60 days and delete the rest
            //using (var context = new ElmahContext())
            //{
            //    var baseLineDate = DateTime.UtcNow.AddDays(-60);
            //    var model = context.ELMAH_Errors.Where(p => p.TimeUtc < baseLineDate);
            //    foreach (var item in model)
            //    {
            //        context.ELMAH_Errors.Remove(item);
            //    }
            //    context.SaveChanges();
            //}
        }
    }
}
