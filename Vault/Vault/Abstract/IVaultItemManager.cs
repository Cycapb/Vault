using System.Collections.Generic;
using VaultDAL.Models;

namespace Vault.Abstract
{
    public interface IVaultItemManager
    {
        void Create(VaultItem item);
        void Delete(string id);
        void Update(VaultItem item);
        IEnumerable<VaultItem> GetAll(string vaultId);
    }
}
