using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vault.Abstract;
using VaultDAL.Abstract;
using VaultDAL.Models;

namespace Vault.Concrete
{
    public class LogManager : ILogManager<VaultAccessLog>
    {
        private readonly IRepository<VaultAccessLog> _accessLogRepository;

        public LogManager(IRepository<VaultAccessLog> accessLogRepository)
        {
            _accessLogRepository = accessLogRepository;
        }

        public async Task<IEnumerable<VaultAccessLog>> ShowByDateLog(string vaultId, DateTime date)
        {
            var logs = await _accessLogRepository.GetListAsync();
            return logs.Where(x => x.VaultId == vaultId && x.DateTime.Date == date.Date).ToList();
        }

        public async Task<IEnumerable<VaultAccessLog>> ShowLog(string vaultId)
        {
            var logs = await _accessLogRepository.GetListAsync();
            return logs.Where(x => x.VaultId == vaultId).ToList();
        }
    }
}