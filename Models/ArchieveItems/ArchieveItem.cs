using CommunityToolkit.Mvvm.ComponentModel;
using NetifePanel.Contracts;
using NetifePanel.Models.ArchieveItems;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetifePanel.Models.ArchieveItems
{
    public partial class ArchieveItem : ObservableObject
    {
        [ObservableProperty]
        /// <summary>
        /// 存档类型
        /// </summary>
        private ArchieveItemType type;
        [ObservableProperty]
        /// <summary>
        /// 存档名称
        /// </summary>
        private string name;
        [ObservableProperty]
        /// <summary>
        /// 存档创建时间
        /// </summary>
        private DateTime createdDateTime;
        [ObservableProperty]
        /// <summary>
        /// 上一次修改的时间
        /// </summary>
        private DateTime lastUpdateDateTime;
        [ObservableProperty]
        /// <summary>
        /// 关联的Netife账号的邮箱
        /// </summary>
        private string accountEmail;
        [ObservableProperty]
        /// <summary>
        /// 内容签名，如果有那么内容就是被加密过的
        /// PS: 规定，如果父级归档加密，那么它的子类归档只能以加密的方式存储
        /// </summary>
        private string? sign;
        [ObservableProperty]
        /// <summary>
        /// 加密后的内容数据
        /// </summary>
        private byte[]? signedContent;
        [ObservableProperty]
        /// <summary>
        /// 归档项目
        /// </summary>
        private ObservableCollection<ArchieveItem> archieveItems = new ObservableCollection<ArchieveItem>();
    }
}
