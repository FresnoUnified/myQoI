using System.Threading;
using log4net;
using RestSharp;
using System.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpeedtestNetCli.Configurations
{
    public class SpeedtestConfiguration
    {

        public double IntervalMinutes { get; set; }
        private static readonly ILog Log = LogManager.GetLogger("Speedtest Service");


        public void getTimeSpan()
        {
            Get_Config();
            string RunTimes = ConfigurationManager.AppSettings.Get("RunTimes");
            // Pass in the time you want to start and the interval
            StartTimer(RunTimes);

        }


        public CancellationToken CancellationToken { get; set; }
        public void Get_Config()
        {
            List<Schedule_API_Class> schedule_api_class;

            try
            {
                var apikey = AzureAuth.GetVaultValue();
                var apiClients = new RestClient("API URL");
                var requests = new RestRequest(Method.GET);
                requests.AddHeader("Ocp-Apim-Subscription-Key", apikey);
                requests.AddHeader("Content-Type", "application/json");
                IRestResponse responses = apiClients.Execute(requests);
                schedule_api_class = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Schedule_API_Class>>(responses.Content);
                Update_App_Settings(schedule_api_class);
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
            }

        }
        private void Update_App_Settings(List<Schedule_API_Class> schedule_api_class)
        {
            var times = new List<TimeSpan>();
            foreach (Schedule_API_Class Time in schedule_api_class)
            {
                int aHrs = Int32.Parse(Time.RunTime.Substring(0, 2));
                int aMin = Int32.Parse(Time.RunTime.Substring(2, 2));
                times.Add(new TimeSpan(aHrs, aMin, 00));
            }

            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings["RunTimes"].Value = String.Join(";", times.ToArray()); ;

            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");

        }
        private void StartTimer(string RunTimes)
        {
            string AppSettingRunTimes = RunTimes;
            List<string> result = AppSettingRunTimes.Split(';').ToList();
            var times = new List<TimeSpan>();
            foreach (String Time in result)
            {
                int aHrs = Int32.Parse(Time.Split(':').First());
                int aMin = Int32.Parse(Time.Split(':').Last());
                times.Add(new TimeSpan(aHrs, aMin, 00));
            }
            Console.WriteLine(times);
            // Initialize timer
            double current = DateTime.Now.TimeOfDay.TotalMinutes;
            var closestTime = times.OrderBy(t => current < t.TotalMinutes ? Math.Abs((t.TotalMinutes - current)) : current).First();
            double scheduledTime = closestTime.TotalMinutes;
            TimeSpan timeBetweenEachRun = new TimeSpan(24, 00, 00);
            double intervalPeriod = timeBetweenEachRun.TotalMinutes;
            // calculates the first execution of the method, either its today at the scheduled time or tomorrow (if scheduled time has already occurred today)
            double firstExecution = current > scheduledTime ? intervalPeriod - (current - scheduledTime) : scheduledTime - current;

            IntervalMinutes = firstExecution;


        }

    }
}
