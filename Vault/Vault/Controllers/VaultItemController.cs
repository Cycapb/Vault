using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
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
        private readonly IVaultManager _vaultManager;
        private readonly IVaultItemHelper _vaultItemHelper;

        private AppUserManager UserManager => HttpContext.GetOwinContext().GetUserManager<AppUserManager>();
        public VaultItemController(IVaultItemManager vaultItemManager, IMailReporter mailReporter, 
            IAccessManager accessManager, IVaultManager vaultManager, IVaultItemHelper vaultItemHelper)
        {
            _vaultItemManager = vaultItemManager;
            _mailtReporter = mailReporter;
            _accessManager = accessManager;
            _vaultManager = vaultManager;
            _vaultItemHelper = vaultItemHelper;
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
                var editmodel = CreateVaultItemListModel(id, vaultItems, accessRight, returnUrl);
                return View(editmodel);
            }

            if (accessRight == null)
            {
                TempData["message"] = "You don't have enough rights to access this vault";
                var message = $"User {user.UserName} tryied to get access to the vault";
                await _vaultItemHelper.Log(id, "Deny", message);
                await ReportToAdmin(id, message);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                if (!await _accessManager.TimeAccessAsync(id))
                {
                    TempData["message"] = "At this this time the vault you want to get access is closed";
                    var message = $"User {user.UserName} tried to get access to the vault when it was closed";
                    await _vaultItemHelper.Log(id, "Deny", message);
                    await ReportToAdmin(id, message);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    if (accessRight == "Create")
                    {
                        var items = await _vaultManager.GetAllItems(id);
                        var editItem = CreateVaultItemListModel(id, items, accessRight, returnUrl);
                        await _vaultItemHelper.Log(id, "Full Access", $"User {user.UserName} entered the vault");
                        return View(editItem);
                    }
                    else
                    {
                        var items = await _vaultManager.GetAllItems(id);
                        var editItem = CreateVaultItemListModel(id, items, accessRight, returnUrl);
                        await _vaultItemHelper.Log(id, "Read Access", $"User {user.UserName} entered the vault");
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
                return RedirectToAction("Items", new { id = id });
            }
            return View(new CreateVaultItemModel() { VaultId = id });
        }

        [HttpPost]
        public async Task<ActionResult> AddItem(WebUser user, CreateVaultItemModel model)
        {
            if (ModelState.IsValid)
            {
                var vaultItem = await _vaultItemManager.CreateAsync(new VaultItem() { Content = model.Content, Name = model.Name });
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
                await _vaultItemHelper.Log(model.VaultId, "Create", $"User {user.UserName} has created new vault item called {model.Name}");
                TempData["message"] = $"Vault item with name {model.Name} has been successfully created";
                return RedirectToAction("Items", new { id = model.VaultId });
            }
            else
            {
                return View(model);
            }
        }

        [HttpPost]
        public async Task<ActionResult> DeleteItem(WebUser user, string id, string itemId)
        {
            await _vaultItemManager.DeleteAsync(id,itemId);
            await _vaultItemHelper.Log(id, "Delete", $"User {user.UserName} has deleted vault item");
            TempData["message"] = $"Vault item has been successfully deleted";
            return RedirectToAction("Items", new { id = id });
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
                await _vaultItemHelper.Log(model.VaultId, "Edit", $"User {user.UserName} has edited new vault item called {model.VaultItem.Name}");
                TempData["message"] = $"Vault item with name {model.VaultItem.Name} has been successfully updated";
                return RedirectToAction("Items", new { id = model.VaultId });
            }
            else
            {
                return View(model);
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

        private VaultItemListModel CreateVaultItemListModel(string id, IEnumerable<VaultItem> items, string accessRight,string returnUrl)
        {
            return new VaultItemListModel()
            {
                VaultId = id,
                ReturnUrl = returnUrl ?? CreateReturnUrl(),
                VaultItems = items,
                AccessRight = accessRight
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