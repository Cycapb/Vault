using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using Vault.Abstract;
using Vault.Models;
using VaultDAL.Models;

namespace Vault.Controllers
{
    [Authorize(Roles = "VaultAdmins")]
    public class VaultController : Controller
    {
        private readonly IVaultHelper _vaultHelper;
        private readonly IUserGetter<VaultUser> _userGetter;
        private readonly IAccessManager _accessManager;

        private AppUserManager UserManager =>
            System.Web.HttpContext.Current.GetOwinContext().GetUserManager<AppUserManager>();

        public VaultController(IVaultHelper vaultHelper, IUserGetter<VaultUser> getter, IAccessManager accessManager )
        {
            _vaultHelper = vaultHelper;
            _userGetter = getter;
            _accessManager = accessManager;
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
                AllowRead = new List<VaultUser>(),
                AllowCreate = new List<VaultUser>(),
            };
            return View(vault);
        }

        [HttpPost]
        public async Task<ActionResult> Create(WebUser user,UserVault vault)
        {
            vault.VaultAdmin = new VaultUser() {Id = user.Id,UserName = user.UserName};
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

        public async Task<ActionResult> Edit(string id)
        {
            var vault = await _vaultHelper.GetVault(id);
            return View(vault);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(UserVault vault)
        {
            if (ModelState.IsValid)
            {
                await _vaultHelper.UpdateAsync(vault);
                return RedirectToAction("Index");
            }
            return View(vault);
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
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<ActionResult> EditUsers(VaultModificationModel model)
        {
            if (model.ReadUsers != null)
            {
                var accessRights = GetUserAccessRights(model.ReadUsers);
                foreach (var user in accessRights)
                {
                    await _accessManager.RevokeCreateAccess(user, model.VaultId);
                    await _accessManager.GrantReadAccess(user, model.VaultId);
                }
            }
            if (model.CreateUsers != null)
            {
                var accessRights = GetUserAccessRights(model.CreateUsers);
                foreach (var user in accessRights)
                {
                    await _accessManager.RevokeReadAccess(user, model.VaultId);
                    await _accessManager.GrantCreateAccess(user, model.VaultId);
                }
            }
            return RedirectToAction("EditUsers");
        }

        public ActionResult AddUsers(string id)
        {
            var addUserModel = new AddUsersModel()
            {
                VaultId = id,
                FreeUsers = _userGetter.Get().ToList(),
            };
            return PartialView(addUserModel);
        }

        [HttpPost]
        public async Task<ActionResult> AddUsers(UserToAddModel user)
        {
            var vaultUser = new VaultUser()
            {
                Id = user.UserId,
                UserName = (await UserManager.FindByIdAsync(user.UserId)).UserName
            };
            if (user.AccessRight == "Read")
            {
                await _accessManager.GrantReadAccess(vaultUser, user.VaultId);
            }
            if (user.AccessRight == "Create")
            {
                await _accessManager.GrantCreateAccess(vaultUser, user.VaultId);
            }
            return RedirectToAction("EditUsers",new {id = user.VaultId});
        }

        private IList<VaultUser> GetUserAccessRights(IList<VaultUser> accessRights)
        {
            return accessRights.Where(accessRight => accessRight.Id != null).ToList();
        }
    }
}