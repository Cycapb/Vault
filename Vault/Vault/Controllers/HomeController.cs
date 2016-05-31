using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Vault.Abstract;
using Vault.Models;

namespace Vault.Controllers
{
    public class HomeController : Controller
    {
        private readonly IVaultGetter _vaultGetter;

        public HomeController(IVaultGetter vaultGetter)
        {
            _vaultGetter = vaultGetter;
        }

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

        public async Task<ActionResult> UserVaults(WebUser user)
        {
            var userVaults = (await _vaultGetter.GetAsync(user)).ToList();
            return PartialView(userVaults);
        }

        public ActionResult AllVaults()
        {
            var allFreeVaults = _vaultGetter.GetAllVaultsAsync();
            return PartialView(allFreeVaults);
        }
    }
}
