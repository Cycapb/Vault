using System.Web.Mvc;
using Vault.Concrete;

namespace Vault.Infrastructure.Filters
{
    public class VaultErrorAttribute:FilterAttribute,IExceptionFilter
    {
        public async void OnException(ExceptionContext filterContext)
        {
            var logger = new FileLogger(filterContext);
            await logger.Log(filterContext.Exception.Message);
            filterContext.Result = new ViewResult() {ViewName = "ErrorPage"};
            filterContext.ExceptionHandled = true;
        }
    }
}