using System.Collections.Generic;
using System.Threading.Tasks;
using VaultDAL.Models;

namespace Vault.Abstract
{
    public interface IVaultItemManager
    {
        Task<VaultItem> CreateAsync(VaultItem item);
        Task DeleteAsync(string id);
        Task UpdateAsync(VaultItem item);
    }
}
