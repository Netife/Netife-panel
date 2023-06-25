using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetifePanel.Contracts
{
    public enum ArchieveItemType
    {
        Packet = 0, //抓包记录，其Content会存有一条抓包数据
        TempPacket = 1,//临时抓包记录，关闭软件后会自动删除，其Content会存有一条抓包数据
        Note = 2,//Netife专有格式的笔记，其Content会存多条含文字，图片，抓包内容的数据
        RawText = 3,//纯文字记录，其Content会存有多条文本数据
        Session = 4,//某一个会话集，其Content会存有多条抓包数据
        CommonArchieve,//普通存档
        SharedArchieve,//从外界加载的存档，或其他账号分享的存档
        TempArchieve,//临时存档，关闭软件后会自动删除
    }
}
