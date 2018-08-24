using System.Web;
using System.Web.Optimization;

namespace DeaconCCGManagement
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                              "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.min.js",
                      "~/Scripts/respond.js"));

            // JS Storage is a plugin for storing trivial data in the browser.
            bundles.Add(new ScriptBundle("~/bundles/plugins").Include(
                     "~/Scripts/js.storage.min.js"));

            bundles.Add(new StyleBundle("~/Content/styles").Include(
                      "~/Content/bootstrap.min.css",
                      "~/Content/site.css",
                      "~/css/font-awesome.min.css"));

            //BundleTable.EnableOptimizations = true;

        }
    }
}
