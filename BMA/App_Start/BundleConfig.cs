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

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));
            bundles.Add(new StyleBundle("~/View/Layout/Css").Include(
                     "~/Content/CustomerLayout/CSS/animate.css",
                     "~/Content/CustomerLayout/CSS/bootstrap.css",
                     "~/Content/CustomerLayout/CSS/owl.carousel.css",
                     "~/Content/CustomerLayout/CSS/photobox.css",
                     "~/Content/CustomerLayout/CSS/responsive.css",
                     "~/Content/CustomerLayout/CSS/revolution-slider.css",
                     "~/Content/CustomerLayout/CSS/style.css",
                     "~/Content/font-awesome-4.3.0/css/font-awesome.min.css")); // font-awesome.min.css with font-awesome.css is the same but font-awesome.min.css have smaill size
            bundles.Add(new ScriptBundle("~/View/Layout/Js").Include(
                      "~/Content/CustomerLayout/JS/jquery.js",
                      "~/Content/CustomerLayout/JS/bootstrap.js",
                      "~/Content/CustomerLayout/JS/jquery-ui.js",
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
