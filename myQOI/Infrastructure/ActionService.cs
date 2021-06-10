using System;
using System.ServiceProcess;
using log4net;
namespace SpeedtestNetCli.Infrastructure
{
    public class ActionService : ServiceBase
    {
        private static readonly ILog Log = LogManager.GetLogger("Speedtest Service");
     
        public Action StartAction { get; set; }
        public Action StopAction { get; set; }

        public ActionService()
        {
            AutoLog = true;
            CanStop = true;
        }

        protected override void OnStart(string[] args)
        {

            StartAction?.Invoke();
        }

        protected override void OnStop()
        {
            StopAction?.Invoke();
        }
       

    }
}
