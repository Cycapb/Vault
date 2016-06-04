using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Vault.Models;

namespace Vault.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private AppUserManager UserManager => HttpContext.GetOwinContext().GetUserManager<AppUserManager>();
        private IAuthenticationManager AuthMan => HttpContext.GetOwinContext().Authentication;

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return View("Error",new string[] {"User has already authenticated or you tried to access forbidden url"});
            }
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindAsync(model.Name, model.Password);
                if (user == null)
                {
                    ModelState.AddModelError("","Invalid name or password");
                }
                else
                {
                    ClaimsIdentity ident =
                        await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
                    AuthMan.SignOut();
                    AuthMan.SignIn(new AuthenticationProperties() {IsPersistent = false}, ident);
                    return Redirect(returnUrl);
                }
            }
            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        [Authorize]
        public ActionResult LogOut()
        {
            AuthMan.SignOut();
            return RedirectToAction("Index", "Home");
        }
    }
}