using System;
using System.Configuration;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Vault.Concrete;
using Vault.Models;

namespace Vault.Infrastructure.Filters
{
    public class VaultErrorAttribute:FilterAttribute,IExceptionFilter
    {
        private Vault.Abstract.ILogger _logger;

        public void OnException(ExceptionContext filterContext)
        {
            _logger = new FileLogger(filterContext);

            StringBuilder errorMessage = new StringBuilder();
            errorMessage.AppendLine("");
            errorMessage.AppendLine($"Date and time: {DateTime.Now}");
            errorMessage.AppendLine($"User: {filterContext.HttpContext.User.Identity.Name}");
            errorMessage.AppendLine($"Controller: {filterContext.RouteData.Values["controller"]}; Action: {filterContext.RouteData.Values["action"]}");
            errorMessage.AppendLine($"Error: {filterContext.Exception.Message}");
            errorMessage.AppendLine($"Stack trace: {filterContext.Exception.StackTrace}");

            Task.Run(() => _logger.Log(errorMessage.ToString()));
            Task.Run(() => SendByEmail(errorMessage.ToString()));
            filterContext.Result = new ViewResult() {ViewName = "ErrorPage"};
            filterContext.ExceptionHandled = true;
        }

        private async void SendByEmail(string message)
        {
            var mailSettings = new EmailSettings();
            var mailer = new MailReporter(mailSettings,_logger)
            {
                MailTo = ConfigurationManager.AppSettings["AdminMail"]
            };
            await mailer.Report(message);
        }
    }
}