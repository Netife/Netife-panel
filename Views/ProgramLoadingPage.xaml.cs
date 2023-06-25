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
            Task.Run(PreLoading);
        }

        public void GetAppWindowAndPresenter()
        {
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.CurrentApp.RootWindow);
            WindowId myWndId = Win32Interop.GetWindowIdFromWindow(hwnd);
            _apw = AppWindow.GetFromWindowId(myWndId);
            _presenter = _apw.Presenter as OverlappedPresenter;
        }

        public void PreLoading()
        {
            Thread.Sleep(500);
            App.GetService<INavigation>().NavigateInUIThread(DispatcherQueue, nameof(MainBodyPage));
        }
        public LoadingViewModel ViewModel { get; } = App.GetService<LoadingViewModel>();
    }
}
