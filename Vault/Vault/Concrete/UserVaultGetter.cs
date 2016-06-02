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
            var vaults = _vaultRepository.GetList().Where(x=>x.VaultAdmin.Id != user.Id);
            foreach (var vault in vaults)
            {
                if (vault.AllowCreate != null)
                {
                    if (vault.AllowCreate.All(x => x.Id == user.Id))
                    {
                        userVaults.Add(vault);
                        continue;
                    }
                }
                if (vault.AllowRead != null)
                {
                    if (vault.AllowRead.All(x => x.Id == user.Id))
                    {
                        userVaults.Add(vault);
                    }
                }
            }
            return userVaults.Distinct();
        }

        public IEnumerable<UserVault> GetAllVaults(WebUser user)
        {
            var vaults = _vaultRepository.GetList().Where(x=>x.VaultAdmin.Id != user.Id);
            var freeVaults = new List<UserVault>();
            foreach (var vault in vaults)
            {
                if (vault.AllowCreate != null)
                {
                    if (vault.AllowCreate.All(x => x.Id != user.Id))
                    {
                       if (vault.AllowRead != null)
                        {
                            if (vault.AllowRead.All(x => x.Id != user.Id))
                            {
                                freeVaults.Add(vault);
                            }
                            else
                            {
                                continue;
                            }
                        }
                        else
                        {
                            freeVaults.Add(vault);
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
                else
                {
                    if (vault.AllowRead != null)
                    {
                        if (vault.AllowRead.All(x => x.Id != user.Id))
                        {
                            freeVaults.Add(vault);
                        }
                    }
                }
            }
            return freeVaults.Distinct();
        }
    }
}