using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Vault.Abstract;
using VaultDAL.Models;

namespace Vault.Concrete
{
    public class VaultHelper:IVaultHelper
    {
        public Task<IEnumerable<UserVault>> GetVaults(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<UserVault> GetVault(string id)
        {
            throw new NotImplementedException();
        }

        public Task CreateAsync(UserVault vault)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(UserVault vault)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<VaultUser>> GetReadUsers(string id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<VaultUser>> GetCreateUsers(string id)
        {
            throw new NotImplementedException();
        }
    }
}