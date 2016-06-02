using System.Collections.Generic;
using System.Threading.Tasks;
using Vault.Abstract;
using VaultDAL.Abstract;
using VaultDAL.Models;

namespace Vault.Concrete
{
    public class AccessManager:IAccessManager
    {
        private readonly IRepository<UserVault> _userVaultRepository;

        public AccessManager(IRepository<UserVault> userVaultRepository)
        {
            _userVaultRepository = userVaultRepository;
        }

        public async Task GrantReadAccess(VaultUser vaultUser, string vaultId)
        {
            var vault = await _userVaultRepository.GetItemAsync(vaultId);
            
            if (vault.AllowRead == null)
            {
                vault.AllowRead = new List<VaultUser> {vaultUser};
            }
            else
            {
                vault.AllowRead.Add(vaultUser);
            }
            await _userVaultRepository.UpdateAsync(vault);
        }

        public async Task GrantCreateAccess(VaultUser vaultUser, string vaultId)
        {
            var vault = await _userVaultRepository.GetItemAsync(vaultId);
            
            if (vault.AllowCreate == null)
            {
                vault.AllowCreate = new List<VaultUser> { vaultUser };
            }
            else
            {
                vault.AllowCreate.Add(vaultUser);
            }
            await _userVaultRepository.UpdateAsync(vault);
        }
    }
}