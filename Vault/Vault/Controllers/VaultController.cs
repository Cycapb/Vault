using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Vault.Abstract;
using Vault.Models;
using VaultDAL.Models;

namespace Vault.Controllers
{
    [Authorize(Roles = "VaultAdmins")]
    public class VaultController : Controller
    {
        private readonly IVaultHelper _vaultHelper;

        public VaultController(IVaultHelper vaultHelper)
        {
            _vaultHelper = vaultHelper;
        }

        public ActionResult Index()
        {
            return RedirectToAction("VaultList");
        }

        public async Task<ActionResult> VaultList(WebUser user)
        {
            var vaultList = await _vaultHelper.GetVaults(user.Id);
            return View(vaultList.ToList());
        }

        public ActionResult Create()
        {
            var vault = new UserVault();
            return View(vault);
        }

        [HttpPost]
        public async Task<ActionResult> Create(UserVault vault)
        {
            if (ModelState.IsValid)
            {
                await _vaultHelper.CreateAsync(vault);
                return RedirectToAction("Index");
            }
            return View(vault);
        }

        [HttpPost]
        public async Task<ActionResult> Delete(string id)
        {
            await _vaultHelper.DeleteAsync(id);
            return RedirectToAction("Index");
        }
    }
}