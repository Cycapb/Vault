using System.Collections.Generic;
using System.Threading.Tasks;
using Vault.Models;
using VaultDAL.Models;

namespace Vault.Abstract
{
    public interface IVaultGetter
    {
        Task<IEnumerable<UserVault>> GetAsync(WebUser user);
        Task<IEnumerable<UserVault>> GetAllVaultsAsync();    
    }
}
