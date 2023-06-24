using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using NetifePanel.Interface;
using NetifePanel.Serivces;
using NetifePanel.ViewModels;
using NetifePanel.Views;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Services.Maps;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace NetifePanel
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
        }

        public IServiceProvider Container { get; set; }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            //Build relative service
            Container = ConfigureService();

            m_window = new MainWindow();
            m_window.Activate();
        }

        private IServiceProvider ConfigureService()
        {
            var service = new ServiceCollection();

            var navigateService = new NavigateService();
            service.AddSingleton(navigateService);
            service.AddSingleton<IStaticData, StaticData>();


            //Navigation:
            //ignore loading view
            navigateService.Configure(nameof(MainBodyPage), typeof(MainBodyPage));
            navigateService.Configure(nameof(MainWindow), typeof(MainWindow));

            
            //Add ViewModel
            service.AddTransient<MainBodyViewModel>();
            service.AddTransient<LoadingViewModel>();

            return service.BuildServiceProvider();
        }

        private Window m_window;
    }
}
