using System.Collections.Generic;
using System.Web.Mvc;
using VaultDAL.Models;

namespace Vault.Models
{
    public class EditVaultModel
    {
        [HiddenInput(DisplayValue = false)]
        public string Id { get; set; }
        public IList<VaultUser> AllowReadUsers { get; set; } 
        public IList<VaultUser> AllowCreateUsers { get; set; } 
    }

    public class VaultModificationModel
    {
        public string VaultId { get; set; }
        public IList<VaultUser> ReadUsers { get; set; } 
        public IList<VaultUser> CreateUsers { get; set; } 
    }

    public class AddUsersModel
    {
        public string VaultId { get; set; }
        public IEnumerable<VaultUser> FreeUsers { get; set; }
        public string[] AccessRights => new string[] {"Read","Create"};
    }

    public class UserToAddModel
    {
        public string VaultId { get; set; }
        public string UserId { get; set; }
        public string AccessRight { get; set; }
    }
}