using System.Threading.Tasks;
using VaultDAL.Models;

namespace Vault.Abstract
{
    public interface IVaultItemManager
    {
        Task<VaultItem> CreateAsync(VaultItem item);
        Task DeleteAsync(string id, string itemId);
        Task UpdateAsync(VaultItem item);

        Task<VaultItem> GetItemAsync(string id);
    }
}
