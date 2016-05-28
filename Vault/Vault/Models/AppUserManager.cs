using AspNet.Identity.MongoDB;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Vault.Infrastructure;

namespace Vault.Models
{
    public class AppUserManager:UserManager<AppUserModel>
    {
        public AppUserManager(IUserStore<AppUserModel> store) : base(store)
        {
        }

        public static AppUserManager Create(IdentityFactoryOptions<AppUserManager> options, IOwinContext context)
        {
            AppUserManager manager = new AppUserManager(new UserStore<AppUserModel>(context.Get<AppIdentityDbContext>().Users));
            return manager;
        }
    }
}