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
    public class RoleAdminController : Controller
    {
        private AppIdentityDbContext IdentityContext => HttpContext.GetOwinContext().GetUserManager<AppIdentityDbContext>();

        private AppRoleManager RoleManager => HttpContext.GetOwinContext().GetUserManager<AppRoleManager>();

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
                    return RedirectToAction("Index");
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

        private void AddModelErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("",error);
            }
        }
    }
}