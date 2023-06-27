using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using NetifePanel.Interface;
using RestSharp;
using System;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using WinUI3Localizer;

namespace NetifePanel.ViewModels
{
    public partial class LoadingViewModel : ObservableRecipient
    {
        private readonly IStaticData staticData;

        private ILocalizer _localizer;

        public LoadingViewModel(IStaticData staticData, ILocalizer localizer) 
        {
            this.staticData = staticData;
            this._localizer = localizer;
            Version = staticData.NetifeVersion;
            VersionType = staticData.NetifeVersionType.ToString();
        }

        [ObservableProperty]
        private string loadingTips = "Checking for Relatives...";

        [ObservableProperty]
        private string version;

        [ObservableProperty]
        private string versionType;

        public XamlRoot XamlRoot;

        public DispatcherQueue DispatcherQueue { get; set; }

        public async Task<bool> PreCheck()
        {
            var dataFolder = ApplicationData.Current.LocalFolder;
            var preChecked = await dataFolder.TryGetItemAsync("md5") as StorageFile;
            if (preChecked == null)
            {
                //Pre check the need packageList
                DispatcherQueue.TryEnqueue(() => LoadingTips = "First Start will take a long time to init...");
                Thread.Sleep(5000);
                DispatcherQueue.TryEnqueue(() => LoadingTips = "Trying to download PreBuilt Binary...");
                var client = new RestClient();
                var md5Req = new RestRequest("http://netife.sorux.cn/download/md5");
                var md5 = await client.ExecuteAsync(md5Req);

                var binaryFiles = new RestRequest("http://netife.sorux.cn/download/Binary.zip");
                var bin = await client.ExecuteAsync(binaryFiles);
                var filePath = Path.Combine(dataFolder.Path, "Binary.zip");
                await File.WriteAllBytesAsync(filePath, bin.RawBytes);

                string md5CacledString = string.Empty;
                using (var md5Cacled = MD5.Create())
                {
                    using (var stream = File.OpenRead(filePath))
                    {
                        byte[] hashBytes = md5Cacled.ComputeHash(stream);
                        md5CacledString = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
                    }
                }

                if (md5CacledString != md5.Content)
                {
                    DispatcherQueue.TryEnqueue(() => LoadingTips = "PreBuilt Binary cannnot fit the expect md5...");
                    Thread.Sleep(3000);
                    DispatcherQueue.TryEnqueue(() => LoadingTips = "Error for downloading prebuilt binary content!!!");
                    Thread.Sleep(1000);
                    return false;
                }
                else
                {
                    DispatcherQueue.TryEnqueue(() => LoadingTips = "Extract PreBuilt Binary...");
                    ZipFile.ExtractToDirectory(filePath, dataFolder.Path);
                    File.WriteAllText(Path.Combine(dataFolder.Path, "md5"), md5.Content);
                    File.Delete(filePath);
                    DispatcherQueue.TryEnqueue(() => LoadingTips = "Environment Checking...");
                    Thread.Sleep(3000);
                }
            }

            DispatcherQueue.TryEnqueue(() => LoadingTips = "Starting Netife Service...");
            Thread.Sleep(2000);
            await App.GetService<INetifeService>().StartBasicService();
            return true;
        }
    }
}
