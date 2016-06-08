using System;
using System.ServiceProcess;

namespace VaultService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
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
        //        // Put the body of your old Main method here.
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
