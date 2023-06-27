using Grpc.Core;
using NetifeMessage;
using NetifePanel.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetifePanel.Serivces
{
    public class NetifeFrontendCoreService : NetifePost.NetifePostBase
    {
        private INetifeService _netifeService;

        public NetifeFrontendCoreService(INetifeService netifeService) 
        {
            _netifeService = netifeService;
        }
        public override async Task<NetifeProbeResponse> UploadRequest(NetifeProbeRequest request, ServerCallContext context)
        {
            return await _netifeService.InvokeTriggers(request);
        }
    }
}
