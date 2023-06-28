using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using NetifeMessage;
using NetifePanel.Contracts;
using NetifePanel.Interface;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Media.Protection.PlayReady;
using Windows.Storage;
using static NetifeMessage.NetifeService;

namespace NetifePanel.Serivces
{
    public class NetifeService : INetifeService
    {

        public event EventHandler<NetifeDispatcherUpdate> OnOutputUpdate;

        private static ConcurrentBag<Process> _processes = new();

        private Process currentProbeProcess = null;

        private IConfiguration _configuration;

        private ConcurrentBag<Func<NetifeProbeRequest, NetifeProbeRequest>> _netifeDealPackets = new();

        private NetifeServiceClient _client;

        public NetifeService(IConfiguration configuration)
        {
            _configuration = configuration;

            var config = _configuration.GetRequiredSection("component").GetRequiredSection("lauchSettings");
            _client =
                new NetifeServiceClient(
                    GrpcChannel.ForAddress(config.GetRequiredSection("Dispatcher")["Host"].Replace("0.0.0.0", "http://localhost") + 
                    ":" + config.GetRequiredSection("Dispatcher")["Port"]));
        }

        public async Task CloseProbeService()
        {
            if (currentProbeProcess != null)
            {
                currentProbeProcess.Kill(true);
                await currentProbeProcess.WaitForExitAsync();
                currentProbeProcess = null;
            }

            var processes = Process.GetProcessesByName("NetifeProbe.exe");
            if (processes.Length != 0)
            {
                foreach (var process in processes)
                {
                    process.Kill(true);
                    await process.WaitForExitAsync();
                }
            }
        }

        public async Task StartProbeService()
        {
            _ = Task.Run(() =>
            {
                Process process = new Process();
                process.StartInfo.FileName = "powershell.exe";
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardInput = true;
                process.Start();
                process.StandardInput.WriteLine("cd " + ApplicationData.Current.LocalFolder.Path);
                process.StandardInput.WriteLine(Path.Combine("bin", "NetifeProbe.exe"));
                _processes.Add(process);
                currentProbeProcess = process;
                ChildProcessTracker.AddProcess(process);
            });
        }

        public async Task<bool> StartBasicService()
        {
            var config = _configuration.GetRequiredSection("component").GetRequiredSection("lauchSettings");
            //Netife JsRemote StartUp
            _ = Task.Run(() =>
                   {
                       //Process process = new Process();
                       //process.StartInfo.FileName = Path.Combine(ApplicationData.Current.LocalFolder.Path, "bin", "NetifeJsRemote.exe");
                       //process.StartInfo.Arguments = $"{config.GetRequiredSection("JsRemote")["Host"]} {config.GetRequiredSection("JsRemote")["Port"]}";
                       //process.StartInfo.CreateNoWindow = true;
                       //process.Start();
                       //_processes.Add(process);
                       Process process = new Process();
                       process.StartInfo.FileName = "powershell.exe";
                       process.StartInfo.CreateNoWindow = true;
                       process.StartInfo.RedirectStandardOutput = true;
                       process.StartInfo.RedirectStandardInput = true;
                       process.Start();
                       process.StandardInput.WriteLine("cd " + ApplicationData.Current.LocalFolder.Path);
                       process.StandardInput.WriteLine(Path.Combine("bin", "NetifeJsRemote.exe") + " " + $"{config.GetRequiredSection("JsRemote")["Host"]} {config.GetRequiredSection("JsRemote")["Port"]}");
                       _processes.Add(process);
                       ChildProcessTracker.AddProcess(process);
                   });

            _ = Task.Run(() =>
                  {
                      //Process process = new Process();
                      //process.StartInfo.FileName = Path.Combine(ApplicationData.Current.LocalFolder.Path, "bin", "NetifeDispatcher.exe");
                      //process.StartInfo.Arguments = $"{config.GetRequiredSection("Dispatcher")["Host"]} {config.GetRequiredSection("Dispatcher")["Port"]} " +
                      //                              $"{config.GetRequiredSection("Frontend")["Host"]} {config.GetRequiredSection("Frontend")["Port"]} " +
                      //                              $"{config.GetRequiredSection("JsRemote")["Host"]} {config.GetRequiredSection("JsRemote")["Port"]}";
                      //process.StartInfo.CreateNoWindow = true;
                      //process.StartInfo.RedirectStandardOutput = true;
                      //process.Start();
                      //_processes.Add(process);

                      Process process = new Process();
                      process.StartInfo.FileName = "powershell.exe";
                      process.StartInfo.CreateNoWindow = true;
                      process.StartInfo.RedirectStandardOutput = true;
                      process.StartInfo.RedirectStandardInput = true;
                      process.Start();
                      process.StandardInput.WriteLine("cd " + ApplicationData.Current.LocalFolder.Path);
                      process.StandardInput.WriteLine(Path.Combine("bin", "NetifeDispatcher.exe") + " " + $"{config.GetRequiredSection("Dispatcher")["Host"]} {config.GetRequiredSection("Dispatcher")["Port"]} " +
                                                           $"{config.GetRequiredSection("Frontend")["Host"]} {config.GetRequiredSection("Frontend")["Port"]} " +
                                                           $"{config.GetRequiredSection("JsRemote")["Host"]} {config.GetRequiredSection("JsRemote")["Port"]}");
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
                      //Process process = new Process();
                      //process.StartInfo.FileName = Path.Combine(ApplicationData.Current.LocalFolder.Path, "bin", "NetifeProbe.exe");
                      //process.StartInfo.Arguments = $"";
                      //process.StartInfo.CreateNoWindow = true;
                      //process.Start();
                      //_processes.Add(process);
                      //currentProbeProcess = process;
                      //ChildProcessTracker.AddProcess(process);
                      Process process = new Process();
                      process.StartInfo.FileName = "powershell.exe";
                      process.StartInfo.CreateNoWindow = true;
                      process.StartInfo.RedirectStandardOutput = true;
                      process.StartInfo.RedirectStandardInput = true;
                      process.Start();
                      process.StandardInput.WriteLine("cd " + ApplicationData.Current.LocalFolder.Path);
                      process.StandardInput.WriteLine(Path.Combine("bin", "NetifeProbe.exe"));
                      _processes.Add(process);
                      currentProbeProcess = process;
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

        public string QueryRemoteCommand(string rawCommand)
        {
            var request = new NetifePluginCommandRequest();

            var paras = SplitWithoutBlank(rawCommand);

            request.CommandPrefix = paras[0];
            request.Params.AddRange(paras);
            
            NetifePluginCommandResponse? res;
            try
            {
                res = _client.Command(request);
            }
            catch (Exception e)
            {
                return "Error AS:\n" + e.Message;
            }

            if (res.Status)
            {
                return res.Result;
            }
            return "(No Reply)";
        }

        private static List<string> SplitWithoutBlank(string rawText)
        {
            var split = rawText.Trim().Split(' ');

            var result = new List<string>();
            var current = "";

            foreach (var segment in split)
            {
                current += segment;
                if (current.Count(c => c == '"') % 2 == 0)
                {
                    result.Add(current.Trim().Trim('"'));
                    current = "";
                }
                else
                {
                    current += " ";
                }
            }

            if (!string.IsNullOrEmpty(current))
            {
                result.Add(current.Trim().Trim('"'));
            }

            return result.ToList();
        }
    }
}
