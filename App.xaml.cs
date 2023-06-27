using Grpc.Core;
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
using NetifeMessage;
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
using System.Xml.Serialization;
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

        public static App CurrentApp { get; private set; } = Current as App;

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

            //Build Grpc Server
            BuildFrontendGrpcServer();

            //Build Default Frame
            BuildDefaultFrame(args);
        }

        private void BuildDefaultFrame(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            RootWindow = new MainWindow();
            RootFrame = new Frame();
            RootWindow.Content = RootFrame;
            RootWindow.Activate();
            RootFrame.Navigate(typeof(ProgramLoadingPage), args.Arguments);
        }

        private void BuildFrontendGrpcServer()
        {
            var config = GetService<IConfiguration>();
            var configNode = config.GetRequiredSection("component").GetRequiredSection("lauchSettings");
            new Server
            {
                Services =
                           {
                               NetifePost.BindService(GetService<NetifeFrontendCoreService>())
                           },
                Ports = { new ServerPort(configNode.GetSection("Frontend")["Host"], int.Parse(configNode.GetSection("Frontend")["Port"]), 
                                            ServerCredentials.Insecure) }
            }.Start();
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
                       service.AddSingleton<INetifeService, Serivces.NetifeService>();
                       service.AddSingleton<NetifeFrontendCoreService>();

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
                       service.AddSingleton<HomeViewModel>();
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
                       var baseConfigPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Config");
                       var configFolder = new DirectoryInfo(baseConfigPath);
                       if (!configFolder.Exists) configFolder.Create();
                       var file = new FileInfo(Path.Combine(baseConfigPath, "Settings.json"));
                       if (!file.Exists)
                       using (var sw = file.CreateText())
                       {
                           //Default Json WriteIn
                           sw.Write("{\r\n  \"appearance\": {\r\n    \"language\": \"en-US\"\r\n  },\r\n  \"component\": {\r\n    \"lauchSettings\": {\r\n      \"JsRemote\": {\r\n        \"Host\": \"0.0.0.0\",\r\n        \"Port\": \"7892\"\r\n      },\r\n      \"Dispatcher\": {\r\n        \"Host\": \"0.0.0.0\",\r\n        \"Port\": \"7890\"\r\n      },\r\n      \"Probe\": {\r\n        \"Host\": \"\",\r\n        \"Port\": \"\"\r\n      },\r\n      \"Frontend\": {\r\n        \"Host\": \"0.0.0.0\",\r\n        \"Port\": \"7891\"\r\n      }\r\n    }\r\n  }\r\n}");
                       }
                       builder.AddJsonFile(file.FullName, optional: false, reloadOnChange: true);
                   })
                   .Build();
            //Default Localizer
            Localizer.Get().SetLanguage(GetService<IConfiguration>().GetRequiredSection("appearance")["language"].ToString());
        }

        private async Task InitializeLocalizer()
        {
            var localFolder = Path.Combine(AppContext.BaseDirectory, "Strings");
            StorageFolder stringsFolder = await StorageFolder.GetFolderFromPathAsync(localFolder);
            if (Window.Current != null)
            {
                string resourceFileName = "Resources.resw";
                await CreateStringResourceFileIfNotExists(stringsFolder, "en-US", resourceFileName);
                await CreateStringResourceFileIfNotExists(stringsFolder, "zh-CN", resourceFileName);
            }

            ILocalizer localizer = await new LocalizerBuilder()
                .AddStringResourcesFolderForLanguageDictionaries(localFolder)
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
