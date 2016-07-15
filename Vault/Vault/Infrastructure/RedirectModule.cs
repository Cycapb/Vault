using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Vault.Infrastructure
{
    public class RedirectModule:IHttpModule
    {
        public void Init(HttpApplication context)
        {
            context.MapRequestHandler += Handler;
        }

        private void Handler(object src, EventArgs args)
        {
            RouteValueDictionary rvd = HttpContext.Current.Request.RequestContext.RouteData.Values;
            if (Compare(rvd,"controller","Vault") && Compare(rvd,"action","Index"))
            {
                var url = UrlHelper.GenerateUrl("", "Index", "Home", rvd, RouteTable.Routes,
                    HttpContext.Current.Request.RequestContext, false);
                HttpContext.Current.Response.Redirect(url);
            }
        }

        private bool Compare(RouteValueDictionary rvd, string key, string value)
        {
            return string.Equals((string) rvd[key], value, StringComparison.OrdinalIgnoreCase);
        }

        public void Dispose()
        {
            
        }
    }
}