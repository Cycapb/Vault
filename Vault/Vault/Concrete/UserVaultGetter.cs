using System.Collections.Generic;
using System.Linq;
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

        public IEnumerable<UserVault> GetUserVaults(WebUser user)
        {
            var userVaults = new List<UserVault>();

                var vaults = _vaultRepository.GetList();
                foreach (var vault in vaults)
                {
                    var allowCreate = vault.AllowCreate?.ToList();
                    var allowRead = vault.AllowRead?.ToList();
                    if (allowCreate != null)
                    {
                        userVaults.AddRange(from vaultUser in allowCreate where vaultUser.Id == user.Id select vault);
                    }
                    if (allowRead != null)
                    {
                        userVaults.AddRange(from vaultUser in allowRead where vaultUser.Id == user.Id select vault);
                    }
                }
            return userVaults.Distinct();
        }

        public IEnumerable<UserVault> GetAllVaults(WebUser user)
        {
            var vaults = _vaultRepository.GetList();
            var freeVaults = new List<UserVault>();
            foreach (var vault in vaults)
            {
                var allowCreate = vault.AllowCreate?.ToList();
                var allowRead = vault.AllowRead?.ToList();
                if (allowCreate != null)
                {
                    if (vault.AllowCreate.All(x => x.Id != user.Id))
                    {
                        freeVaults.Add(vault);
                    }
                }
                if (allowRead != null)
                {
                    if (vault.AllowRead.All(x => x.Id != user.Id))
                    {
                        freeVaults.Add(vault);
                    }
                }
            }
            return freeVaults.Distinct();
        }
    }
}