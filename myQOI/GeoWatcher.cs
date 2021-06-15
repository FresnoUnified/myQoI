using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Device.Location;
using System.Threading;
using Microsoft.Win32;
using log4net;
namespace SpeedtestNetCli
{
    class GeoWatcher
    {
        public GeoWatcher()
        {
            coordLat = 0.0;
            coordLong = 0.0;

        }
        public double coordLat { get; set; }
        public double coordLong { get; set; }
        private string SensorStatePath = "SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Sensor\\Overrides\\{BFA794E4-F964-4FDB-90F6-51056BFE4B44}";
        private string LocationSvcCfgPath = "SYSTEM\\CurrentControlSet\\Services\\lfsvc\\Service\\Configuration";
        private string MachineConsentPath = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\CapabilityAccessManager\\ConsentStore\\location";
        //Need to append UserConstentPath with Current user
        //private static string user = WindowsIdentity.GetCurrent().User.ToString();
        private string UserConsentPath = "Software\\Microsoft\\Windows\\CurrentVersion\\CapabilityAccessManager\\ConsentStore\\location";
        private static readonly ILog Log = LogManager.GetLogger("Speedtest Service");
        public struct Result
        {
            public double coordLat;
            public double coordLong;
        }
        public struct RegSettings
        {
            public int SensorStateSetting;
            public int LocationSvcCfgSetting;
            public string MachineConsentSetting;
            public string UserConsentSetting;

        }
        GeoCoordinateWatcher _watcher;
        public Result Coord()
        {
            // Need to get the state of the three reg keys then modify them to be enabled. Then restore settings

            _watcher = new GeoCoordinateWatcher();
            int _watcherTimeOut = 6000;

            var regkeysettings = new RegSettings()
            {
                UserConsentSetting = GetRegistryValuesCU(UserConsentPath, "Value").ToString(),
                SensorStateSetting = Convert.ToInt32(GetRegistryValuesLM(SensorStatePath, "SensorPermissionState")),
                LocationSvcCfgSetting = Convert.ToInt32(GetRegistryValuesLM(LocationSvcCfgPath, "Status")),
                MachineConsentSetting = GetRegistryValuesLM(MachineConsentPath, "Value").ToString()
                
            };
            // Change Registry values to ensure Geowatcher is able to get coordinates
            PrepareGeo();

            _watcher.TryStart(true, TimeSpan.FromMilliseconds(6000));
            while (_watcher.Status.ToString() != "Ready" && _watcherTimeOut > 0)
            {
                Thread.Sleep(TimeSpan.FromMilliseconds(_watcherTimeOut));
                _watcherTimeOut -= 100;
            }

            var coord = _watcher.Position.Location;
            var result = new Result()
            {
                coordLat = coord.Latitude,
                coordLong = coord.Longitude
            };

            //Restoring original Reg Values
            UpdateRegKeyLM(SensorStatePath, "SensorPermissionState", regkeysettings.SensorStateSetting);
            UpdateRegKeyLM(LocationSvcCfgPath, "Status", regkeysettings.LocationSvcCfgSetting);
            UpdateRegKeyLM(MachineConsentPath, "Value", regkeysettings.MachineConsentSetting);
            UpdateRegKeyCU(UserConsentPath, "Value", regkeysettings.UserConsentSetting);


            return result;

        }
        private void PrepareGeo()
        {
            //Need to get current owner
            //Get User SID
            //get Reg key vaules stored into an object

            //Updating Reg Key values
            UpdateRegKeyLM(SensorStatePath, "SensorPermissionState", 1);
            UpdateRegKeyLM(LocationSvcCfgPath, "Status", 1);
            UpdateRegKeyLM(MachineConsentPath, "Value", "Allow");
            UpdateRegKeyCU(UserConsentPath, "Value", "Allow");
            
        }
        
        private void UpdateRegKeyLM(string Path, string attribue, object Value )
        {
            using (RegistryKey myKey = Registry.LocalMachine.OpenSubKey(Path, true))
            {
                try
                {
                    myKey.SetValue(attribue, Value);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }

            }

        }
        private void UpdateRegKeyCU(string Path, string attribue, object Value)
        {
            using (RegistryKey myKey = Registry.CurrentUser.OpenSubKey(Path, true))
            {
                try
                {
                    myKey.SetValue(attribue, Value);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }

            }

        }
        private object GetRegistryValuesLM(string Path, string attribue)
        {
            var attr = new object();
            try
            {
                using (RegistryKey myKey = Registry.LocalMachine.OpenSubKey(Path))
                {


                    attr= myKey.GetValue(attribue);
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
            return attr;

        }
        private object GetRegistryValuesCU(string Path, string attribue)
        {
            var attr = new object();
            try
            {
                using (RegistryKey myKey = Registry.CurrentUser.OpenSubKey(Path))
                {

                    attr =  myKey.GetValue(attribue);
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
            return attr;

        }
    }
}
