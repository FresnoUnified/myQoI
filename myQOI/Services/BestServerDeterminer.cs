﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Xml.Linq;
using log4net;
using SpeedtestNetCli.Configurations;
using SpeedtestNetCli.Query;
using SpeedtestNetCli.Model;

namespace SpeedtestNetCli.Services
{
    
    public interface IBestServerDeterminer
    {
        Task<XElement> GetBestServer();
        Task<List<XElement>> GetClosestServers(int number);
    }

    public class BestServerDeterminer : IBestServerDeterminer
    {
        private readonly Func<IHttpQueryExecutor> _httpExecutor;
        private readonly SpeedtestConfiguration _speedtestConfiguration;
        private static string _ISP;
        private static string _Client_IP;
        public static string ISP { get { return _ISP; } set { _ISP = value; } }
        
        public static string Client_IP { get { return _Client_IP; } set { _Client_IP = value; } }

        private static readonly ILog Log = LogManager.GetLogger("Best server determiner");

        public BestServerDeterminer(Func<IHttpQueryExecutor> httpExecutor,
            SpeedtestConfiguration speedtestConfiguration)
        {
            _httpExecutor = httpExecutor;
            _speedtestConfiguration = speedtestConfiguration;
        }

        public async Task<List<XElement>> GetClosestServers(int number)
        {
            var client = await _httpExecutor().Execute(new SpeedtestConfigQuery());
            var servers = await _httpExecutor().Execute(new SpeedtestServerQuery());
            ISP = client.Descendants("client").First().Attribute("isp").Value;
            Client_IP = client.Descendants("client").First().Attribute("ip").Value;
            var clientLocation = new Location(client.Descendants("client").First());
            foreach (var server in servers.Descendants("server"))
            {
                server.Add(new XAttribute("clientDistance", clientLocation.DistanceTo(new Location(server))));
            }

            return servers.Descendants("server")
                .OrderBy(server => Convert.ToDouble(server.Attribute("clientDistance").Value))
                .Take(number)
                .ToList();
        }

        public async Task<XElement> GetBestServer()
        {
            return GetLowestLatencyServerFrom(await GetClosestServers(5));
        }

        private XElement GetLowestLatencyServerFrom(IList<XElement> closestServers)
        {
            //Log.Debug("Determining latency to closest servers");
            foreach (var server in closestServers)
            {
                var averageLatency = 0.0;
                for (var latencyIteration = 0; latencyIteration < 5; latencyIteration++)
                {
                    averageLatency += DetermineLatencyTo(server.Attribute("host").Value) / 5.0;
                }
                server.Add(new XAttribute("latency", averageLatency));
            }

            return closestServers.OrderBy(server => Convert.ToDouble(server.Attribute("latency").Value)).FirstOrDefault();
        }

        private long DetermineLatencyTo(string url)
        {
            using (var pingTest = new Ping())
            {
                try
                {
                    _speedtestConfiguration.CancellationToken.Register(() => pingTest.SendAsyncCancel());
                    var result = pingTest.SendPingAsync(new Uri($"http://{url}").Host, 3600).Result;
                    return result.RoundtripTime;
                }
                catch (PingException)
                {
                    return 3600;
                }
            }
        }
    }
}
