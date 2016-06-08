using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vault.Infrastructure;
using VaultDAL.Models;

namespace VaultService.Models
{
    public class ServiceHelper
    {
        public async void StartNotification()
        {
            var vaults = (await new VaultHelper().GetVaults());
            if (vaults == null)
            {
                return;
            }
            var vaultAdmins = vaults.Select(vault => vault.VaultAdmin).Distinct(new VaultUserEqualityComparer()).ToList();
            var 
        }
    }
}
