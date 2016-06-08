using System.ServiceProcess;
using VaultDAL.Abstract;
using VaultDAL.Concrete;
using VaultDAL.Models;

namespace VaultService
{
    public partial class ReportingService : ServiceBase
    {
        private readonly IRepository<VaultAccessLog> _accessLogRepository;

        public ReportingService()
        {
            InitializeComponent();
            _accessLogRepository = new MongoRepository<VaultAccessLog>(new MongoConnectionProvider());
        }

        protected override void OnStart(string[] args)
        {

        }

        protected override void OnStop()
        {
        }
    }
}
