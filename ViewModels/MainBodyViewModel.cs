using CommunityToolkit.Mvvm.ComponentModel;
using NetifePanel.Interface;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using WinUI3Localizer;

namespace NetifePanel.ViewModels
{
    public class MainBodyViewModel : ObservableRecipient
    {
        private INavigation _navigateService;

        private ILocalizer localizer;
        public MainBodyViewModel(INavigation navigateService, ILocalizer localizer) 
        {
            this._navigateService = navigateService;
            this.localizer = localizer;
            _navigateService.ResetBreadPath();
            _navigateService.PushBreadPath(localizer.GetLocalizedString("BreadNavigate_DashBoard"));
        }

        public ObservableCollection<string> TopBreadPath
        {
            get => _navigateService.BreadStack;
        }
    }
}
