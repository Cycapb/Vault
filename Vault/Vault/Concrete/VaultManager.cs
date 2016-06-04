using System;
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

        public VaultManager(IRepository<UserVault> repository, IRepository<VaultItem> vaultItemRepository)
        {
            _userVaultRepository = repository;
            _vaultItemRepository = vaultItemRepository;
        }

        public async Task<IEnumerable<UserVault>> GetVaults(string userId)
        {
            var vaults = await _userVaultRepository.GetListAsync();
            return vaults.Where(x => x.VaultAdmin.Id == userId).ToList();
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
            return vault.AllowRead?.ToList();
        }

        public async Task<IEnumerable<VaultUser>> GetCreateUsers(string id)
        {
            var vault = await _userVaultRepository.GetItemAsync(id);
            return vault.AllowCreate?.ToList();
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

        public async Task<string> GetUserAccess(string vaultId, string userId)
        {
            var vault = await _userVaultRepository.GetItemAsync(vaultId);
            if (vault.VaultAdmin.Id == userId){ return "Create"; }
            if (vault.AllowCreate != null)
            {
                if (vault.AllowCreate.Any(x => x.Id == userId))
                {
                    return "Create";
                }
                else
                {
                    if (vault.AllowRead != null)
                    {
                        if (vault.AllowRead.Any(x => x.Id == userId))
                        {
                            return "Read";
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            else
            {
                if (vault.AllowRead != null)
                {
                    if (vault.AllowRead.Any(x => x.Id == userId))
                    {
                        return "Read";
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        public async Task DeleteItemAsync(string vaultId, string itemId)
        {
            var vault = await _userVaultRepository.GetItemAsync(vaultId);
            var item = vault.VaultItems.SingleOrDefault(x => x == itemId);
            vault.VaultItems.Remove(item);
            await _vaultItemRepository.DeleteAsync(itemId);
            await _userVaultRepository.UpdateAsync(vault);
        }
    }
}