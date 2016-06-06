using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VaultDAL.Models;

namespace Vault.Abstract
{
    public interface ILogManager<T> where T:VaultAccessLog
    {
        Task<IEnumerable<T>> ShowLog(string vaultId);
        Task<IEnumerable<T>> ShowByDateLog(string vaultId, DateTime date);
    }
}
