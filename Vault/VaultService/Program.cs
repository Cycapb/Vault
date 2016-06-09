using System.ServiceProcess;

namespace VaultService
{
    static class Program
    {
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new ReportingService()
            };
            ServiceBase.Run(ServicesToRun);
        }

        //static void Main(string[] args)
        //{
        //    if (Environment.UserInteractive)
        //    {
        //        ReportingService reportingService = new ReportingService();
        //        reportingService.TestStartupAndStop(args);
        //    }
        //    else
        //    {
        //        ServiceBase[] ServicesToRun;
        //        ServicesToRun = new ServiceBase[]
        //        {
        //            new ReportingService()
        //        };
        //        ServiceBase.Run(ServicesToRun);
        //    }
        //}
    }
}
