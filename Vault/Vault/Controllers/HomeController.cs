using System.Web.Mvc;
using Vault.Models;

namespace Vault.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            var currentUser = HttpContext.User;
            if (currentUser.IsInRole("Administrators"))
            {
                return RedirectToAction("Index", "Admin");
            }
            return View();
        }

        public PartialViewResult ViewCredentials(WebUser user)
        {
            return PartialView(user);
        }
    }
}