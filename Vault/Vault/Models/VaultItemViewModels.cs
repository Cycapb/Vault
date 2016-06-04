using System.ComponentModel.DataAnnotations;
using VaultDAL.Models;

namespace Vault.Models
{
    public class CreateVaultItemModel
    {
        public string VaultId { get; set; }
        [Required(ErrorMessage = "Field Name can't be empty")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Field Content can't be empty")]
        public string Content { get; set; }
    }

    public class EditVaultItemModel
    {
        public string VaultId { get; set; }
        public VaultItem VaultItem { get; set; }
    }
}