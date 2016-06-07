using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Vault.Abstract;
using Vault.Models;
using VaultDAL.Models;

namespace Vault.Controllers
{
    [Authorize(Roles = "Users,VaultAdmins")]
    public class VaultItemController : Controller
    {
        private readonly IVaultItemManager _vaultItemManager;
        private readonly IMailReporter _mailtReporter;
        private readonly IAccessManager _accessManager;
        private readonly IDbLogger _dbLogger;
        private readonly IVaultManager _vaultManager;

        private AppUserManager UserManager => HttpContext.GetOwinContext().GetUserManager<AppUserManager>();
        public VaultItemController(IVaultItemManager vaultItemManager, IMailReporter mailReporter, 
            IAccessManager accessManager, IDbLogger dbLogger, IVaultManager vaultManager)
        {
            _vaultItemManager = vaultItemManager;
            _mailtReporter = mailReporter;
            _accessManager = accessManager;
            _dbLogger = dbLogger;
            _vaultManager = vaultManager;
        }

        public ActionResult Index()
        {
            return RedirectToAction("Items");
        }

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
                    Task.Run(() => _dbLogger.Log($"User {user.UserName} tried to get access to the vault when it was closed"));
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

        public async Task<ActionResult> AddItem(WebUser user, string id)
        {
            var access = await _accessManager.GetUserAccess(id, user.Id);
            if (access != "Create")
            {
                return RedirectToAction("Items", new { id = id });
            }
            return View(new CreateVaultItemModel() { VaultId = id });
        }

        [HttpPost]
        public async Task<ActionResult> AddItem(WebUser user, CreateVaultItemModel model)
        {
            if (ModelState.IsValid)
            {
                var vaultItem =
                    await _vaultItemManager.CreateAsync(new VaultItem() { Content = model.Content, Name = model.Name });
                var vault = await _vaultManager.GetVault(model.VaultId);
                if (vault.VaultItems == null)
                {
                    vault.VaultItems = new List<string>() { vaultItem.Id };
                }
                else
                {
                    vault.VaultItems.Add(vaultItem.Id);
                }
                await _vaultManager.UpdateAsync(vault);
                InitiateDbLogger(model.VaultId, "Create");
                await _dbLogger.Log($"User {user.UserName} has created new vault item called {model.Name}");
                TempData["message"] = $"Vault item with name {model.Name} has been successfully created";
                return RedirectToAction("Items", new { id = model.VaultId });
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
            TempData["message"] = $"Vault item has been successfully deleted";
            return RedirectToAction("Items", new { id = vaultId });
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
                TempData["message"] = $"Vault item with name {model.VaultItem.Name} has been successfully updated";
                return RedirectToAction("Items", new { id = model.VaultId });
            }
            else
            {
                return View(model);
            }
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