using System;
using System.Collections.Generic;
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

        public ActionResult Create(WebUser user)
        {
            var vault = new UserVault()
            {
                VaultUsers = new List<VaultUser>(),
                AllowRead = new List<VaultUser>(),
                AllowCreate = new List<VaultUser>(),
            };
            return View(vault);
        }

        [HttpPost]
        public async Task<ActionResult> Create(WebUser user,UserVault vault)
        {
            vault.VaultAdmin = new VaultUser() {Id = user.Id};
            if (ModelState.IsValid)
            {
                try
                {
                    await _vaultHelper.CreateAsync(vault);
                    return RedirectToAction("Index");
                }
                catch (Exception)
                {
                   return View("Error",new string[] {"Something went wrong. Please try again."});
                }
            }
            return View(vault);
        }

        [HttpPost]
        public async Task<ActionResult> Delete(string id)
        {
            try
            {
                await _vaultHelper.DeleteAsync(id);
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return View("Error",new string[] {"Something went wrong. Please try again"});
            }
        }

        public ActionResult Edit(string id)
        {
            return View();
        }

        public async Task<ActionResult> EditUsers(string id)
        {
            var vault = await _vaultHelper.GetVault(id);
            if (vault != null)
            {
                var editModel = new EditVaultModel()
                {
                    Id = vault.Id,
                    AllowCreateUsers = vault.AllowCreate,
                    AllowReadUsers = vault.AllowRead
                };
                return View(editModel);
            }
            else
            {
                return View("VaultList");
            }
        }

        [HttpPost]
        public async Task<ActionResult> EditUsers(VaultModificationModel model)
        {
            return View();
        }

        //ToDo When adding users to vaults i have to ckeck if user has both create and read rights/ If so then i must remove read rights from user
        
        public ActionResult AddUsers(string id)
        {
            return View();
        }
    }
}