using System.Web.Optimization;

namespace Sampoerna.EMS.Website
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            
            bundles.Add(new Bundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                         "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/modernizrmin").Include(
                        "~/Scripts/modernizr.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.min.js",
                      "~/Scripts/js/vendors/bootstrap-datepicker.min.js",
                       "~/Scripts/js/vendors/bootstrap-select-custom.js"
                      ));

            bundles.Add(new ScriptBundle("~/bundles/customjs").Include(
                      "~/Scripts/js/scripts.min.js",
                      "~/Scripts/js/custom.js"
                      //,"~/Scripts/js/_scripts.js"
                      ));

            bundles.Add(new ScriptBundle("~/bundles/fixedheadertable").Include(
                      "~/Scripts/js/vendors/jquery.fixedheadertable.min.js"
                      ));

            bundles.Add(new StyleBundle("~/bundles/css").Include(
                      "~/Content/css/main.min.css",
                      "~/Content/css/vendors/yamm.css",
                      "~/Content/css/vendors/font-awesome.min.css"
                      ));

            bundles.Add(new StyleBundle("~/bundles/jqueryui/autocomplete/css").Include(
                        "~/Content/themes/base/core.css",
                        "~/Content/themes/base/autocomplete.css")
                        );
            bundles.Add(new ScriptBundle("~/bundles/validate").Include(
                       "~/Scripts/jquery.validate.min.js",
                       "~/Scripts/jquery.validate.unobtrusive.min.js"
                       ));
            bundles.Add(new ScriptBundle("~/bundles/datatables").Include(
                      "~/Scripts/DataTables-1.10.7/media/js/jquery.dataTables.js"
                    
                      ));
            BundleTable.EnableOptimizations = false;
        }
    }
}
