using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using MongoDB.Driver;
using Vault.Abstract;
using Vault.Infrastructure;
using VaultDAL.Models;

namespace Vault.Concrete
{
    public class UserGetter:IUserGetter<VaultUser>
    {
        private AppIdentityDbContext UserManager => HttpContext.Current.GetOwinContext().GetUserManager<AppIdentityDbContext>();
        private string UserId => HttpContext.Current.User.Identity.GetUserId();

        public IEnumerable<VaultUser> Get()
        {
            var currentUser = new VaultUser() {Id = UserId};
            var allUsers = UserManager.Users.AsQueryable()
                .Where(x => x.Id != currentUser.Id)
                .Select(x => x)
                .Where(x => x.Roles.Contains("VaultAdmins") || x.Roles.Contains("Users"))
                .ToList();
            var freeUsers = new List<VaultUser>();
            allUsers.ForEach(user => freeUsers.Add(new VaultUser() {Id = user.Id,UserName = user.UserName}));
            return freeUsers;
        }
    }
}