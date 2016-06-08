using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using VaultDAL.Models;

namespace Vault.Models
{
    public class EditUsersModel
    {
        [HiddenInput(DisplayValue = false)]
        public string Id { get; set; }
        public IList<VaultUser> AllowReadUsers { get; set; } 
        public IList<VaultUser> AllowCreateUsers { get; set; } 
    }

    [EditTimeComparator]
    public class EditVaultModel
    {
        public string Id { get; set; }
        [Required(ErrorMessage = "Field Name can't be empty")]
        public string Name { get; set; }
        public string Description { get; set; }
        public int OpenTime { get; set; }
        public int CloseTime { get; set; }
    }

    public class UsersModificationModel
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

    public class VaultItemListModel
    {
        public string VaultId { get; set; }
        public string AccessRight { get; set; }
        public IEnumerable<VaultItem> VaultItems { get; set; } 
        public string ReturnUrl { get; set; }
    }
}