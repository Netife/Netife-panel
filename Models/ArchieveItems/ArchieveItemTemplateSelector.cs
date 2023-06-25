using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetifePanel.Models.ArchieveItems
{
    public class ArchieveItemTemplateSelector : DataTemplateSelector
    {
        //Archieve State
        public DataTemplate CommonArchieve { get; set; }

        public DataTemplate SharedArchieve { get; set; }

        public DataTemplate TempArchieve { get; set; }

        //ArchieveItem State

        public DataTemplate Packet { get; set; }

        public DataTemplate TempPacket { get; set; }

        public DataTemplate Note { get; set; }

        public DataTemplate RawText { get; set; }

        public DataTemplate Session { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            var explorerItem = (ArchieveItem)item;
            return explorerItem.Type switch
            {
                Contracts.ArchieveItemType.Packet => Packet,
                Contracts.ArchieveItemType.TempPacket => TempPacket,
                Contracts.ArchieveItemType.Note => Note,
                Contracts.ArchieveItemType.RawText => RawText,
                Contracts.ArchieveItemType.Session => Session,
                Contracts.ArchieveItemType.CommonArchieve => CommonArchieve,
                Contracts.ArchieveItemType.SharedArchieve => SharedArchieve,
                Contracts.ArchieveItemType.TempArchieve => TempArchieve,
                _ => throw new ArgumentException("Not support ArchieveItemType"),
            };
        }
    }
}
