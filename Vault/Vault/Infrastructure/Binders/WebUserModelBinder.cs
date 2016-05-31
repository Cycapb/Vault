using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Vault.Models;
using IModelBinder = System.Web.Mvc.IModelBinder;
using ModelBindingContext = System.Web.Mvc.ModelBindingContext;

namespace Vault.Infrastructure.Binders
{
    public class WebUserModelBinder:IModelBinder
    {
        private AppUserManager UserManager => HttpContext.Current.GetOwinContext().GetUserManager<AppUserManager>();

        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var appUser = UserManager.FindById(controllerContext.HttpContext.User.Identity.GetUserId());
            var webUser = new WebUser()
            {
                Id = appUser.Id,
                UserName = appUser.UserName
            };
            return webUser;
        }
    }
}