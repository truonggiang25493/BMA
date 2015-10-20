using System.Web;
using System.Web.Optimization;

namespace BMA
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
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));


            bundles.Add(new StyleBundle("~/AdminLTE/css").Include(
                "~/Content/AdminLayout/CSS/_all-skins.min.css",
                "~/Content/AdminLayout/CSS/AdminLTE.min.css",
                "~/Content/AdminLayout/CSS/bootstrap.min.css"));
            bundles.Add(new ScriptBundle("~/AdminLTE/js").Include(
                      "~/Content/AdminLayout/JS/app.js",
                      "~/Content/AdminLayout/JS/bootstrap.min.js",
                     "~/Content/AdminLayout/JS/demo.js",
                     "~/Content/AdminLayout/JS/fastclick.min.js",
                     "~/Content/AdminLayout/JS/jQuery-2.1.4.min.js",
                     "~/Content/AdminLayout/JS/jquery.dataTables.min.js",
                     "~/Content/AdminLayout/JS/jquery.slimscroll.min.js"));

            bundles.Add(new StyleBundle("~/CustomerCss").Include(
                     "~/Content/CustomerLayout/CSS/animate.css",
                     "~/Content/CustomerLayout/CSS/bootstrap.css",
                     "~/Content/CustomerLayout/CSS/owl.carousel.css",
                     "~/Content/CustomerLayout/CSS/photobox.css",
                     "~/Content/CustomerLayout/CSS/responsive.css",
                     "~/Content/CustomerLayout/CSS/revolution-slider.css",
                     "~/Content/CustomerLayout/CSS/style.css",
                     "~/Content/font-awesome-4.3.0/css/font-awesome.css",
                     "~/Content/font-awesome-4.3.0/css/font-awesome.min.css"));

            bundles.Add(new ScriptBundle("~/CustomerJs").Include(
                      "~/Content/CustomerLayout/JS/jquery.js",
                      "~/Content/CustomerLayout/JS/bootstrap.js",
                      "~/Content/CustomerLayout/JS/jquery-ui.js",
                      "~/Content/CustomerLayout/JS/bootstrap-datepicker.min.js",
                      "~/Content/CustomerLayout/JS/jquery.appear.js",
                      "~/Content/CustomerLayout/JS/jquery.mixitup.min.js",
                      "~/Content/CustomerLayout/JS/jquery.photobox.js",
                      "~/Content/CustomerLayout/JS/jquery.themepunch.revolution.js",
                     "~/Content/CustomerLayout/JS/jquery.themepunch.tools.min.js",
                     "~/Content/CustomerLayout/JS/owl.carousel.min.js",
                     "~/Content/CustomerLayout/JS/scripts.js"));
        }
    }
}
