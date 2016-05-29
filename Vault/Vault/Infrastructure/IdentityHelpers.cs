using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using MongoDB.Driver;

namespace Vault.Infrastructure
{
    public static class IdentityHelpers
    {
        public static MvcHtmlString GetRoleUsers(this HtmlHelper html, string name)
        {
            var identityContext = HttpContext.Current.GetOwinContext().GetUserManager<AppIdentityDbContext>();
            var usersInRole = identityContext.Users.Find(u => u.Roles.Contains(name)).ToListAsync().Result;
            var users = string.Join(", ", usersInRole);
            return new MvcHtmlString(users);
        }
    }
}