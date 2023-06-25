using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Configuration;
using NetifePanel.Interface;
using NetifePanel.Models;
using NetifePanel.Models.ArchieveItems;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinUI3Localizer;

namespace NetifePanel.ViewModels
{
    public partial class HomeViewModel : ObservableRecipient
    {
        private ILocalizer _localizer;

        private IConfiguration _configuration;

        private IConfigurationService _configurationService;

        public HomeViewModel(ILocalizer localizer, IConfiguration configuration, IConfigurationService configurationService)
        {
            this._localizer = localizer;
            this._configuration = configuration;
            this._configurationService = configurationService;
        }

        [ObservableProperty]
        private ObservableCollection<ArchieveItem> archieves = new() { 
            new ArchieveItem()
            {
                Type = Contracts.ArchieveItemType.TempArchieve,
                Name = Localizer.Get().GetLocalizedString("Archieve_TempArchieve"),
                ArchieveItems = new () {
                    new Session()
                    {
                        Type = Contracts.ArchieveItemType.Session,
                        Name = Localizer.Get().GetLocalizedString("Archieve_TempSession"),
                    }
                }
            }
        };
    }
}
