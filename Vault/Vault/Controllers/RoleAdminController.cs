using System;
using System.Collections.Generic;
using System.Linq;
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
                var list = await IdentityContext.Roles.Find(x => true).ToListAsync();
                return View(list);
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


    }
}