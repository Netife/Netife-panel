using NetifePanel.Interface;

namespace NetifePanel.Serivces
{
    class StaticData : IStaticData
    {
        public string NetifeVersion => "V1.0";

        public NetifeVersionType NetifeVersionType => NetifeVersionType.Canary;
    }
}
