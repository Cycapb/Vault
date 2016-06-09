using System.Collections.Generic;
using VaultDAL.Models;

namespace VaultService.Models
{
    public class VaultUserEqualityComparer:IEqualityComparer<VaultUser>
    {
        public bool Equals(VaultUser x, VaultUser y)
        {
            return x.Id.Equals(y.Id);
        }

        public int GetHashCode(VaultUser obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}