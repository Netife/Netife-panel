using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetifePanel.Models
{
    public class Account
    {
        /// <summary>
        /// 账号名
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// 账号邮箱
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// Netife对应的身份识别Token
        /// </summary>
        public string NetifeToken { get; set; }
    }
}
