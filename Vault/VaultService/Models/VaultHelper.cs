using System.Collections.Generic;
using System.Threading.Tasks;
using VaultDAL.Abstract;
using VaultDAL.Concrete;
using VaultDAL.Models;

namespace VaultService.Models
{
    public class VaultHelper
    {
        private readonly IRepository<UserVault> _userVault;

        public VaultHelper()
        {
            _userVault = new MongoRepository<UserVault>(new DbConnectionProvider());
        }

        public async Task<IEnumerable<UserVault>> GetVaults()
        {
            return await _userVault.GetListAsync();
        }
    }
}
