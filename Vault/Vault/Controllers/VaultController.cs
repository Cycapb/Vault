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
    [Authorize(Roles = "VaultAdmins,Users")]
    public class VaultController : Controller
    {
        private readonly IVaultManager _vaultManager;
        private readonly IUserGetter<VaultUser> _userGetter;
        private readonly IAccessManager _accessManager;
        private readonly ILogManager<VaultAccessLog> _logManager;


        private AppUserManager UserManager => HttpContext.GetOwinContext().GetUserManager<AppUserManager>();

        public VaultController(IVaultManager vaultManager,
            IUserGetter<VaultUser> getter,
            IAccessManager accessManager,
            ILogManager<VaultAccessLog> logManager)
        {
            _vaultManager = vaultManager;
            _userGetter = getter;
            _accessManager = accessManager;
            _logManager = logManager;
        }

        [Authorize(Roles = "VaultAdmins")]
        public ActionResult Index()
        {
            return RedirectToAction("VaultList");
        }

        [Authorize(Roles = "VaultAdmins")]
        public async Task<ActionResult> VaultList(WebUser user)
        {
            var vaultList = await _vaultManager.GetVaults(user.Id);
            return View(vaultList.ToList());
        }

        [Authorize(Roles = "VaultAdmins")]
        public ActionResult Create(WebUser user)
        {
            var vault = new UserVault()
            {
                AllowRead = new List<VaultUser>(),
                AllowCreate = new List<VaultUser>(),
            };
            return View(vault);
        }

        [Authorize(Roles = "VaultAdmins")]
        [HttpPost]
        public async Task<ActionResult> Create(WebUser user, UserVault vault)
        {
            vault.VaultAdmin = new VaultUser() {Id = user.Id, UserName = user.UserName};
            if (ModelState.IsValid)
            {
                try
                {
                    await _vaultManager.CreateAsync(vault);
                    TempData["message"] = $"Vault with name {vault.Name} has been successfully created";
                    return RedirectToAction("Index");
                }
                catch (Exception)
                {
                    return View("Error", new string[] {"Something went wrong. Please try again."});
                }
            }
            return View(vault);
        }

        [Authorize(Roles = "VaultAdmins")]
        [HttpPost]
        public async Task<ActionResult> Delete(WebUser user, string id)
        {
            try
            {
                var admin = await _vaultManager.GetVaultAdmin(id);
                if (admin.Id != user.Id)
                {
                    return RedirectToAction("Index");
                }
                TempData["message"] = $"Vault has been successfully deleted";
                await _vaultManager.DeleteAsync(id);
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return View("Error", new string[] {"Something went wrong. Please try again"});
            }
        }

        [Authorize(Roles = "VaultAdmins")]
        public async Task<ActionResult> Edit(WebUser user, string id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            var vault = await _vaultManager.GetVault(id);
            if (vault == null){ return RedirectToAction("Index"); }
            if (vault.VaultAdmin.Id != user.Id)
            {
                return RedirectToAction("Index");
            }
            var editModel = new EditVaultModel()
            {
                Name = vault.Name,
                Description = vault.Description,
                OpenTime = vault.OpenTime,
                CloseTime = vault.CloseTime
            };
            return View(editModel);
        }

        [Authorize(Roles = "VaultAdmins")]
        [HttpPost]
        public async Task<ActionResult> Edit(EditVaultModel vault)
        {
            if (ModelState.IsValid)
            {
                var vaultToEdit = await _vaultManager.GetVault(vault.Id);
                vaultToEdit.Name = vault.Name;
                vaultToEdit.Description = vault.Description;
                vaultToEdit.OpenTime = vault.OpenTime;
                vaultToEdit.CloseTime = vault.CloseTime;
                await _vaultManager.UpdateAsync(vaultToEdit);
                TempData["message"] = $"Vault with name {vault.Name} has been successfully updated";
                return RedirectToAction("Index");
            }
            return View(vault);
        }

        [Authorize(Roles = "VaultAdmins")]
        public async Task<ActionResult> EditUsers(WebUser user, string id)
        {
            var vault = await _vaultManager.GetVault(id);
            if (vault != null)
            {
                if (vault.VaultAdmin.Id != user.Id)
                {
                    return RedirectToAction("Index");
                }
                var editModel = new EditUsersModel()
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

        [Authorize(Roles = "VaultAdmins")]
        [HttpPost]
        public async Task<ActionResult> EditUsers(UsersModificationModel model)
        {
            if (model.ReadUsers != null)
            {
                var accessRights = model.ReadUsers.Where(accessRight => accessRight.Id != null).ToList();
                foreach (var user in accessRights)
                {
                    await _accessManager.RevokeCreateAccess(user, model.VaultId);
                    await _accessManager.GrantReadAccess(user, model.VaultId);
                }
            }
            if (model.CreateUsers != null)
            {
                var accessRights = model.CreateUsers.Where(accessRight => accessRight.Id != null).ToList();
                foreach (var user in accessRights)
                {
                    await _accessManager.RevokeReadAccess(user, model.VaultId);
                    await _accessManager.GrantCreateAccess(user, model.VaultId);
                }
            }
            return RedirectToAction("EditUsers");
        }

        [Authorize(Roles = "VaultAdmins")]
        [ChildActionOnly]
        public ActionResult AddUsers(string id)
        {
            var addUserModel = new AddUsersModel()
            {
                VaultId = id,
                FreeUsers = _userGetter.Get().ToList(),
            };
            return PartialView(addUserModel);
        }

        [Authorize(Roles = "VaultAdmins")]
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
            await _accessManager.ValidateUserAccessRights(user.VaultId, user.UserId);
            return RedirectToAction("EditUsers", new {id = user.VaultId});
        }

        [Authorize(Roles = "VaultAdmins")]
        [ChildActionOnly]
        public ActionResult VaultUsers(string id)
        {
            var users = _vaultManager.GetAllUsers(id);
            ViewBag.VaultId = id;
            return PartialView("VaultUsersPartial", users);
        }

        [Authorize(Roles = "VaultAdmins")]
        [HttpPost]
        public async Task<ActionResult> DeleteUser(string id, string vaultId)
        {
            var userToDel = new VaultUser() {Id = id};
            await _accessManager.RevokeReadAccess(userToDel, vaultId);
            await _accessManager.RevokeCreateAccess(userToDel, vaultId);
            return RedirectToAction("EditUsers", new {id = vaultId});
        }

        [Authorize(Roles = "VaultAdmins")]
        public async Task<ActionResult> VaultLog(WebUser user, string id)
        {
            var vaultName = (await _vaultManager.GetVault(id)).Name;
            var events = await _logManager.ShowLog(id);
            var logModel = new VaultAccessLogModel()
            {
                Events = events,
                VaultName = vaultName
            };
            return View(logModel);
        }
    }
}