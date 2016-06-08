using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
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
