using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml.Controls;
using NetifePanel.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinUI3Localizer;

namespace NetifePanel.ViewModels
{
    public partial class SettingAppearanceViewModel : ObservableRecipient
    {
        private readonly IConfiguration _configuration;

        private readonly ILocalizer _localizer;

        private readonly IConfigurationService _configurationService;

        public SettingAppearanceViewModel(IConfiguration configuration, ILocalizer localizer, IConfigurationService configurationService)
        {
            _configuration = configuration;
            _localizer = localizer;
            _configurationService = configurationService;

            //Init settings
            LanguageSettings = configuration.GetRequiredSection("appearance")["language"].ToString();
            PropertyChanged += SettingAppearanceViewModel_PropertyChanged;
        }

        private void SettingAppearanceViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(LanguageSettings):
                    LanguageComboxSetChanged();
                    break;

            }
        }

        [ObservableProperty]
        private string languageSettings;

        [RelayCommand]
        private void LanguageComboxSetChanged()
        {
            _configurationService.UpdateCommonValue("appearance:language", LanguageSettings);
            _localizer.SetLanguage(LanguageSettings);
        }
    }
}
