using System.Linq;
using Autofac;
using log4net.Config;
using SpeedtestNetCli.Configurations;
using SpeedtestNetCli.Infrastructure;
using SpeedtestNetCli.Services;
using CommandLine;
using log4net;

namespace SpeedtestNetCli
{
    internal class Program
    {
        private static readonly ILog Log = LogManager.GetLogger("Speedtest Service");
        private static void Main(string[] args)
        {
            Log.Info("Program Started");
            XmlConfigurator.Configure();
            var container = IocBuilder.Build();
            SetupCommandLineOptions(args, container);

            StartSpeedtest(container);


        }

        private static void SetupCommandLineOptions(string[] args, IComponentContext container)
        {
            var config = container.Resolve<SpeedtestConfiguration>();
            if (args.Any())
                Parser.Default.ParseArguments(args, config);
        }

        private static void StartSpeedtest(IComponentContext container)
        { 
            var speedtestService = container.Resolve<SpeedtestService>();
   
            var runner = new ActionServiceRunner(speedtestService);
            runner.Run();
            

        }
        

    }
}
