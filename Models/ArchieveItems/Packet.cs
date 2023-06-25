using NetifePanel.Contracts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetifePanel.Models.ArchieveItems
{
    public class Packet : ArchieveItem
    {
        public string UUID { get; set; }

        public NetworkRequestType RequestType { get; set; }

        public NetworkProtocal Protocal { get; set; }

        public ObservableCollection<NetworkSiglePacket> NetworkSiglePackets { get; set; }
    }
}
