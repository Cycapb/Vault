using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using VaultDAL.Concrete;
using VaultDAL.Models;
using VaultService.Models.Mail;

namespace VaultService.Models
{
    public class ServiceHelper
    {
        
        public async Task StartNotification(DateTime date)
        {
            var notifications = await GetNotificationModels();
            if (notifications == null)
            {
                return;
            }
            await SendLogs(notifications, date);
        }

        private async Task<List<NotificationModel>> GetNotificationModels()
        {
            var helper = new VaultHelper(new MongoRepository<UserVault>(new MongoConnectionProvider()));
            var identityUsers = new IdentityHelper().Users;
            var vaultAdmins = (await identityUsers.FindAsync(x => x.Roles.Contains("VaultAdmins")))?.ToList();
            if (vaultAdmins == null)
            {
                return null;
            }
            var notificationModels = new List<NotificationModel>();
            foreach (var admin in vaultAdmins)
            {
                var userVaults = (await helper.GetVaults())?.ToList().Where(x => x.VaultAdmin.Id == admin.Id).ToList();
                notificationModels.Add(new NotificationModel()
                {
                    VaultAdminId = admin.Id,
                    Email = admin.Email,
                    Vaults = userVaults
                });
            }
            return notificationModels;
        }

        private async Task SendLogs(List<NotificationModel> users, DateTime date)
        {
            var logManager = new LogManager(new MongoRepository<VaultAccessLog>(new MongoConnectionProvider()));
            var logger = new FileLogger();
            foreach (var user in users)
            {
                var fileName = "";
                if (user.Vaults == null)
                {
                    continue;
                }
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
