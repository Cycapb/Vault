using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VaultDAL.Abstract;
using VaultDAL.Concrete;
using VaultDAL.Models;

namespace VaultService.Models
{
    public class LogManager
    {
        private readonly IRepository<VaultAccessLog> _accessLogRepository;

        public LogManager()
        {
            _accessLogRepository = new MongoRepository<VaultAccessLog>(new MongoConnectionProvider());
        }

        public async Task<IEnumerable<VaultAccessLog>> ShowByDateLog(string vaultId, DateTime date)
        {
            var logs = await _accessLogRepository.GetListAsync();
            return logs.Where(x => x.VaultId == vaultId && x.DateTime.Date == date.Date).ToList();
        }
    }
}
