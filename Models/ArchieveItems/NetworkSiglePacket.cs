using NetifePanel.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetifePanel.Models.ArchieveItems
{
    public class NetworkSinglePacket
    {
        public NetworkApplicationType ApplicationType { get; set; }

        public string DstIpAddr { get; set; }

        public string UUID { get; set; }

        public string DstIpPort { get; set; }

        public string SrcIpAddr { get; set; }

        public string SrcIpPort { get; set; }

        public bool IsRawText { get; set; }

        public int? UUIDSub { get; set; }

        public string RawText { get; set; }

        public string? Pid { get; set; }

        public string? ProcessName { get; set; }
    }
}
