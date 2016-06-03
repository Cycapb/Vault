using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Vault.Abstract;
using VaultDAL.Abstract;
using VaultDAL.Models;

namespace Vault.Concrete
{
    public class VaultHelper:IVaultHelper
    {
        private readonly IRepository<UserVault> _userVaultRepository;

        public VaultHelper(IRepository<UserVault> repository)
        {
            _userVaultRepository = repository;
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
            //var allowRead = vault.AllowRead.ToList();
            //var allowCreate = vault.AllowCreate.ToList();
            //var users = allowRead.Except(allowCreate).ToList();
            return vault.AllowCreate.Except(vault.AllowRead).ToList();
        }
    }
}