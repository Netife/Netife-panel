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
    }
}
