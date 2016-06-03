using System.Collections.Generic;
using System.Threading.Tasks;
using VaultDAL.Models;

namespace Vault.Abstract
{
    public interface IVaultManager
    {
        Task<IEnumerable<UserVault>> GetVaults(string userId);
        Task<UserVault> GetVault(string id);
        Task<UserVault> CreateAsync(UserVault vault);
        Task DeleteAsync(string id);
        Task UpdateAsync(UserVault vault);
        Task<IEnumerable<VaultUser>> GetReadUsers(string id);
        Task<IEnumerable<VaultUser>> GetCreateUsers(string id);
        IEnumerable<VaultUser> GetAllUsers(string id);
        Task DeleteUserAsync(string userId, string vaultId);
    }
}
