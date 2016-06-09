using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VaultDAL.Abstract;
using VaultDAL.Models;

namespace VaultService.Models
{
    public class LogManager
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
    }
}
