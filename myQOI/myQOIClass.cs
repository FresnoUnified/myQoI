using Microsoft.Win32;


namespace SpeedtestNetCli
{
    class myQOIClass
    {
        public string ipAddress;
        public string macAddress;
        public string make;
        public string model;
        public string serialNumber;
        public string downloadSpeed;
        public string uploadSpeed;
        public string latency;
        public string timeDate;
        public string timeStamp;
        public string computerUser;
        public string Hostname;
        public string OS;
        public string Arch;
        public string IEVer = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Internet Explorer").GetValue("Version").ToString();
        public MonitorsClass[] monitorclass;
        public double Lat;
        public double Long;
        public string ISP;
        public string ClientIP;
        public string NetworkInterface;
    }
}
