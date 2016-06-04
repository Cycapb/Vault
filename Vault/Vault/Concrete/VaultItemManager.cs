using System.Threading.Tasks;
using Vault.Abstract;
using VaultDAL.Abstract;
using VaultDAL.Models;

namespace Vault.Concrete
{
    public class VaultItemManager:IVaultItemManager
    {
        private readonly IRepository<VaultItem> _vaultItemRepository;


        public VaultItemManager(IRepository<VaultItem> repository)
        {
            _vaultItemRepository = repository;
        }

        public async Task<VaultItem> CreateAsync(VaultItem item)
        {
            return await _vaultItemRepository.CreateAsync(item);
        }

        public async Task DeleteAsync(string id)
        {
            await _vaultItemRepository.DeleteAsync(id);
        }

        public async Task UpdateAsync(VaultItem item)
        {
            await _vaultItemRepository.UpdateAsync(item);
        }
    }
}