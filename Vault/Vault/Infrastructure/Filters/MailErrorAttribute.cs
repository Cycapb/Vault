using System.Threading.Tasks;
using System.Web.Mvc;
using VaultServices.Concrete;

namespace Vault.Infrastructure.Filters
{
    public class MailErrorAttribute:FilterAttribute,IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            var logger = new FileLogger(filterContext);
            Task.Run(() => logger.Log(filterContext.Exception.Message));
            filterContext.ExceptionHandled = true;
        }
    }
}