using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vault.Abstract;
using Vault.Models;
using VaultDAL.Abstract;
using VaultDAL.Models;

namespace Vault.Concrete
{
    public class UserVaultGetter:IVaultGetter
    {
        private readonly IRepository<UserVault> _vaultRepository;

        public UserVaultGetter(IRepository<UserVault> vaultRepository)
        {
            _vaultRepository = vaultRepository;
        }

        public async Task<IEnumerable<UserVault>> GetAsync(WebUser user)
        {
            var vaults = await _vaultRepository.GetListAsync();
            var userVaults = new List<UserVault>();
            foreach (var vault in vaults)
            {
                var vaultusers = vault.AllowCreate.ToList();
                userVaults.AddRange(from vaultUser in vaultusers where vaultUser.Id == user.Id select vault);
            }
            foreach (var vault in vaults)
            {
                var vaultusers = vault.AllowRead.ToList();
                userVaults.AddRange(from vaultUser in vaultusers where vaultUser.Id == user.Id select vault);
            }
            return userVaults.Distinct();
        }

        public Task<IEnumerable<UserVault>> GetAllVaultsAsync()
        {
            return null;
        }
    }
}