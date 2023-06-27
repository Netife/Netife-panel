using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Configuration;
using Microsoft.UI.Dispatching;
using NetifeMessage;
using NetifePanel.Interface;
using NetifePanel.Models;
using NetifePanel.Models.ArchieveItems;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinUI3Localizer;

namespace NetifePanel.ViewModels
{
    public partial class HomeViewModel : ObservableRecipient
    {
        private ILocalizer _localizer;

        private IConfiguration _configuration;

        private IConfigurationService _configurationService;

        private INetifeService _netifeService;

        private int no = 1;
        public DispatcherQueue DispatcherQueue { get; set; }
        public HomeViewModel(ILocalizer localizer, IConfiguration configuration, 
                             IConfigurationService configurationService, INetifeService netifeService)
        {
            this._localizer = localizer;
            this._configuration = configuration;
            this._configurationService = configurationService;
            _netifeService = netifeService;

            //Config Netife Dispatcher Output
            netifeService.OnOutputUpdate += NetifeDispatcherOnOutputUpdate;

            //Config Netife Stream Print
            ConfigureStreamPrint();
        }

        private void NetifeDispatcherOnOutputUpdate(object sender, Contracts.NetifeDispatcherUpdate e)
        {
            DispatcherQueue.TryEnqueue(() => NetifeDispatcherOutput += e.UpdateText + "\n");
        }

        private void ConfigureStreamPrint()
        {
            _netifeService.ConfigureNetifePacketEvents(request =>
            {
                //Configure for double packets
                var packet = new Packet();
                var singalPacket = new NetworkSinglePacket();
                singalPacket.ApplicationType = (Contracts.NetworkApplicationType)request.ApplicationType;
                singalPacket.SrcIpPort = request.SrcIpPort;
                singalPacket.DstIpPort = request.DstIpPort;
                singalPacket.SrcIpAddr = request.SrcIpAddr;
                singalPacket.DstIpAddr = request.DstIpAddr;
                singalPacket.IsRawText = request.IsRawText;
                singalPacket.ProcessName = request.ProcessName;
                singalPacket.UUIDSub = request.UuidSub;
                singalPacket.RawText = request.RawText;
                singalPacket.Pid = request.Pid;
                packet.RequestType = (Contracts.NetworkRequestType)request.RequestType;
                packet.Protocal = (Contracts.NetworkProtocal)request.Protocol;
                packet.NetworkSiglePackets.Add(singalPacket);
                var wrappedPacket = new WrappedPacket();
                wrappedPacket.Packet = packet;
                wrappedPacket.Pid = request.Pid;
                wrappedPacket.ProcessName = request.ProcessName;
                if (string.IsNullOrEmpty(request.DstIpPort))
                {
                    wrappedPacket.DstServer = request.DstIpAddr;
                }
                else
                {
                    wrappedPacket.DstServer = request.DstIpAddr + ":" + request.DstIpPort;
                }
                wrappedPacket.Protocal = packet.Protocal;
                wrappedPacket.RequestType = packet.RequestType;
                wrappedPacket.No = no++;
                string hostUrl = string.Empty;
                if (!request.IsRawText)
                {
                    var host = request.RawText.Split("\n");
                    if (host[1].StartsWith("Host") && host.Length >= 2 )
                    {
                        hostUrl = host[1].Split(" ")[1];
                        hostUrl += host[0].Split(" ")[1];
                        hostUrl = hostUrl.Replace("\n", "").Replace("\r","");
                        hostUrl = request.RequestType + "://" + hostUrl;
                    }
                    else
                    {
                        hostUrl = "...";
                    }
                }
                wrappedPacket.ApiEndPoint = hostUrl;
                DispatcherQueue.TryEnqueue(() => Packets.Add(wrappedPacket));
                return request;
            });
        }

        [ObservableProperty]
        private ObservableCollection<ArchieveItem> archieves = new() { 
            new ArchieveItem()
            {
                Type = Contracts.ArchieveItemType.TempArchieve,
                Name = Localizer.Get().GetLocalizedString("Archieve_TempArchieve"),
                ArchieveItems = new () {
                    new Session()
                    {
                        Type = Contracts.ArchieveItemType.Session,
                        Name = Localizer.Get().GetLocalizedString("Archieve_TempSession"),
                    }
                }
            }
        };

        [ObservableProperty]
        private string netifeDispatcherOutput;

        [ObservableProperty]
        private ObservableCollection<WrappedPacket> packets = new();
    }
}
