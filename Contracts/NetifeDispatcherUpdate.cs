using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetifePanel.Contracts
{
    public class NetifeDispatcherUpdate : EventArgs
    {
        /// <summary>
        /// Dispatcher Service Output
        /// </summary>
        public string UpdateText { get; }

        public NetifeDispatcherUpdate(string content)
        {
            UpdateText = content;
        }
    }
}
