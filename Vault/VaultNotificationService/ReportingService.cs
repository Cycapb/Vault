using System;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Timers;
using VaultService.Models;

namespace VaultService
{
    public partial class ReportingService : ServiceBase
    {
        private Timer _timer;

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool SetServiceStatus(IntPtr handle, ref ServiceStatus serviceStatus);

        public enum ServiceState
        {
            SERVICE_STOPPED = 0x00000001,
            SERVICE_START_PENDING = 0x00000002,
            SERVICE_STOP_PENDING = 0x00000003,
            SERVICE_RUNNING = 0x00000004,
            SERVICE_CONTINUE_PENDING = 0x00000005,
            SERVICE_PAUSE_PENDING = 0x00000006,
            SERVICE_PAUSED = 0x00000007,
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ServiceStatus
        {
            public long dwServiceType;
            public ServiceState dwCurrentState;
            public long dwControlsAccepted;
            public long dwWin32ExitCode;
            public long dwServiceSpecificExitCode;
            public long dwCheckPoint;
            public long dwWaitHint;
        };

        public ReportingService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            ServiceStatus serviceStatus = new ServiceStatus();
            serviceStatus.dwCurrentState = ServiceState.SERVICE_START_PENDING;
            serviceStatus.dwWaitHint = 100000;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            _timer = new System.Timers.Timer();
            _timer.Interval = 3600000; 
            _timer.Elapsed += new System.Timers.ElapsedEventHandler(this.OnTimer);
            _timer.Start();

            serviceStatus.dwCurrentState = ServiceState.SERVICE_RUNNING;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);
        }

        protected override void OnStop()
        {
        }

        private async void OnTimer(object sender, EventArgs e)
        {
            //_timer.Stop();
            var hourOfTheDay = DateTime.Now.Hour;
            if (hourOfTheDay == 0)
            {
                var serviceHelper = new ServiceHelper();
                await serviceHelper.StartNotification(DateTime.Now.AddDays(-1));
            }
        }

        //internal void TestStartupAndStop(string[] args)
        //{
        //    this.OnStart(args);
        //    Console.ReadLine();
        //    this.OnStop();
        //}
    }
}
