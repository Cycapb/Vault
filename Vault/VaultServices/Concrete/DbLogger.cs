using System;
using System.Threading.Tasks;
using VaultDAL.Abstract;
using VaultDAL.Models;
using VaultServices.Abstract;

namespace VaultServices.Concrete
{
    public class DbLogger:IDbLogger
    {
        private readonly IRepository<VaultAccessLog> _repository;

        public string EventType { get; set; }
        public string VaultId { get; set; }

        public DbLogger(IRepository<VaultAccessLog> repository)
        {
            _repository = repository;
        }

        public async Task Log(string message)
        {
            await SaveToDb(message);
        }

        private async Task SaveToDb(string message)
        {
            var logItem = new VaultAccessLog()
            {
                DateTime = DateTime.Now,
                Event = message,
                EventType = this.EventType,
                VaultId = this.VaultId
            };
            await _repository.CreateAsync(logItem);
        }
    }
}