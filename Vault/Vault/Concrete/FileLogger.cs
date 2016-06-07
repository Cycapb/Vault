using System.IO;
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
            await CreateLogDir().ConfigureAwait(false);

            using (Stream stream = new FileStream(@"C:\Programs\Logs\VaultLog.txt", FileMode.Append))
            {
                using (TextWriter writer = new StreamWriter(stream))
                {
                    await writer.WriteAsync(error).ConfigureAwait(false);
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