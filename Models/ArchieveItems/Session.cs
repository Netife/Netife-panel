using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetifePanel.Models.ArchieveItems
{
    public class Session : ArchieveItem
    {
        public ObservableCollection<Packet> packets = new ObservableCollection<Packet>();
    }
}
