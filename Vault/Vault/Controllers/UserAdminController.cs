using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using MongoDB.Driver;
using Vault.Abstract;
using Vault.Infrastructure;
using Vault.Models;

namespace Vault.Controllers
{
    [Authorize(Roles = "Administrators")]
    public class UserAdminController : Controller
    {
        private AppUserManager UserManager => HttpContext.GetOwinContext().GetUserManager<AppUserManager>();

        private AppIdentityDbContext IdentityContext
            => HttpContext.GetOwinContext().GetUserManager<AppIdentityDbContext>();

        private readonly IVaultManager _vaultManager;

        public UserAdminController(IVaultManager vaultManager)
        {
            _vaultManager = vaultManager;
        }

        public async Task<ActionResult> Index()
        {
            try
            {
                var list = await IdentityContext.Users.Find(x => true).ToListAsync();
                return View(list);
            }
            catch (Exception)
            {
                return View("Error",new string[] {"No connection to database"});
            }
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(CreateModel userModel)
        {
            if (ModelState.IsValid)
            {
                var newUser = new AppUserModel() {UserName = userModel.Name, Email = userModel.Email};
                var result = await UserManager.CreateAsync(newUser, userModel.Password);
                if (result.Succeeded)
                {
                    TempData["message"] = $"New user with login {userModel.Name} has been successfully created";
                   return RedirectToAction("Index");
                }
                else
                {
                    AddModelErrors(result);
                }
            }
            return View(userModel);
        }

        [HttpPost]
        public async Task<ActionResult> Delete(string id)
        {
            var userToDel = await UserManager.FindByIdAsync(id);
            if (userToDel != null)
            {
                var result = await UserManager.DeleteAsync(userToDel);
                if (result.Succeeded)
                {
                    await _vaultManager.DeleteVaultsByUser(userToDel.Id);
                    TempData["message"] = $"User with login {userToDel.UserName} has been successfully deleted";
                    return RedirectToAction("Index");
                }
                else
                {
                    AddModelErrors(result);
                    return View("Error", result.Errors);
                }
            }
            else
            {
                return View("Error", new string[] {"User not found"});
            }
        }

        public async Task<ActionResult> Edit(string id)
        {
            var user = await UserManager.FindByIdAsync(id);
            if (user != null)
            {
                var editModel = new EditModel()
                {
                    Id = user.Id,
                    Name = user.UserName,
                    Email = user.Email,
                    Password = string.Empty
                };
                return View(editModel);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<ActionResult> Edit(EditModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByIdAsync(model.Id);
                if (user != null)
                {
                    user.Email = model.Email;
                    IdentityResult validEmail = await UserManager.UserValidator.ValidateAsync(user);
                    if (!validEmail.Succeeded)
                    {
                        AddModelErrors(validEmail);
                    }

                    user.UserName = model.Name;
                    IdentityResult validName = await UserManager.UserValidator.ValidateAsync(user);
                    if (!validName.Succeeded)
                    {
                        AddModelErrors(validName);
                    }

                    IdentityResult validPassword = null;
                    if (model.Password != null)
                    {
                        validPassword = await UserManager.PasswordValidator.ValidateAsync(model.Password);
                        if (validPassword.Succeeded)
                        {
                            user.PasswordHash = UserManager.PasswordHasher.HashPassword(model.Password);
                        }
                        else
                        {
                            AddModelErrors(validPassword);
                        }
                    }

                    if ((validPassword == null && validEmail.Succeeded && validName.Succeeded) || 
                        (validName.Succeeded && validEmail.Succeeded && model.Password != null && 
                        validPassword.Succeeded))
                    {
                        var result = await UserManager.UpdateAsync(user);
                        if (result.Succeeded)
                        {
                            TempData["message"] = $"User with login {user.UserName} has been successfully updated";
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            AddModelErrors(result);
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("","User not found");
                }
            }
            return View(model);
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