using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetifePanel.Interface
{
    public interface IStaticData
    {
        public string NetifeVersion { get;}

        public NetifeVersionType NetifeVersionType { get;}
    }

    public enum NetifeVersionType
    {
        Release = 0,
        Beta,
        Canary
    }
}
