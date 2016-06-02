using System.Threading.Tasks;
using VaultDAL.Models;

namespace Vault.Abstract
{
    public interface IAccessManager
    {
        Task GrantReadAccess(VaultUser vaultUser, string vaultId);
        Task GrantCreateAccess(VaultUser vaultUser, string vaultId);
    }
}
