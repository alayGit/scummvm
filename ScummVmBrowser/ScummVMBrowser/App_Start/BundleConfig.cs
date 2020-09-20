using System.Web;
using System.Web.Optimization;

namespace ScummVMBrowser
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/signalr").Include(
          "~/node_modules/signalr/jquery.signalR.js"));

            bundles.Add(new ScriptBundle("~/bundles/ender-js").Include(
         "~/node_modules/ender-js/ender.js"));

            //C:\ScummVMNew\ScummVMWeb\GIT\dists\ScummVMBrowser\node_modules\signalr-client\signalR.js

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Components/iceProxy").Include(
         "~/Scripts/Proxies/ScummWebServer.js"));

            bundles.Add(new StyleBundle("~/Content/react").Include(
                      "~/dist/Scripts/dist/bundle.js"));

			bundles.Add(new StyleBundle("~/Content/pako").Include(
				   "~/dist/Scripts/dist/pako_inflate.min.js"));

		}
    }
}
