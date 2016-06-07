using System.Web.Optimization;

namespace Vault
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/datetimepicker").Include(
                "~/Scripts/jquery-{version}.js",
                "~/Scripts/moment-with-locales.js",
                "~/Scripts/bootstrap-datetimepicker.js"));
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                "~/Scripts/jquery-{version}.js"));

            bundles.Add(new StyleBundle("~/bundles/bootstrap/css").Include(
                "~/Content/bootstrap.css",
                "~/Content/bootstrap-theme.css"));
            bundles.Add(new StyleBundle("~/bundles/datetimepicker/css").Include(
                "~/Content/bootstrap.css",
                "~/Content/bootstrap-theme.css",
                "~/Content/bootstrap-datetimepicker.css"));
            bundles.Add(new StyleBundle("~/bundles/sitestyles/css").Include(
                "~/Content/ErrorStyles.css",
                "~/Content/Site.css"));

            bundles.Add(new StyleBundle("~/bundles/bootstrap-datetimepicker/css").Include(
                "~/Content/bootstrap.css",
                "~/Content/bootstrap-theme.css",
                "~/Content/bootstrap-datetimepicker.css",
                "~/Content/ErrorStyles.css")
                );
        }
    }
}