using System.Threading.Tasks;

namespace Vault.Abstract
{
    public interface IVaultItemHelper
    {
        Task Log(string vaultId, string accessType, string message);
    }
}
