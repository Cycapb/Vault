using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using Vault.Infrastructure;
using VaultService.Models.Mail;

namespace VaultService.Models
{
    public class ServiceHelper
    {
        
        public async Task StartNotification(DateTime date)
        {
            var notifications = await GetNotificationModels();
            await SendLogs(notifications, date);
        }

        private async Task<List<NotificationModel>> GetNotificationModels()
        {
            var helper = new VaultHelper();
            var vaults = (await helper.GetVaults())?.ToList();
            if (vaults == null)
            {
                return null;
            }
            var vaultAdmins = vaults.Select(vault => vault.VaultAdmin).Distinct(new VaultUserEqualityComparer()).ToList();
            var identUsers = await new IdentityHelper().Users.Find(x => true).ToListAsync();
            var notificationModels = new List<NotificationModel>();
            foreach (var admin in vaultAdmins)
            {
                foreach (var user in identUsers)
                {
                    if (admin.Id == user.Id)
                    {
                        var userVaults = vaults.Where(x => x.VaultAdmin.Id == admin.Id).ToList();
                        notificationModels.Add(new NotificationModel() { VaultAdminId = user.Id, Email = user.Email, Vaults = userVaults });
                    }
                }
            }
            return notificationModels;
        }

        private async Task SendLogs(List<NotificationModel> users, DateTime date)
        {
            var logManager = new LogManager();
            var logger = new FileLogger();
            foreach (var user in users)
            {
                var fileName = "";
                foreach (var vault in user.Vaults)
                {
                    var logItems = await logManager.ShowByDateLog(vault.Id, date);
                    fileName = await logger.Log(logItems, user.VaultAdminId, vault.Name);
                }
                var mailer = new MailReporter()
                {
                    MailTo = user.Email
                };
                await mailer.Report(fileName);
            }
        }
    }
}
