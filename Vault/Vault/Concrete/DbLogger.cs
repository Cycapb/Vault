using System;
using System.Threading.Tasks;
using Vault.Abstract;
using Vault.Models;
using VaultDAL.Abstract;
using VaultDAL.Models;

namespace Vault.Concrete
{
    public class DbLogger:ILogger
    {
        private readonly IRepository<VaultAccessLog> _repository;
        public CreateLogModel LogModel { get; set; }

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
                EventType = this.LogModel.EventType,
                VaultId = this.LogModel.VaultId
            };
            await _repository.CreateAsync(logItem);
        }
    }
}