using System.Collections.Generic;
using System.Threading.Tasks;
using VaultDAL.Abstract;
using VaultDAL.Models;

namespace VaultService.Models
{
    public class VaultHelper
    {
        private readonly IRepository<UserVault> _userVaultRepository;

        public VaultHelper(IRepository<UserVault> userVaultrepository)
        {
            _userVaultRepository = userVaultrepository;
        }

        public async Task<IEnumerable<UserVault>> GetVaults()
        {
            return await _userVaultRepository.GetListAsync();
        }
    }
}
