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
        private readonly IVaultItemManager _vaultItemManager;
        private readonly IUserGetter<VaultUser> _userGetter;
        private readonly IAccessManager _accessManager;
        private readonly IDbLogger _dbLogger;
        private readonly ILogManager<VaultAccessLog> _logManager;
        private readonly IMailReporter _mailtReporter;

        private AppUserManager UserManager => HttpContext.GetOwinContext().GetUserManager<AppUserManager>();

        public VaultController(IVaultManager vaultManager,
            IUserGetter<VaultUser> getter,
            IAccessManager accessManager,
            IVaultItemManager vaultItemManager,
            IDbLogger dbLogger,
            ILogManager<VaultAccessLog> logManager,
            IMailReporter mailReporter)
        {
            _vaultManager = vaultManager;
            _userGetter = getter;
            _accessManager = accessManager;
            _vaultItemManager = vaultItemManager;
            _dbLogger = dbLogger;
            _logManager = logManager;
            _mailtReporter = mailReporter;
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

        //ToDo Refactor this method
        [Authorize(Roles = "Users,VaultAdmins")]
        public async Task<ActionResult> Items(WebUser user, string id, string returnUrl)
        {
            var accessRight = await _accessManager.GetUserAccess(id, user.Id);

            if ((await _vaultManager.GetVaultAdmin(id)).Id == user.Id)
            {
                var vaultItems = await _vaultManager.GetAllItems(id);
                var editmodel = CreateVaultItemListModel(id, vaultItems, returnUrl);
                editmodel.AccessRight = accessRight;
                return View(editmodel);
            }

            if (accessRight == null)
            {
                TempData["message"] = "You don't have enough rights to access this vault";
                InitiateDbLogger(id, "Deny");
                var message = $"User {user.UserName} tryied to get access to the vault";
                Task.Run(async () => await _dbLogger.Log(message));
                await ReportToAdmin(id, message);

                return RedirectToAction("Index", "Home");
            }
            else
            {
                if (!await _accessManager.TimeAccessAsync(id))
                {
                    TempData["message"] = "At this this time the vault you want to get access is closed";
                    InitiateDbLogger(id, "Deny");
                    Task.Run(() =>_dbLogger.Log($"User {user.UserName} tryied to get access to the vault when it was closed"));
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    if (accessRight == "Create")
                    {
                        var items = await _vaultManager.GetAllItems(id);
                        var editItem = CreateVaultItemListModel(id, items, returnUrl);
                        editItem.AccessRight = accessRight;
                        InitiateDbLogger(id, "Full Access");
                        Task.Run(() => _dbLogger.Log($"User {user.UserName} entered the vault"));
                        return View(editItem);
                    }
                    else
                    {
                        var items = await _vaultManager.GetAllItems(id);
                        var editItem = CreateVaultItemListModel(id, items, returnUrl);
                        editItem.AccessRight = accessRight;
                        InitiateDbLogger(id, "Read Access");
                        Task.Run(() => _dbLogger.Log($"User {user.UserName} entered the vault"));
                        return View(editItem);
                    }
                }
            }
        }

        public async Task<ActionResult> AddItem(WebUser user, string id)
        {
            var access = await _accessManager.GetUserAccess(id, user.Id);
            if (access != "Create")
            {
                return RedirectToAction("Items", new {id = id});
            }
            return View(new CreateVaultItemModel() {VaultId = id});
        }

        [HttpPost]
        public async Task<ActionResult> AddItem(WebUser user, CreateVaultItemModel model)
        {
            if (ModelState.IsValid)
            {
                var vaultItem =
                    await _vaultItemManager.CreateAsync(new VaultItem() {Content = model.Content, Name = model.Name});
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
                InitiateDbLogger(model.VaultId, "Create");
                await _dbLogger.Log($"User {user.UserName} has created new vault item called {model.Name}");
                return RedirectToAction("Items", new {id = model.VaultId});
            }
            else
            {
                return View(model);
            }
        }

        [HttpPost]
        public async Task<ActionResult> DeleteItem(WebUser user, string vaultId, string itemId)
        {
            await _vaultManager.DeleteItemAsync(vaultId, itemId);
            InitiateDbLogger(vaultId, "Delete");
            await _dbLogger.Log($"User {user.UserName} has deleted vault item");
            return RedirectToAction("Items", new {id = vaultId});
        }

        public async Task<ActionResult> EditItem(string id, string vaultId)
        {
            var vaultItem = await _vaultItemManager.GetItemAsync(id);
            var editModel = new EditVaultItemModel()
            {
                VaultId = vaultId,
                VaultItem = vaultItem
            };
            return View(editModel);
        }

        [HttpPost]
        public async Task<ActionResult> EditItem(WebUser user, EditVaultItemModel model)
        {
            if (ModelState.IsValid)
            {
                await _vaultItemManager.UpdateAsync(model.VaultItem);
                InitiateDbLogger(model.VaultId, "Edit");
                await _dbLogger.Log($"User {user.UserName} has edited new vault item called {model.VaultItem.Name}");
                return RedirectToAction("Items", new {id = model.VaultId});
            }
            else
            {
                return View(model);
            }
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

        private string CreateReturnUrl()
        {
            var isUser = HttpContext.User.IsInRole("Users");
            if (isUser)
            {
                return Url.Action("Index", "Home");
            }
            else
            {
                return Url.Action("Index");
            }
        }

        private void InitiateDbLogger(string vaultId, string accessType)
        {
            _dbLogger.VaultId = vaultId;
            _dbLogger.EventType = accessType;
        }

        private VaultItemListModel CreateVaultItemListModel(string id, IEnumerable<VaultItem> items, string returnUrl)
        {
            return new VaultItemListModel()
            {
                VaultId = id,
                ReturnUrl = returnUrl ?? CreateReturnUrl(),
                VaultItems = items
            };
        }

        private async Task ReportToAdmin(string vaultId, string message)
        {
            var vaultAdmin = await _vaultManager.GetVaultAdmin(vaultId);
            var userEmail = (await UserManager.FindByIdAsync(vaultAdmin.Id)).Email;
            _mailtReporter.MailTo = userEmail;
            Task.Run(async () => await _mailtReporter.Report($"{DateTime.Now}: {message}"));
        }
    }
}