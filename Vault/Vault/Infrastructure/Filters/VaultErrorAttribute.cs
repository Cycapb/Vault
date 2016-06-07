using System;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Vault.Concrete;

namespace Vault.Infrastructure.Filters
{
    public class VaultErrorAttribute:FilterAttribute,IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            var logger = new FileLogger(filterContext);

            StringBuilder errorMessage = new StringBuilder();
            errorMessage.AppendLine("");
            errorMessage.AppendLine($"Date and time: {DateTime.Now}");
            errorMessage.AppendLine($"User: {filterContext.HttpContext.User.Identity.Name}");
            errorMessage.AppendLine($"Controller: {filterContext.RouteData.Values["controller"]}; Action: {filterContext.RouteData.Values["action"]}");
            errorMessage.AppendLine($"Error: {filterContext.Exception.Message}");
            errorMessage.AppendLine($"Stack trace: {filterContext.Exception.StackTrace}");

            Task.Run(() => logger.Log(errorMessage.ToString()));
            filterContext.Result = new ViewResult() {ViewName = "ErrorPage"};
            filterContext.ExceptionHandled = true;
        }
    }
}