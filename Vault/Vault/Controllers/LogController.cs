using System.Linq;
using System.Threading.Tasks;
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
        private readonly int _itemsPerPage = 15;

        public LogController(ILogManager<VaultAccessLog> logManager)
        {
            _logManager = logManager;
        }

        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> VaultLog(WebUser user, string id, string name, int page = 1)
        {
            var vaultName = name;
            var events = (await _logManager.ShowLog(id)).ToList();
            var logModel = new VaultAccessLogModel()
            {
                Events = events.Skip((page - 1) * _itemsPerPage)
                    .Take(_itemsPerPage)
                    .ToList(),
                VaultName = vaultName,
                PagingInfo = new PagingInfo()
                {
                    CurrentPage = page,
                    ItemsPerPage = _itemsPerPage,
                    TotalItems = events.Count()
                }
            };
            return View(logModel);
        }
    }
}