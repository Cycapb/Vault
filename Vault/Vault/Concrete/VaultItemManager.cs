using System;
using System.Linq;
using System.Threading.Tasks;
using Vault.Abstract;
using VaultDAL.Abstract;
using VaultDAL.Models;

namespace Vault.Concrete
{
    public class VaultItemManager:IVaultItemManager
    {
        private readonly IRepository<VaultItem> _vaultItemRepository;
        private readonly IRepository<UserVault> _userVaultRepository; 

        public VaultItemManager(IRepository<VaultItem> repository, IRepository<UserVault> userVaultRepository)
        {
            _vaultItemRepository = repository;
            _userVaultRepository = userVaultRepository;
        }

        public async Task<VaultItem> CreateAsync(VaultItem item)
        {
            return await _vaultItemRepository.CreateAsync(item);
        }

        public async Task DeleteAsync(string id, string itemId)
        {
            var vault = await _userVaultRepository.GetItemAsync(id);
            var item = vault.VaultItems.SingleOrDefault(x => x == itemId);
            vault.VaultItems.Remove(item);
            await _vaultItemRepository.DeleteAsync(itemId);
            await _userVaultRepository.UpdateAsync(vault);
        }

        public Task<VaultItem> GetItemAsync(string id)
        {
            return _vaultItemRepository.GetItemAsync(id);
        }

        public async Task UpdateAsync(VaultItem item)
        {
            await _vaultItemRepository.UpdateAsync(item);
        }
    }
}