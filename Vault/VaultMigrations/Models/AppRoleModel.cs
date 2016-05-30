using AspNet.Identity.MongoDB;

namespace VaultMigrations.Models
{
    public class AppRoleModel:IdentityRole
    {
        public AppRoleModel() : base(){ }

        public AppRoleModel(string name) : base(name) { }
    }
}