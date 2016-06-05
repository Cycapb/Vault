using System.Web.Mvc;
using System.Web.Routing;
using Vault.Infrastructure.Binders;
using Vault.Models;

namespace Vault
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            ModelBinders.Binders.Add(typeof(WebUser),new WebUserModelBinder());
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
        }
    }
}
