using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using MongoDB.Driver;
using Vault.Infrastructure;
using Vault.Models;

namespace Vault.Controllers
{
    public class UserAdminController : Controller
    {
        private AppUserManager UserManager => HttpContext.GetOwinContext().GetUserManager<AppUserManager>();

        private AppIdentityDbContext IdentityContext
            => HttpContext.GetOwinContext().GetUserManager<AppIdentityDbContext>();

        public async Task<ActionResult> Index()
        {
            var list = await IdentityContext.Users.Find(x=>true).ToListAsync();
            return View(list);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(CreateModel userModel)
        {
            if (ModelState.IsValid)
            {
                var newUser = new AppUserModel() {UserName = userModel.Name, Email = userModel.Email};
                var result = await UserManager.CreateAsync(newUser, userModel.Password);
                if (result.Succeeded)
                {
                   return RedirectToAction("Index");
                }
                else
                {
                    AddModelErrors(result);
                }
            }
            return View(userModel);
        }

        [HttpPost]
        public async Task<ActionResult> Delete(string id)
        {
            var userToDel = await UserManager.FindByIdAsync(id);
            if (userToDel != null)
            {
                var result = await UserManager.DeleteAsync(userToDel);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    AddModelErrors(result);
                    return View("Error", result.Errors);
                }
            }
            else
            {
                return View("Error", new string[] {"User not found"});
            }
        } 

        private void AddModelErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("",error);
            }
        }
    }
}