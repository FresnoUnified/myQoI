using System;
using log4net;
using SpeedtestNetCli.Configurations;
using SpeedtestNetCli.Infrastructure;
using System.Threading;
using System.Net.NetworkInformation;
using System.Management;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SpeedtestNetCli.Services
{
    

    public interface ISpeedtestService
    {
    }

    public class SpeedtestService : ThreadedActionService, ISpeedtestService
    {
        
        private static readonly ILog Log = LogManager.GetLogger("Speedtest Service");
        public static Network_Results nTrafficResults = new Network_Results();
        //GeoCoordinateWatcher watcher = new GeoCoordinateWatcher();
        private readonly IBestServerDeterminer _bestServerDeterminer;
        private readonly IDownloadSpeedTester _downloadSpeedTester;
        private readonly IUploadSpeedTester _uploadSpeedTester;
        private readonly SpeedtestConfiguration _speedtestConfiguration;
        myQOIClass myQOIInfo = new myQOIClass();
        QOI_API_Helper qoi_api_helper = new QOI_API_Helper();
        GeoWatcher _geowatcher = new GeoWatcher();

        public SpeedtestService(
            IBestServerDeterminer bestServerDeterminer,
            IDownloadSpeedTester downloadSpeedTester,
            IUploadSpeedTester uploadSpeedTester,
            SpeedtestConfiguration speedtestConfiguration)
            : base(speedtestConfiguration)
        {
            _bestServerDeterminer = bestServerDeterminer;
            _downloadSpeedTester = downloadSpeedTester;
            _uploadSpeedTester = uploadSpeedTester;
            _speedtestConfiguration = speedtestConfiguration;
        }

        protected override void Run()
        {
          
            while (!_speedtestConfiguration.CancellationToken.IsCancellationRequested)
            {
                
                GetScreenResolutions();
                _speedtestConfiguration.getTimeSpan();
                _speedtestConfiguration.CancellationToken.WaitHandle.WaitOne(TimeSpan.FromMinutes(_speedtestConfiguration.IntervalMinutes));
                RunProgram();

            }
            
        }
        
       
        public void RunProgram()
        {
            //Randomize a sleep to create a buffer between 10 min  to 20 mins
            Random rnd = new Random();
            int bufferTime = rnd.Next(60, 600);
            int bufferTimeMili = bufferTime * 1000;
            Thread.Sleep(bufferTimeMili);
            TryRunSpeedTest();

        }
        private void TryRunSpeedTest()
        {
            try
            {
                var bestServer = _bestServerDeterminer.GetBestServer().Result;
                var downSpeedMbps = _downloadSpeedTester.GetSpeedMbps(bestServer);
                var upSpeedMbps = _uploadSpeedTester.GetSpeedMbps(bestServer);

                var latency = Convert.ToDouble(bestServer.Attribute("latency").Value);
                var server = bestServer.Attribute("host").Value;

                myQOIInfo.uploadSpeed = upSpeedMbps.ToString();
                myQOIInfo.latency = latency.ToString();
                myQOIInfo.downloadSpeed = downSpeedMbps.ToString();
                //Calling other methods
                GetComputerInfo();
                GetTimeStamps();
                //get Coord
                GeoWatcher.Result _result = _geowatcher.Coord();
                myQOIInfo.Lat = _result.coordLat;
                myQOIInfo.Long = _result.coordLong;
                myQOIInfo.ISP = BestServerDeterminer.ISP.ToString();
                myQOIInfo.ClientIP = BestServerDeterminer.Client_IP.ToString();
                qoi_api_helper.InitializeClient(myQOIInfo);


                Log.Info($"Latency: {latency:N2} Download Speed: {downSpeedMbps:N2} Upload Speed: {upSpeedMbps:N2} Server: {server}");
            }


            catch (Exception e)
            {
                Log.Error(e);
            }
        }
        private void GetComputerInfo()
        {
            NetworkInterface[] nicarray;
            nicarray = NetworkInterface.GetAllNetworkInterfaces();
            List<String> ListofActiveAdapters = new List<string>();


            //initialize the select query with command text
            SelectQuery query = new SelectQuery(@"Select * from Win32_ComputerSystem");

            //initialize the searcher with the query it is supposed to execute
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(query))
            {
                //execute the query
                foreach (ManagementObject process in searcher.Get())
                {
                    //print system info

                    process.Get();
                    myQOIInfo.make = process["Manufacturer"].ToString();
                    myQOIInfo.model = process["Model"].ToString();
                    myQOIInfo.computerUser = process["UserName"].ToString();
                    myQOIInfo.Hostname = String.Format("{0} {1}", process["DNSHostName"].ToString(), process["Domain"].ToString());

                }
            }
            //initialize the select query with command text
            SelectQuery BiosQuery = new SelectQuery(@"Select * from Win32_BIOS");

            //initialize the searcher with the query it is supposed to execute
            using (ManagementObjectSearcher Biossearcher = new ManagementObjectSearcher(BiosQuery))
            {
                //execute the query
                foreach (ManagementObject BiosProcess in Biossearcher.Get())
                {
                    //print system info

                    BiosProcess.Get();
                    myQOIInfo.serialNumber = BiosProcess["SerialNumber"].ToString();

                }
            }
            //initialize the select query with command text
            SelectQuery OSQuery = new SelectQuery(@"Select * from Win32_OperatingSystem");

            //initialize the searcher with the query it is supposed to execute
            using (ManagementObjectSearcher OSsearcher = new ManagementObjectSearcher(OSQuery))
            {
                //execute the query
                foreach (ManagementObject OSProcess in OSsearcher.Get())
                {
                    //print system info

                    OSProcess.Get();
                    myQOIInfo.OS = OSProcess["Caption"].ToString();
                    myQOIInfo.Arch = OSProcess["OSArchitecture"].ToString();

                }
            }

            foreach (NetworkInterface nicnac in nicarray)
            {

                if (nicnac.OperationalStatus.ToString() == "Up" && nicnac.GetIPv4Statistics().UnicastPacketsReceived >= 1)
                {
                    myQOIInfo.NetworkInterface = nicnac.NetworkInterfaceType.ToString();
                    UnicastIPAddressInformationCollection ipInfo = nicnac.GetIPProperties().UnicastAddresses;
                    foreach (UnicastIPAddressInformation item in ipInfo)
                    {
                        if (item.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            myQOIInfo.ipAddress = item.Address.ToString();
                                                 
                            break;
                        }
                    }

                    myQOIInfo.macAddress = nicnac.GetPhysicalAddress().ToString();
                    


                }
            }

            

        }
        private void GetTimeStamps()
        {
            //get timestamp
            myQOIInfo.timeDate = DateTime.Now.ToString("MM/dd/yyy");
            myQOIInfo.timeStamp = DateTime.Now.ToString("h:mm:ss tt");
            
        }
        private void GetScreenResolutions()
        {
            MonitorsClass[] monitorclass = new MonitorsClass[Screen.AllScreens.Length];
            for (int i = 0; i < Screen.AllScreens.Length; i++)
            {
                foreach (var screen in Screen.AllScreens)
                {
                    monitorclass[i] = new MonitorsClass();
                    monitorclass[i].Width = screen.Bounds.Width.ToString();
                    monitorclass[i].Height = screen.Bounds.Height.ToString();
                }
               
            }
            myQOIInfo.monitorclass = monitorclass;
        }

    }
}
