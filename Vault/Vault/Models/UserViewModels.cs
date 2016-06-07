using System.Collections.Generic;
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

    public class LoginModel
    {
        [Required(ErrorMessage = "Field Login can't be empty")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Field password can't be empty")]
        public string Password { get; set; }
    }

    public class EditRolemodel
    {
        public AppRoleModel Role { get; set; }
        public IEnumerable<AppUserModel> Members { get; set; }
        public IEnumerable<AppUserModel> NonMembers { get; set; }   
    }

    public class RoleModificationModel
    {
        public string RoleName { get; set; }
        public string[] UsersToAdd { get; set; }
        public string[] UsersToDelete { get; set; }
    }

    public class RegisterModel
    {
        [Required(ErrorMessage = "Field UserName can't be empty")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Field Email can't be empty")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Field Password can't be empty")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Field PasswordConfirmation can't be empty")]
        [System.ComponentModel.DataAnnotations.Compare("Password")]
        public string PasswordConfirmation { get; set; }
    }
}