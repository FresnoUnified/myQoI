using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Data.SqlClient;
namespace QOI_API
{
    public static class QOI_HTTP
    {
        [FunctionName("QOI_HTTP")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Client_Information Received");

            

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject<Client_Information>(requestBody);


            string s = data.IPAddress;
            log.LogInformation(s);

            var str = Environment.GetEnvironmentVariable("QOI_SQL", EnvironmentVariableTarget.Process);
            using (SqlConnection conn = new SqlConnection(str))
            {
                conn.Open();
                var text = "INSERT INTO dbo.Client_Info (IPAddress, MacAddress, Make, Model, SerialNumber, DownloadSpeed, UploadSpeed, Latency, timeDate,  timeStamp, computerUser, Hostname, OS, Arch, IEVer, Monitors, Lat, Long, ISP, ClientIP, NetworkInterface) " +
                        "VALUES (@IPAddress, @MacAddress, @Make, @Model, @SerialNumber, @DownloadSpeed, @UploadSpeed, @Latency, @timeDate, @timeStamp, @computerUser, @Hostname, @OS, @Arch, @IEVer, @Monitors, @Lat, @Long, @ISP, @ClientIP, @NetworkInterface);";
                try
                {
                    using (SqlCommand cmd = new SqlCommand(text, conn))
                    {
                        cmd.Parameters.AddWithValue("@IPAddress", data.IPAddress);
                        cmd.Parameters.AddWithValue("@MacAddress", data.MacAddress);
                        cmd.Parameters.AddWithValue("@Make", data.Make);
                        cmd.Parameters.AddWithValue("@Model", data.Model);
                        cmd.Parameters.AddWithValue("@SerialNumber", data.SerialNumber);
                        cmd.Parameters.AddWithValue("@DownloadSpeed", data.DownloadSpeed);
                        cmd.Parameters.AddWithValue("@UploadSpeed", data.UploadSpeed);
                        cmd.Parameters.AddWithValue("@Latency", data.Latency);
                        cmd.Parameters.AddWithValue("@timeDate", data.timeDate);
                        cmd.Parameters.AddWithValue("@timeStamp", data.timeStamp);
                        cmd.Parameters.AddWithValue("@computerUser", data.computerUser);
                        cmd.Parameters.AddWithValue("@Hostname", data.Hostname);
                        cmd.Parameters.AddWithValue("@OS", data.OS);
                        cmd.Parameters.AddWithValue("@Arch", data.Arch);
                        cmd.Parameters.AddWithValue("@IEVer", data.IEVer);
                        cmd.Parameters.AddWithValue("@Monitors", JsonConvert.SerializeObject(data.monitorclass));
                        cmd.Parameters.AddWithValue("@Lat", data.Lat);
                        cmd.Parameters.AddWithValue("@Long", data.Long);
                        cmd.Parameters.AddWithValue("@ISP", data.ISP);
                        cmd.Parameters.AddWithValue("@ClientIP", data.ClientIP);
                        cmd.Parameters.AddWithValue("@NetworkInterface", data.NetworkInterface);
                        // Execute the command and log the # rows affected.
                        var rows = await cmd.ExecuteNonQueryAsync();
                        log.LogInformation($"{rows} rows were updated {data}");
                    }
                }
                catch(Exception e)
                {
                    log.LogError(e.ToString());
                    return new BadRequestResult();
                }
            }
            return new OkObjectResult(data);
        }





        
        
    }

      
    public class Client_Information
    {
        public string IPAddress { get; set; }
        public string MacAddress { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public string SerialNumber { get; set; }
        public string DownloadSpeed { get; set; }
        public string UploadSpeed { get; set; }
        public string Latency { get; set; }
        public DateTime timeDate { get; set; }
        public string timeStamp { get; set; }
        public string computerUser { get; set; }
        public string Hostname { get; set; }
        public string OS { get; set; }
        public string Arch { get; set; }
        public string IEVer { get; set; }
        public string ISP { get; set; }
        public string ClientIP { get; set; }
        public string NetworkInterface { get; set; }
        
        public MonitorsClass[] monitorclass { get; set; }
        public double Lat;
        public double Long;





    }
    public class MonitorsClass
    {
        public string Width;
        public string Height;
    }

}

