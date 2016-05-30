using Microsoft.AspNet.Identity;

namespace VaultMigrations.Models
{
    public class AppUserManager:UserManager<AppUserModel>
    {
        public AppUserManager(IUserStore<AppUserModel> store) : base(store)
        {
        }
    }
}
