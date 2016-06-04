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
        private readonly IVaultManager _vaultManager;
        private readonly IVaultItemManager _vaultItemManager;
        private readonly IUserGetter<VaultUser> _userGetter;
        private readonly IAccessManager _accessManager;

        private AppUserManager UserManager =>
            System.Web.HttpContext.Current.GetOwinContext().GetUserManager<AppUserManager>();

        public VaultController(IVaultManager vaultHelper, IUserGetter<VaultUser> getter, IAccessManager accessManager, IVaultItemManager vaultItemManager )
        {
            _vaultManager = vaultHelper;
            _userGetter = getter;
            _accessManager = accessManager;
            _vaultItemManager = vaultItemManager;
        }

        public ActionResult Index()
        {
            return RedirectToAction("VaultList");
        }

        public async Task<ActionResult> VaultList(WebUser user)
        {
            var vaultList = await _vaultManager.GetVaults(user.Id);
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
                    await _vaultManager.CreateAsync(vault);
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
                await _vaultManager.DeleteAsync(id);
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return View("Error",new string[] {"Something went wrong. Please try again"});
            }
        }

        public async Task<ActionResult> Edit(string id)
        {
            var vault = await _vaultManager.GetVault(id);
            var editModel = new EditVaultModel()
            {
                Name = vault.Name,
                Description = vault.Description,
                OpenTime = vault.OpenTime,
                CloseTime = vault.CloseTime
            };
            return View(editModel);
        }

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
                return RedirectToAction("Index");
            }
            return View(vault);
        }

        public async Task<ActionResult> EditUsers(string id)
        {
            var vault = await _vaultManager.GetVault(id);
            if (vault != null)
            {
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

        [ChildActionOnly]
        public ActionResult VaultUsers(string id)
        {
            var users = _vaultManager.GetAllUsers(id);
            ViewBag.VaultId = id;
            return PartialView("VaultUsersPartial",users);
        }

        [HttpPost]
        public async Task<ActionResult> DeleteUser(string id, string vaultId)
        {
            var userToDel = new VaultUser() { Id = id };
            await _accessManager.RevokeReadAccess(userToDel, vaultId);
            await _accessManager.RevokeCreateAccess(userToDel, vaultId);
            return RedirectToAction("EditUsers", new {id = vaultId});
        }

        public async Task<ActionResult> Items(WebUser user,string id)
        {
            var items = await _vaultManager.GetAllItems(id);
            var editItem = new VaultItemListModel()
            {
                VaultId = id,
                VaultItems = items,
                AccessRight = await _vaultManager.GetUserAccess(id, user.Id)
            };
            return View(editItem);
        }

        public ActionResult AddItem(string id)
        {
            return View(new CreateVaultItemModel() {VaultId = id});
        }

        [HttpPost]
        public async Task<ActionResult> AddItem(CreateVaultItemModel model)
        {
            if (ModelState.IsValid)
            {
                var vaultItem = await _vaultItemManager.CreateAsync(new VaultItem() {Content = model.Content, Name = model.Name});
                var vault = await _vaultManager.GetVault(model.VaultId);
                if (vault.VaultItems == null)
                {
                    vault.VaultItems = new List<string>() {vaultItem.Id};
                }
                else
                {
                    vault.VaultItems.Add(vaultItem.Id);
                }
                await _vaultManager.UpdateAsync(vault);
                return RedirectToAction("Items", new {id = model.VaultId});
            }
            else
            {
                return View(model);
            }
        }

        [HttpPost]
        public async Task<ActionResult> DeleteItem(string vaultId, string itemId)
        {
            await _vaultManager.DeleteItemAsync(vaultId, itemId);
            return RedirectToAction("Items",new {id = vaultId});
        }
    }
}