using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;
using System.Collections.Generic;
namespace QOI_API

{
    public static class Config_Schedule
    {
        [FunctionName("Config_Schedule")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Client_Information Received");

            Schedule[] allRecords = null;
            var str = Environment.GetEnvironmentVariable("QOI_SQL", EnvironmentVariableTarget.Process);
            using (SqlConnection conn = new SqlConnection(str))
            {
                var text = "select DISTINCT Runtime from dbo.MultiSchedule;";
                
                try
                {
                   
                    // Execute the command and log the # rows affected

                    using (SqlCommand cmd = new SqlCommand(text, conn))
                    {
                        conn.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            var list = new List<Schedule>();
                            while (reader.Read())
                                list.Add(new Schedule { RunTime = reader.GetString(0)});
                            allRecords = list.ToArray();
                        }
                    }
                  

                }
                catch (Exception e)
                {
                    log.LogError(e.ToString());
                    return new BadRequestResult();
                }
            }
            return new JsonResult(allRecords);
        }
    }
    public class Schedule
    {
        public string RunTime { get; set; }
    }

}

