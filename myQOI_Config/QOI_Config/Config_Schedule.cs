using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;
namespace QOI_Config
{
    public static class Config_Schedule
    {
        [FunctionName("Config_Schedule")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Client_Information Received");

            Schedule schedule = new Schedule();
            var str = Environment.GetEnvironmentVariable("QOI_SQL", EnvironmentVariableTarget.Process);
            using (SqlConnection conn = new SqlConnection(str))
            {
                conn.Open();
                var text = "select TOP 1 * from dbo.Schedule;";
                
                try
                {
                    SqlCommand cmd = new SqlCommand(text, conn);
                    // Execute the command and log the # rows affected.
                    SqlDataReader rows = cmd.ExecuteReader();
                    if (rows.Read()) 
                    {

                        schedule.FrequencyHr = rows["FrequencyHr"].ToString();
                        schedule.RunTime = rows["RunTime"].ToString();
                        
                    }
                  

                }
                catch (Exception e)
                {
                    log.LogError(e.ToString());
                    return new BadRequestResult();
                }
            }
            return new JsonResult(schedule);
        }
    }
    public class Schedule
    {
        public string FrequencyHr { get; set; }
        public string RunTime { get; set; }
    }

}

