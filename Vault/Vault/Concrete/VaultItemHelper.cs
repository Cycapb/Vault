using System.Threading.Tasks;
using Vault.Abstract;

namespace Vault.Concrete
{
    public class VaultItemHelper:IVaultItemHelper
    {
        private readonly IDbLogger _dbLogger;

        public VaultItemHelper(IDbLogger dbLogger)
        {
            _dbLogger = dbLogger;
        }

        public async Task Log(string vaultId, string accessType, string message)
        {
            _dbLogger.VaultId = vaultId;
            _dbLogger.EventType = accessType;
            await _dbLogger.Log(message);
        }
    }
}