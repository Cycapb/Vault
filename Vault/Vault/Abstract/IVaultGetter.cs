using System.Collections.Generic;
using Vault.Models;
using VaultDAL.Models;

namespace Vault.Abstract    
{
    public interface IVaultGetter
    {
        IEnumerable<UserVault> GetUserVaults(WebUser user);
        IEnumerable<UserVault> GetAllVaults(WebUser user);    
    }
}
