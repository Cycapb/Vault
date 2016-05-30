using System.ComponentModel.DataAnnotations;
using VaultDAL.Concrete;


namespace VaultDAL.Models
{
    public class VaultItem:Entity
    {
        [Required(ErrorMessage = "Field name can't be empty")]
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
