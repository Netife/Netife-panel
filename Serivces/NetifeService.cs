using Microsoft.Extensions.Configuration;
using NetifeMessage;
using NetifePanel.Contracts;
using NetifePanel.Interface;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;

namespace NetifePanel.Serivces
{
    public class NetifeService : INetifeService
    {

        public event EventHandler<NetifeDispatcherUpdate> OnOutputUpdate;

        private static ConcurrentBag<Process> _processes = new();

        private IConfiguration _configuration;

        private ConcurrentBag<Func<NetifeProbeRequest, NetifeProbeRequest>> _netifeDealPackets = new();

        public NetifeService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> StartBasicService()
        {
            var config = _configuration.GetRequiredSection("component").GetRequiredSection("lauchSettings");
            //Netife JsRemote StartUp
            _ = Task.Run(() =>
                   {
                        Process process = new Process();
                        process.StartInfo.FileName = Path.Combine(ApplicationData.Current.LocalFolder.Path, "bin", "NetifeJsRemote.exe");
                        process.StartInfo.Arguments = $"{config.GetRequiredSection("JsRemote")["Host"]} {config.GetRequiredSection("JsRemote")["Port"]}";
                        process.StartInfo.CreateNoWindow = true;
                        process.Start();
                        _processes.Add(process);
                        ChildProcessTracker.AddProcess(process);
                   });

            _ = Task.Run(() =>
                  {
                      Process process = new Process();
                      process.StartInfo.FileName = Path.Combine(ApplicationData.Current.LocalFolder.Path, "bin", "NetifeDispatcher.exe");
                      process.StartInfo.Arguments = $"{config.GetRequiredSection("Dispatcher")["Host"]} {config.GetRequiredSection("Dispatcher")["Port"]} " +
                                                    $"{config.GetRequiredSection("Frontend")["Host"]} {config.GetRequiredSection("Frontend")["Port"]} " +
                                                    $"{config.GetRequiredSection("JsRemote")["Host"]} {config.GetRequiredSection("JsRemote")["Port"]}";
                      process.StartInfo.CreateNoWindow = true;
                      process.StartInfo.RedirectStandardOutput = true;
                      process.Start();
                      _processes.Add(process);
                      ChildProcessTracker.AddProcess(process);
                      StreamReader reader = process.StandardOutput;
                      string line = reader.ReadLine();
                      while (true)
                      {
                          // execute event for other service to recieve the output of the Netife Dispatcher
                          if (string.IsNullOrEmpty(line))
                          {
                              Thread.Sleep(100);
                          }
                          var args = new NetifeDispatcherUpdate(line);
                          while (true)
                          {
                              if (OnOutputUpdate != null)
                              {
                                  break;
                              }
                              Thread.Sleep(1000);
                          }
                          OnOutputUpdate?.Invoke(this, args);
                          line = reader.ReadLine();
                          Thread.Sleep(10);
                      }
                  });

            _ = Task.Run(() =>
                  {
                        Process process = new Process();
                        process.StartInfo.FileName = Path.Combine(ApplicationData.Current.LocalFolder.Path, "bin", "NetifeProbe.exe");
                        process.StartInfo.Arguments = $"";
                        process.StartInfo.CreateNoWindow = true;
                        process.Start();
                        _processes.Add(process);
                        ChildProcessTracker.AddProcess(process);
                  });

            return true;
        }

        /// <summary>
        /// Dispatch Netife Packet to other component
        /// </summary>
        /// <param name="action"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void ConfigureNetifePacketEvents(Func<NetifeProbeRequest, NetifeProbeRequest> action)
        {
            _netifeDealPackets.Add(action);
        }
        /// <summary>
        /// Invoke registed triggers
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<NetifeProbeResponse> InvokeTriggers(NetifeProbeRequest request)
        {
            foreach (var sp in _netifeDealPackets)
            {
                request = sp(request);
            }
            var res = new NetifeProbeResponse();
            res.Uuid = request.Uuid;
            res.DstIpAddr = request.DstIpAddr;
            res.DstIpPort = request.DstIpPort;
            res.ResponseText = request.RawText;
            return Task.FromResult(res);
        }
    }
}
