﻿using System.Web.Mvc;

namespace Vault.Controllers
{
    [Authorize(Roles = "Administrators")]
    public class AdminController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}