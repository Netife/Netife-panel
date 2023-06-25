using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI;
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
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Services.Maps;
using Windows.Storage;
using WinUI3Localizer;
using Path = System.IO.Path;

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

        public IHost Host { get; private set; }

        public Frame RootFrame { get; private set; }

        public Window RootWindow { get; private set; }

        public static T GetService<T>() where T : class{
            if ((Application.Current as App).Host.Services.GetRequiredService<T>() is not T service)
            {
                throw new ArgumentException($"Cannot get the target type: {nameof(T)}");
            }

            return service;
        }

        public static App CurrentApp { get; private set; } = (Current as App);

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override async void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            //I18N
            await InitializeLocalizer();

            //Build relative service
            ConfigureService();
            
            RootWindow = new MainWindow();
            RootFrame = new Frame();
            RootWindow.Content = RootFrame;
            RootWindow.Activate();

            RootFrame.Navigate(typeof(ProgramLoadingPage), args.Arguments);
        }

        private void ConfigureService()
        {
            Host = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder()
                   .UseContentRoot(AppContext.BaseDirectory)
                   .ConfigureServices((ctx, service) =>
                   {

                       //Core services
                       service.AddSingleton(Localizer.Get());
                       var navigateService = new NavigateService(Localizer.Get());

                       service.AddSingleton<INavigation>(navigateService);
                       service.AddSingleton<IStaticData, StaticData>();
                       service.AddSingleton<IConfigurationService, ConfigurationService>();


                       //Configure Navigation
                       navigateService.Configure(nameof(ProgramLoadingPage), typeof(ProgramLoadingPage));
                       navigateService.Configure(nameof(MainBodyPage), typeof(MainBodyPage));
                       navigateService.Configure(nameof(HelpPage), typeof(HelpPage));
                       navigateService.Configure(nameof(HomePage), typeof(HomePage));
                       navigateService.Configure(nameof(ComposerPage), typeof(ComposerPage));
                       navigateService.Configure(nameof(SettingPage), typeof(SettingPage));
                       navigateService.Configure(nameof(AccountPage), typeof(AccountPage));
                       navigateService.Configure(nameof(MailPage), typeof(MailPage));
                       navigateService.Configure(nameof(LibraryPage), typeof(LibraryPage));
                       navigateService.Configure(nameof(SettingAppearancePage), typeof(SettingAppearancePage));

                       //ViewModel
                       service.AddTransient<MainBodyViewModel>();
                       service.AddTransient<LoadingViewModel>();
                       service.AddTransient<HelpViewModel>();
                       service.AddTransient<HomeViewModel>();
                       service.AddTransient<ComposerViewModel>();
                       service.AddTransient<SettingViewModel>();
                       service.AddTransient<AccountViewModel>();
                       service.AddTransient<MailViewModel>();
                       service.AddTransient<LibraryViewModel>();
                       service.AddTransient<SettingAppearanceViewModel>();

                   })
                   .ConfigureAppConfiguration((ctx,builder) =>
                   {
                       //Check for the existence of the configuration
                       var baseConfigPath = Path.Combine(AppContext.BaseDirectory, "Config");
                       var configFolder = new DirectoryInfo(baseConfigPath);
                       if (!configFolder.Exists) configFolder.Create();
                       var file = new FileInfo(Path.Combine(baseConfigPath, "Settings.json"));
                       if (!file.Exists) file.Create();
                       
                       builder.AddJsonFile(file.FullName, optional: false, reloadOnChange: true);
                   })
                   .Build();
            //Default Localizer
            Localizer.Get().SetLanguage(GetService<IConfiguration>().GetRequiredSection("appearance")["language"].ToString());
        }

        private async Task InitializeLocalizer()
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            StorageFolder stringsFolder = await localFolder.CreateFolderAsync("Strings", CreationCollisionOption.OpenIfExists);

            string resourceFileName = "Resources.resw";
            await CreateStringResourceFileIfNotExists(stringsFolder, "en-US", resourceFileName);
            await CreateStringResourceFileIfNotExists(stringsFolder, "zh-CN", resourceFileName);
            
            ILocalizer localizer = await new LocalizerBuilder()
                .AddStringResourcesFolderForLanguageDictionaries(stringsFolder.Path)
                .SetOptions(options =>
                {
                    options.DefaultLanguage = "en-US";
                    options.UseUidWhenLocalizedStringNotFound = true;
                })
                .Build();
        }

        private static async Task CreateStringResourceFileIfNotExists(StorageFolder stringsFolder, string language, string resourceFileName)
        {
            StorageFolder languageFolder = await stringsFolder.CreateFolderAsync(
                language,
                CreationCollisionOption.OpenIfExists);

            if (await languageFolder.TryGetItemAsync(resourceFileName) is null)
            {
                string resourceFilePath = Path.Combine(stringsFolder.Name, language, resourceFileName);
                StorageFile resourceFile = await LoadStringResourcesFileFromAppResource(resourceFilePath);
                _ = await resourceFile.CopyAsync(languageFolder);
            }
        }

        private static async Task<StorageFile> LoadStringResourcesFileFromAppResource(string filePath)
        {
            Uri resourcesFileUri = new($"ms-appx:///{filePath}");
            return await StorageFile.GetFileFromApplicationUriAsync(resourcesFileUri);
        }
    }
}
