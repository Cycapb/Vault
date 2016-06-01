using System.Collections.Generic;
using System.Web.Mvc;
using VaultDAL.Models;

namespace Vault.Models
{
    public class EditVaultModel
    {
        [HiddenInput(DisplayValue = false)]
        public string Id { get; set; }
        public IEnumerable<VaultUser> AllowReadUsers { get; set; } 
        public IEnumerable<VaultUser> AllowCreateUsers { get; set; } 
    }

    public class VaultModificationModel
    {
        public string VaultId { get; set; }
        public string[] ReadUsers { get; set; }
        public string[] CreateUsers { get; set; }
    }
}