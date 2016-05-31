using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vault.Abstract;
using Vault.Models;

namespace Vault.Controllers
{
    public class VaultController : Controller
    {
        private readonly IVaultHelper _vaultHelper;

        public VaultController(IVaultHelper vaultHelper)
        {
            _vaultHelper = vaultHelper;
        }

        public ActionResult Index()
        {
            return RedirectToAction("VaultList");
        }

        public ActionResult VaultList(WebUser user)
        {
            return View();
        }
    }
}