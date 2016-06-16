using System.Collections.Generic;
using VaultDAL.Models;

namespace VaultService.Models
{
    public class NotificationModel
    {
        public string VaultAdminId { get; set; }
        public string Email { get; set; }
        public IEnumerable<UserVault> Vaults { get; set; }
    }
}
