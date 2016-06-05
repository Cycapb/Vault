using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using ILogger = Vault.Abstract.ILogger;

namespace Vault.Concrete
{
    public class FileLogger:ILogger
    {
        private readonly ExceptionContext _exceptionContext;

        public FileLogger(ExceptionContext exceptionContext)
        {
            _exceptionContext = exceptionContext;
        }

        public async Task Log(string message)
        {
            await SaveToLogFile(message).ConfigureAwait(false);
        }

        private async Task SaveToLogFile(string error)
        {
            StringBuilder errorMessage = new StringBuilder();
            errorMessage.AppendLine("");
            errorMessage.AppendLine($"Date and time: {DateTime.Now}");
            errorMessage.AppendLine($"User: {_exceptionContext.HttpContext.User.Identity.Name}");
            errorMessage.AppendLine($"Controller: {_exceptionContext.RouteData.Values["controller"]}; Action: {_exceptionContext.RouteData.Values["action"]}");
            errorMessage.AppendLine($"Error: {error}");
            errorMessage.AppendLine($"Stack trace: {_exceptionContext.Exception.StackTrace}");

            await CreateLogDir().ConfigureAwait(false);

            using (Stream stream = new FileStream(@"C:\Programs\Logs\VaultLog.txt", FileMode.Append))
            {
                using (TextWriter writer = new StreamWriter(stream))
                {
                    await writer.WriteAsync(errorMessage.ToString()).ConfigureAwait(false);
                }
            }
        }

        private Task CreateLogDir()
        {
            return Task.Run(() =>
            {
                if (!Directory.Exists(@"C:\Programs\Logs"))
                {
                    Directory.CreateDirectory(@"C:\Programs\Logs");
                }
            });
        }
    }
}