using CommunityToolkit.Mvvm.ComponentModel;
using NetifePanel.Interface;

namespace NetifePanel.ViewModels
{
    public class LoadingViewModel : ObservableRecipient
    {
        private readonly IStaticData staticData;

        public LoadingViewModel(IStaticData staticData) 
        {
            this.staticData = staticData;
            Version = staticData.NetifeVersion;
            VersionType = staticData.NetifeVersionType.ToString();
        }

        public string Version { get; }

        public string VersionType { get; }
    }
}
