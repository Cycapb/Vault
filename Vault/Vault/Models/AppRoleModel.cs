using AspNet.Identity.MongoDB;

namespace Vault.Models
{
    public class AppRoleModel:IdentityRole
    {
        public AppRoleModel() : base(){ }

        public AppRoleModel(string name) : base(name) { }
    }
}