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
            Task.Run(() => logger.Log(filterContext.Exception.Message));
            filterContext.Result = new ViewResult() {ViewName = "ErrorPage"};
            filterContext.ExceptionHandled = true;
        }
    }
}