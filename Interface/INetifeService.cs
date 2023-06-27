using NetifeMessage;
using NetifePanel.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetifePanel.Interface
{
    /// <summary>
    /// Netife Core Service
    /// </summary>
    public interface INetifeService
    {
        /// <summary>
        /// Run Netife Basic Service
        /// </summary>
        /// <returns></returns>
        Task<bool> StartBasicService();
        /// <summary>
        /// Recieve Netife Event
        /// </summary>
        event EventHandler<NetifeDispatcherUpdate> OnOutputUpdate;
        /// <summary>
        /// Configure Netife Packet Event
        /// </summary>
        /// <param name="action"></param>
        public void ConfigureNetifePacketEvents(Func<NetifeProbeRequest, NetifeProbeRequest> action);
        /// <summary>
        /// Invoke Netife Triggers
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<NetifeProbeResponse> InvokeTriggers(NetifeProbeRequest request);
    }
}
