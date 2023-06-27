using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using NetifePanel.ViewModels;
using WinUI3Localizer;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace NetifePanel.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HomePage : Page
    {
        public HomePage()
        {
            this.InitializeComponent();

            //Set DataContext
            this.packetDataGrid.DataContext = ViewModel.Packets;

            //Register DataGrid Header
            //I18N
            RegisterDataGridHeader();
            var localizer = Localizer.Get();
            localizer.LanguageChanged += LanguageChanged;
            ViewModel.DispatcherQueue = DispatcherQueue;
        }

        private void RegisterDataGridHeader()
        {
            var localizer = Localizer.Get();
            MainPage_DataGrid_No.Header = localizer.GetLocalizedString("MainPage_DataGrid_No");
            MainPage_DataGrid_ApiEndPoint.Header = localizer.GetLocalizedString("MainPage_DataGrid_ApiEndPoint");
            MainPage_DataGrid_RequestType.Header = localizer.GetLocalizedString("MainPage_DataGrid_RequestType");
            MainPage_DataGrid_Protocal.Header = localizer.GetLocalizedString("MainPage_DataGrid_Protocal");
            MainPage_DataGrid_DstServer.Header = localizer.GetLocalizedString("MainPage_DataGrid_DstServer");
            MainPage_DataGrid_Pid.Header = localizer.GetLocalizedString("MainPage_DataGrid_Pid");
            MainPage_DataGrid_ProcessName.Header = localizer.GetLocalizedString("MainPage_DataGrid_ProcessName");
        }

        private void LanguageChanged(object sender, LanguageChangedEventArgs e)
        {
            RegisterDataGridHeader();
        }

        public HomeViewModel ViewModel { get; } = App.GetService<HomeViewModel>();
    }
}
