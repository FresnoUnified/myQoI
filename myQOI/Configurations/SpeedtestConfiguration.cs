using System.Threading;
using log4net;
using RestSharp;
using System.Configuration;
using System;

namespace SpeedtestNetCli.Configurations
{
    public class SpeedtestConfiguration
    {
        
        public double IntervalMinutes { get; set; }
        private static readonly ILog Log = LogManager.GetLogger("Speedtest Service");


        public void getTimeSpan()
        {
            Get_Config();
            string hr = ConfigurationManager.AppSettings.Get("hr");
            string min = ConfigurationManager.AppSettings.Get("min");
            string sec = ConfigurationManager.AppSettings.Get("sec");
            string dayhr = ConfigurationManager.AppSettings.Get("dayhr");
            string daymin = ConfigurationManager.AppSettings.Get("daymin");
            string daysec = ConfigurationManager.AppSettings.Get("daysec");
            Scheduler scheduler = new Scheduler(hr, min, sec, dayhr, daymin, daysec);
            // Pass in the time you want to start and the interval
            StartTimer(new TimeSpan(scheduler.hr, scheduler.min, scheduler.sec), new TimeSpan(scheduler.dayhr, scheduler.daymin, scheduler.daysec));

           
        }
       

        public CancellationToken CancellationToken { get; set; }
        public void Get_Config()
        {

            try
            {
                var apikey = AzureAuth.GetVaultValue();
                //Input the myQOI_Config url below
                var apiClients = new RestClient("API CONFIG URL");
                var requests = new RestRequest(Method.GET);
                requests.AddHeader("Ocp-Apim-Subscription-Key", apikey);
                requests.AddHeader("Content-Type", "application/json");
                IRestResponse responses = apiClients.Execute(requests);
                Schedule_API_Class schedule_api_class = Newtonsoft.Json.JsonConvert.DeserializeObject<Schedule_API_Class>(responses.Content);
                Update_App_Settings(schedule_api_class.FrequencyHr, schedule_api_class.RunTime);
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
            }

        }
        private void Update_App_Settings(string aFrequencyHr, string aTimeRun)
        {
            string aDayHr = aFrequencyHr;
            string aHrs = aTimeRun.Substring(0, 2);
            string aMin = aTimeRun.Substring(2, 2);
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings["hr"].Value = aHrs;
            config.AppSettings.Settings["min"].Value = aMin;
            config.AppSettings.Settings["dayhr"].Value = aDayHr;
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");

        }
        private void StartTimer(TimeSpan scheduledRunTime, TimeSpan timeBetweenEachRun)
        { 
            // Initialize timer
            double current = DateTime.Now.TimeOfDay.TotalMinutes;
            double scheduledTime = scheduledRunTime.TotalMinutes;
            double intervalPeriod = timeBetweenEachRun.TotalMinutes;
            // calculates the first execution of the method, either its today at the scheduled time or tomorrow (if scheduled time has already occurred today)
            double firstExecution = current > scheduledTime ? intervalPeriod - (current - scheduledTime) : scheduledTime - current;
            IntervalMinutes = firstExecution;
        }

    }
}
