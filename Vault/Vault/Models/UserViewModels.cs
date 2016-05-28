using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Vault.Models
{
    public class CreateModel
    {
        [Required(ErrorMessage = "Field Name is empty")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Field Email is empty")]
        public string Email { get; set; }   

        [Required(ErrorMessage = "Field Password is empty")]
        public string Password { get; set; }

    }

    public class EditModel
    {
        [HiddenInput(DisplayValue = false)]
        public string Id { get; set; }

        [Required(ErrorMessage = "Field Name is empty")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Field Email is empty")]
        public string Email { get; set; }
        
        public string Password { get; set; }
    }
}