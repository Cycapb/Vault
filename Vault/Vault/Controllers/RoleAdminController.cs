using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using MongoDB.Driver;
using Vault.Infrastructure;
using Vault.Models;

namespace Vault.Controllers
{
    [Authorize(Roles = "Administrators")]
    public class RoleAdminController : Controller
    {
        private AppIdentityDbContext IdentityContext => HttpContext.GetOwinContext().GetUserManager<AppIdentityDbContext>();
        private AppRoleManager RoleManager => HttpContext.GetOwinContext().GetUserManager<AppRoleManager>();
        private AppUserManager UserManager => HttpContext.GetOwinContext().GetUserManager<AppUserManager>();

        public async Task<ActionResult> Index()
        {
            try
            {
                var roleList = await IdentityContext.Roles.Find(x => true).ToListAsync();
                return View(roleList);
            }
            catch (Exception)
            {
                return View("Error", new string[] {"No connection to database"});
            }
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create([Required(ErrorMessage = "Role name can't be empty")]string name)
        {
            if (ModelState.IsValid)
            {
                var result = await RoleManager.CreateAsync(new AppRoleModel(name));
                if (result.Succeeded)
                {
                    TempData["message"] = $"Role {name} has been successfully created";
                    return RedirectToAction("Index");
                }
                else
                {
                    AddModelErrors(result);
                }
            }
            return View((object)name);
        }

        [HttpPost]
        public async Task<ActionResult> Delete(string id)
        {
            var role = await RoleManager.FindByIdAsync(id);
            if (role != null)
            {
                var result = await RoleManager.DeleteAsync(role);
                if (result.Succeeded)
                {
                    return await DeleteRoleFromUsers(role.Name);
                }
                else
                {
                    return View("Error",result.Errors);
                }
            }
            else
            {
                return View("Error", new string[] {"No role with such Id"});
            }
        }

        private async Task<ActionResult> DeleteRoleFromUsers(string roleName)
        {
            var users = await IdentityContext.Users.Find(x => x.Roles.Contains(roleName)).ToListAsync();
            foreach (var user in users)
            {
                var result = await UserManager.RemoveFromRoleAsync(user.Id, roleName);
                if (!result.Succeeded)
                {
                    return View("Error", result.Errors);
                }
            }
            TempData["message"] = $"Role {roleName} has been successfully deleted";
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Edit(string id)
        {
            var role = await RoleManager.FindByIdAsync(id);
            if (role != null)
            {
                var usersInRole = await IdentityContext.Users.Find(x => x.Roles.Contains(role.Name)).ToListAsync();
                var usersNotInRole = await IdentityContext.Users.Find(x => !x.Roles.Contains(role.Name)).ToListAsync();
                var editModel = new EditRolemodel()
                {
                    Role = role,
                    Members = usersInRole,
                    NonMembers = usersNotInRole
                };
                return View(editModel);
            }
            else
            {
                return View("Index");
            }
        }

        [HttpPost]
        public async Task<ActionResult> Edit(RoleModificationModel model)
        {
            IdentityResult result = null;
            if (model.UsersToAdd != null)
            {
                foreach (var user in model.UsersToAdd)
                {
                    result = await UserManager.AddToRoleAsync(user, model.RoleName);
                    if (!result.Succeeded)
                    {
                        return View("Error", result.Errors);
                    }
                } 
            }

            if (model.UsersToDelete != null)
            {
                foreach (var user in model.UsersToDelete)
                {
                    result = await UserManager.RemoveFromRoleAsync(user, model.RoleName);
                    if (!result.Succeeded)
                    {
                        return View("Error", result.Errors);
                    }
                }
            }
            TempData["message"] = $"Role {model.RoleName} has been successfully updated";
            return RedirectToAction("Index");
        }

        private void AddModelErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("",error);
            }
        }
    }
}