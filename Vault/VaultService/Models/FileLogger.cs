using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using VaultDAL.Models;

namespace VaultService.Models
{
    public class FileLogger
    {
        private const string FilePath = @"C:\Programs\Logs\Temp\";

        public async Task<string> Log(IEnumerable<VaultAccessLog> logItems, string userId, string vaultName)
        {
            await CreateLogDir().ConfigureAwait(false);
            return await SaveToLogFile(logItems, userId, vaultName).ConfigureAwait(false);
        }

        private async Task<string> SaveToLogFile(IEnumerable<VaultAccessLog> logItems, string userId, string vaultName)
        {
            var fileName = $"{userId}-{DateTime.Today.Date.ToShortDateString()}.txt";
            using (Stream stream = new FileStream(FilePath + fileName, FileMode.Append))
            {
                using (TextWriter writer = new StreamWriter(stream))
                {
                    var logMessage = new StringBuilder();
                    logMessage.Append("");
                    logMessage.Append($"Date: {DateTime.Today.AddDays(-1).ToShortDateString()}\r\n");
                    logMessage.Append($"Name of the vault: {vaultName}\r\n");
                    foreach (var item in logItems)
                    {
                        logMessage.Append($"{item.DateTime.ToShortTimeString()}: AccessType:{item.EventType} Event:{item.Event}\r\n");
                    }
                    await writer.WriteAsync(logMessage.ToString()).ConfigureAwait(false);
                }
                return FilePath + fileName;
            }
        }

        private Task CreateLogDir()
        {
            return Task.Run(() =>
            {
                if (!Directory.Exists(@"C:\Programs\Logs\Temp"))
                {
                    Directory.CreateDirectory(@"C:\Programs\Logs\Temp");
                }
            });
        }
    }
}
