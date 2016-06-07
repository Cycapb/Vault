using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Vault.Abstract;
using Vault.Models;
using VaultDAL.Models;

namespace Vault.Controllers
{
    [Authorize(Roles = "VaultAdmins")]
    public class LogController : Controller
    {
        private readonly ILogManager<VaultAccessLog> _logManager;

        public LogController(ILogManager<VaultAccessLog> logManager)
        {
            _logManager = logManager;
        }

        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> VaultLog(WebUser user, string id, string name)
        {
            var vaultName = name;
            var events = await _logManager.ShowLog(id);
            var logModel = new VaultAccessLogModel()
            {
                Events = events,
                VaultName = vaultName
            };
            return View(logModel);
        }
    }
}