using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vault.Abstract;
using Vault.Infrastructure;
using VaultDAL.Abstract;
using VaultDAL.Models;

namespace Vault.Concrete
{
    public class VaultManager:IVaultManager
    {
        private readonly IRepository<UserVault> _userVaultRepository;
        private readonly IRepository<VaultItem> _vaultItemRepository;
        private readonly ILogManager<VaultAccessLog> _logManager;

        public VaultManager(IRepository<UserVault> repository, 
            IRepository<VaultItem> vaultItemRepository,
            ILogManager<VaultAccessLog> logManager)
        {
            _userVaultRepository = repository;
            _vaultItemRepository = vaultItemRepository;
            _logManager = logManager;
        }

        public async Task<IEnumerable<UserVault>> GetVaults(string userId)
        {
            var vaults = await _userVaultRepository.GetListAsync();
            return vaults?.Where(x => x.VaultAdmin.Id == userId).ToList();
        }

        public async Task<UserVault> GetVault(string id)
        {
            return await _userVaultRepository.GetItemAsync(id);
        }

        public async Task<UserVault> CreateAsync(UserVault vault)
        {
            return await _userVaultRepository.CreateAsync(vault);
        }

        public async Task DeleteAsync(string id)
        {
            await _userVaultRepository.DeleteAsync(id);
        }

        public async Task UpdateAsync(UserVault vault)
        {
            await _userVaultRepository.UpdateAsync(vault);
        }

        public async Task<IEnumerable<VaultUser>> GetReadUsers(string id)
        {
            var vault = await _userVaultRepository.GetItemAsync(id);
            return vault?.AllowRead?.ToList();
        }

        public async Task<IEnumerable<VaultUser>> GetCreateUsers(string id)
        {
            var vault = await _userVaultRepository.GetItemAsync(id);
            return vault?.AllowCreate?.ToList();
        }

        public IEnumerable<VaultUser> GetAllUsers(string id)
        {
            var vault = _userVaultRepository.GetItem(id);
            if (vault.AllowCreate != null)
            {
                return vault.AllowRead?.Union(vault.AllowCreate).Distinct(new VaultUserEqualityComparer()).ToList();
            }
            else
            {
                return vault.AllowRead?.ToList();
            }
        }

        public async Task<IEnumerable<VaultItem>> GetAllItems(string id)
        {
            var vault = await _userVaultRepository.GetItemAsync(id);
            var items = new List<VaultItem>();
            if (vault.VaultItems != null)
            {
                items = (await _vaultItemRepository.GetListAsync())?.Where(x => vault.VaultItems.Contains(x.Id)).ToList();
            }
            return items;
        }

        public async Task DeleteItemAsync(string vaultId, string itemId)
        {
            var vault = await _userVaultRepository.GetItemAsync(vaultId);
            var item = vault.VaultItems.SingleOrDefault(x => x == itemId);
            vault.VaultItems.Remove(item);
            await _vaultItemRepository.DeleteAsync(itemId);
            await _userVaultRepository.UpdateAsync(vault);
        }

        public async Task<VaultUser> GetVaultAdmin(string vaultId)
        {
            var vault = await _userVaultRepository.GetItemAsync(vaultId);
            return vault.VaultAdmin;
        }

        public async Task DeleteVaultsByUser(string userId)
        {
            var vaults = (await _userVaultRepository.GetListAsync())?
                .Where(vault => vault.VaultAdmin.Id == userId)
                .ToList();

            if (vaults != null)
            {
                foreach (var vault in vaults)
                {
                    await _userVaultRepository.DeleteAsync(vault.Id);
                }
            }
        }
    }
}