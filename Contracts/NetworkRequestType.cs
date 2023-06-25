using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetifePanel.Contracts
{
    public enum NetworkRequestType
    {
        HTTP = 0, // HTTP协议
        HTTPS = 1, // HTTPS协议
        WS = 2, // WS协议
        WSS = 3, // WSS协议
        PING = 4, // PING
        OTHER = 5 //[本字段保留以便于扩展] 其他
    }
}
