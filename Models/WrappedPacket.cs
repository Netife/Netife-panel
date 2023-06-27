using NetifePanel.Contracts;
using NetifePanel.Models.ArchieveItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetifePanel.Models
{
    public class WrappedPacket
    {
        public int No { get; set; }

        public Packet Packet { get; set; }

        /// <summary>
        /// API 终结点
        /// </summary>
        public string ApiEndPoint { get; set; }
        /// <summary>
        /// Api 终结点的缩减版，只会显示最后的Api终结点地址
        /// </summary>
        public string ApiEndPointMini { 

            get
            {
                return ApiEndPoint.Split("/").Last();
            } 
        }

        public NetworkRequestType RequestType { get; set; }

        public NetworkProtocal Protocal { get; set; }

        public string? Pid { get; set; }

        public string? ProcessName { get; set; }

        public string DstServer { get; set; }
    }
}
