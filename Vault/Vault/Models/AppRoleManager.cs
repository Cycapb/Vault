using AspNet.Identity.MongoDB;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Vault.Infrastructure;

namespace Vault.Models
{
    public class AppRoleManager:RoleManager<AppRoleModel>
    {
        public AppRoleManager(IRoleStore<AppRoleModel, string> store) : base(store)
        {
        }

        public static AppRoleManager Create(IdentityFactoryOptions<AppRoleManager> options, IOwinContext context)
        {
            AppRoleManager manager = new AppRoleManager(new RoleStore<AppRoleModel>(context.Get<AppIdentityDbContext>().Roles));
            return manager;
        }
    }
}