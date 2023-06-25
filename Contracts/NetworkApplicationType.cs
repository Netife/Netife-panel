using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetifePanel.Contracts
{
    public enum NetworkApplicationType
    {
        CLIENT = 0, // 表示本消息为客户端请求
        SERVER = 1 // 表示本消息为服务器响应
    }
}
