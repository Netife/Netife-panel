using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using NetifePanel.Utils;
using NetifePanel.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace NetifePanel
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {

        private AppWindow _apw;
        private OverlappedPresenter _presenter;

        public MainWindow()
        {
            this.InitializeComponent();

            //调整窗口大小
            IntPtr hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            WindowsHelper.SetWindowSize(hwnd, 561, 290);
            
            //禁用 User Resize
            GetAppWindowAndPresenter();
            _presenter.IsResizable = false;

            //设置标题栏
            this.ExtendsContentIntoTitleBar = true;
            this.SetTitleBar(null);

            //窗口居中
            var windowId = Win32Interop.GetWindowIdFromWindow(hwnd);
            var appWindow = AppWindow.GetFromWindowId(windowId);
            var CenteredPosition = appWindow.Position;
            var displayArea = DisplayArea.GetFromWindowId(windowId, DisplayAreaFallback.Nearest);
            CenteredPosition.X = ((displayArea.WorkArea.Width - appWindow.Size.Width) / 2);
            CenteredPosition.Y = ((displayArea.WorkArea.Height - appWindow.Size.Height) / 2);
            this.AppWindow.Move(CenteredPosition);
        }

        public void GetAppWindowAndPresenter()
        {
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            WindowId myWndId = Win32Interop.GetWindowIdFromWindow(hwnd);
            _apw = AppWindow.GetFromWindowId(myWndId);
            _presenter = _apw.Presenter as OverlappedPresenter;
        }


        public LoadingViewModel ViewModel { get; } = (Application.Current as App).Container.GetRequiredService<LoadingViewModel>();
    }
}
