using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetifePanel.Interface
{
    public interface IConfigurationService
    {
        IConfiguration Configuration { get; }

        void UpdateCommonValue(string pos, string content);
    }
}
