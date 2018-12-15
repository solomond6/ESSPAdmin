using System.Web;
using System.Web.Optimization;

namespace ESPAdmin
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
                      "~/Scripts/vendors.bundle.js",
                      "~/Scripts/scripts.bundle.js",
                      "~/Scripts/jquery.dataTables.min.js",
                      "~/Scripts/dataTables.responsive.js",
                      "~/Scripts/jquery-confirm.min.js",
                      "~/Scripts/jquery-ui.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap2").Include(
                      "~/Scripts/bootstrap.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/vendors.bundle.css",
                      "~/Content/style.bundle.css",
                      "~/Content/jquery.dataTables.css",
                      "~/Content/jquery-ui.css",
                      "~/Content/bootstrap-datepicker.css",
                      "~/Content/jquery-confirm.min.css"));

            bundles.Add(new StyleBundle("~/Content/css2").Include(
                      "~/Content/bootstrap.css"));
        }
    }
}
