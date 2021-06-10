using System;
using log4net;
using Newtonsoft.Json;
using RestSharp;

namespace SpeedtestNetCli
{
    class QOI_API_Helper
    {
        private static readonly ILog Log = LogManager.GetLogger("Speedtest Service");
        
        public void InitializeClient(myQOIClass myQOIInfo)
        {
            //converting nicJson object to json
            var nicJson = JsonConvert.SerializeObject(myQOIInfo);
            //making a post call to the api with nicJson
            try
            {
                var apikey = AzureAuth.GetVaultValue();
                var apiClient = new RestClient("myQOI API URL");
                var request = new RestRequest(Method.POST);
                request.AddHeader("Ocp-Apim-Subscription-Key", apikey);
                request.AddHeader("Content-Type", "application/json");
                request.AddJsonBody(nicJson);
                IRestResponse response = apiClient.Execute(request);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
            Log.Info("Finished Program");
        }
    }
    
}
