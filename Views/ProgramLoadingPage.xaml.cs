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
using Microsoft.UI.Windowing;
using Microsoft.UI;
using NetifePanel.Utils;
using NetifePanel.Interface;
using NetifePanel.ViewModels;
using System.Threading;
using System.Threading.Tasks;
using WinUI3Localizer;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace NetifePanel.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ProgramLoadingPage : Page
    {
        private AppWindow _apw;

        private OverlappedPresenter _presenter;

        private ILocalizer _localizer = App.GetService<ILocalizer>();
        public ProgramLoadingPage()
        {
            this.InitializeComponent();

            //Resize Windows 
            IntPtr hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.CurrentApp.RootWindow);
            WindowsHelper.SetWindowSize(hwnd, 561, 290);

            //Forbide Windows resizeable
            GetAppWindowAndPresenter();
            _presenter.IsResizable = false;

            //Set the title bar
            App.CurrentApp.RootWindow.ExtendsContentIntoTitleBar = true;
            App.CurrentApp.RootWindow.SetTitleBar(null);

            //Make Windows center
            var windowId = Win32Interop.GetWindowIdFromWindow(hwnd);
            var appWindow = AppWindow.GetFromWindowId(windowId);
            var CenteredPosition = appWindow.Position;
            var displayArea = DisplayArea.GetFromWindowId(windowId, DisplayAreaFallback.Nearest);
            CenteredPosition.X = ((displayArea.WorkArea.Width - appWindow.Size.Width) / 2);
            CenteredPosition.Y = ((displayArea.WorkArea.Height - appWindow.Size.Height) / 2);
            App.CurrentApp.RootWindow.AppWindow.Move(CenteredPosition);
            ViewModel.DispatcherQueue = DispatcherQueue;
            ViewModel.XamlRoot = this.XamlRoot;
            Task.Run(PreLoading);
        }

        public void GetAppWindowAndPresenter()
        {
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.CurrentApp.RootWindow);
            WindowId myWndId = Win32Interop.GetWindowIdFromWindow(hwnd);
            _apw = AppWindow.GetFromWindowId(myWndId);
            _presenter = _apw.Presenter as OverlappedPresenter;
        }

        public async void PreLoading()
        {
            Thread.Sleep(1000);
            var loadRes = await ViewModel.PreCheck();
            if (!loadRes)
            {
                DispatcherQueue.TryEnqueue(async () =>
                {
                    var dialog = new ContentDialog();
                    dialog.XamlRoot = this.XamlRoot;
                    dialog.Title = _localizer.GetLocalizedString("FirstLoading_Dialog_Title");
                    dialog.CloseButtonText = _localizer.GetLocalizedString("FirstLoading_Dialog_Button");
                    dialog.DefaultButton = ContentDialogButton.Primary;

                    var dialogContent = new TextBlock();
                    dialogContent.Text = _localizer.GetLocalizedString("FirstLoading_Dialog_Content");
                    dialogContent.TextWrapping = TextWrapping.Wrap;
                    dialog.Content = dialogContent;
                    await dialog.ShowAsync();
                    Application.Current.Exit();
                });
                Thread.Sleep(60*1000);
                Application.Current.Exit(); //Ensure exit
            }
            else
            {
                App.GetService<INavigation>().NavigateInUIThread(DispatcherQueue, nameof(MainBodyPage));
            }   
        }
        public LoadingViewModel ViewModel { get; } = App.GetService<LoadingViewModel>();
    }
}
