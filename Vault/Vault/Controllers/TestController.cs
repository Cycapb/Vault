using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using Vault.Models;

namespace Vault.Controllers
{
    public class TestController : Controller
    {
        private AppUserManager UserManager => HttpContext.GetOwinContext().GetUserManager<AppUserManager>();

        public async Task<ActionResult> Index()
        {
            var user = await UserManager.FindByIdAsync("5749938fb3b6d23d540be565");
            var list = new List<AppUserModel>();
            if (user != null)
            {
                list.Add(user);
            }
            return View(list);
        }
    }
}