using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vault.Abstract;
using Vault.Infrastructure;
using VaultDAL.Abstract;
using VaultDAL.Models;

namespace Vault.Concrete
{
    public class VaultHelper:IVaultHelper
    {
        private readonly IRepository<UserVault> _userVaultRepository;
        private readonly IAccessManager _accessManager;

        public VaultHelper(IRepository<UserVault> repository, IAccessManager accessManager)
        {
            _userVaultRepository = repository;
            _accessManager = accessManager;
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

        public async Task DeleteUserAsync(string userId, string vaultId)
        {
            var userToDel = new VaultUser() {Id = userId};
            await _accessManager.RevokeReadAccess(userToDel, vaultId);
            await _accessManager.RevokeCreateAccess(userToDel, vaultId);
        }


    }
}