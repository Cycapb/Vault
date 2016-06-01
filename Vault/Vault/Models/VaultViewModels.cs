using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using VaultDAL.Models;

namespace Vault.Models
{
    public class EditVaultModel
    {
        [HiddenInput(DisplayValue = false)]
        public string Id { get; set; }
        [Required(ErrorMessage = "Field Name can't be empty")]
        public string Name { get; set; }
        public IEnumerable<VaultUser> AllowReadUsers { get; set; } 
        public IEnumerable<VaultUser> AllowCreateUsers { get; set; } 
    }

    public class VaultModificationModel
    {
        public string Id { get; set; }
        public IEnumerable<VaultUser> ReadUsers { get; set; }
        public IEnumerable<VaultUser> CreateUsers { get; set; }
    }
}