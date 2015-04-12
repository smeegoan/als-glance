using System.Web;
using System.Web.Optimization;

namespace ALS.Glance.Web
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

            bundles.Add(new ScriptBundle("~/bundles/dc").Include(
                      "~/Scripts/CrossFilter/crossfilter.js",
                      "~/Scripts/d3.v3.js",
                      "~/Scripts/DC/dc.js",
                      "~/Scripts/apiclient.js",
                      "~/Scripts/regression.js",
                      "~/Scripts/moment.js",
                      "~/Scripts/colorbrewer.js",
                      "~/Scripts/alsglance.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/datatable").Include(
                      "~/Scripts/jquery.dataTables.js",
                      "~/Scripts/dataTables.bootstrap.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/dc.css",
                "~/Content/bootstrap.css",
                "~/Content/site.css"));
        }
    }
}
