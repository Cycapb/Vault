using System.Collections.Generic;
using VaultDAL.Models;

namespace Vault.Models
{
    public class VaultAccessLogModel
    {
        public string VaultId { get; set; }
        public IEnumerable<VaultAccessLog> Events { get; set; }

        public PagingInfo PagingInfo;
    }
}