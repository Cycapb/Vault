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

        public IEnumerable<UserVault> Get(WebUser user)
        {
            var userVaults = new List<UserVault>();
            var awaiter = _vaultRepository.GetListAsync().GetAwaiter();
            awaiter.OnCompleted((() =>
            {
                var vaults = awaiter.GetResult();
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
            }));
            
            return userVaults.Distinct();
        }

        public IEnumerable<UserVault> GetAllVaults(WebUser user)
        {
            var awaiter = _vaultRepository.GetListAsync().GetAwaiter();
            var freeVaults = new List<UserVault>();
            awaiter.OnCompleted((() =>
            {
                var vaults = awaiter.GetResult();
                foreach (var vault in vaults)
                {
                    var allowCreate = vault.AllowCreate?.ToList();
                    var allowRead = vault.AllowRead?.ToList();
                    if (allowCreate != null)
                    {
                        freeVaults.AddRange(from vaultUser in allowCreate where vaultUser.Id != user.Id select vault);
                    }
                    if (allowRead != null)
                    {
                        freeVaults.AddRange(from vaultUser in allowRead where vaultUser.Id != user.Id select vault);
                    }
                }
            }));
            return freeVaults;
        }
    }
}