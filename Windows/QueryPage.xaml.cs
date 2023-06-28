using Microsoft.UI.Windowing;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using NetifePanel.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using WinUI3Localizer;
using NetifePanel.Interface;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace NetifePanel.Windows
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class QueryPage : Page
    {
        private AppWindow _apw;

        private OverlappedPresenter _presenter;

        private ILocalizer _localizer = App.GetService<ILocalizer>();
        public QueryPage()
        {
            this.InitializeComponent();

            //Resize Windows 
            IntPtr hwnd = WinRT.Interop.WindowNative.GetWindowHandle(QueryWindow.Instance);
            WindowsHelper.SetWindowSize(hwnd, 1200, 800);

            //Forbide Windows resizeable
            GetAppWindowAndPresenter();
            _presenter.IsResizable = false;

            //Set the title bar
            QueryWindow.Instance.ExtendsContentIntoTitleBar = true;
            QueryWindow.Instance.SetTitleBar(AppTitleBar);

            //Make Windows center
            var windowId = Win32Interop.GetWindowIdFromWindow(hwnd);
            var appWindow = AppWindow.GetFromWindowId(windowId);
            var CenteredPosition = appWindow.Position;
            var displayArea = DisplayArea.GetFromWindowId(windowId, DisplayAreaFallback.Nearest);
            CenteredPosition.X = ((displayArea.WorkArea.Width - appWindow.Size.Width) / 2);
            CenteredPosition.Y = ((displayArea.WorkArea.Height - appWindow.Size.Height) / 2);
            QueryWindow.Instance.AppWindow.Move(CenteredPosition);
        }

        public void GetAppWindowAndPresenter()
        {
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(QueryWindow.Instance);
            WindowId myWndId = Win32Interop.GetWindowIdFromWindow(hwnd);
            _apw = AppWindow.GetFromWindowId(myWndId);
            _presenter = _apw.Presenter as OverlappedPresenter;
        }

        private void SendCommand(object sender, RoutedEventArgs e)
        {
            var netifeService = App.GetService<INetifeService>();
            var res = netifeService.QueryRemoteCommand(CommandInput.Text);
            if (!string.IsNullOrEmpty(res))
            {
                CommandOutput.Text = res;
            }
            else
            {
                CommandOutput.Text = "(No Result)";
            }
        }
    }
}
