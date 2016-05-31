using System.Web.Mvc;

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
    }
}