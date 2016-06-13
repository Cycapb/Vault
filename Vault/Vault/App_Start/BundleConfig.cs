using System.Web.Optimization;

namespace Vault
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/datetime").Include(
                "~/Scripts/jquery-2.2.3.js",
                "~/Scripts/moment-with-locales.js",
                "~/Scripts/bootstrap-datetimepicker.js"));
            bundles.Add(new ScriptBundle("~/bundles/datetime-without-jquery").Include(
                "~/Scripts/moment-with-locales.js",
                "~/Scripts/bootstrap-datetimepicker.js"));
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                "~/Scripts/jquery-2.2.3.js"));
            bundles.Add(new ScriptBundle("~/bundles/validation").Include(
                "~/Scripts/jquery-2.2.3.js",
                "~/Scripts/jquery.validate.js",
                "~/Scripts/jquery.validate.unobtrusive.js"));
            bundles.Add(new ScriptBundle("~/bundles/validation-without-jquery").Include(
                "~/Scripts/jquery.validate.js",
                "~/Scripts/jquery.validate.unobtrusive.js"));
            bundles.Add(new ScriptBundle("~/bundles/jquery-ajax").Include(
                "~/Scripts/jquery-2.2.3.js",
                "~/Scripts/jquery.unobtrusive-ajax.js"));
            bundles.Add(new ScriptBundle("~/bundles/jquery-ajax-without-jquery").Include(
                "~/Scripts/jquery.unobtrusive-ajax.js"));

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
        }
    }
}