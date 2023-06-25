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
using Microsoft.Extensions.DependencyInjection;
using NetifePanel.ViewModels;
using Microsoft.UI.Windowing;
using Microsoft.UI;
using NetifePanel.Utils;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.UI.Composition.SystemBackdrops;
using System.Runtime.InteropServices;
using WinRT;
using Microsoft.UI.Xaml.Media.Animation;
using Windows.UI.ApplicationSettings;
using NetifePanel.Interface;
using WinUI3Localizer;
// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace NetifePanel.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainBodyPage : Page
    {
        private AppWindow _apw;

        private OverlappedPresenter _presenter;

        public MainBodyPage()
        {
            this.InitializeComponent();

            //Resize Windows 
            IntPtr hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.CurrentApp.RootWindow);
            WindowsHelper.SetWindowSize(hwnd, 1440, 1024);

            //Forbide Windows resizeable
            GetAppWindowAndPresenter();
            _presenter.IsResizable = true;

            //Set the title bar
            App.CurrentApp.RootWindow.ExtendsContentIntoTitleBar = true;
            App.CurrentApp.RootWindow.SetTitleBar(AppTitleBar);

            //Make Windows center
            var windowId = Win32Interop.GetWindowIdFromWindow(hwnd);
            var appWindow = AppWindow.GetFromWindowId(windowId);
            var CenteredPosition = appWindow.Position;
            var displayArea = DisplayArea.GetFromWindowId(windowId, DisplayAreaFallback.Nearest);
            CenteredPosition.X = ((displayArea.WorkArea.Width - appWindow.Size.Width) / 2);
            CenteredPosition.Y = ((displayArea.WorkArea.Height - appWindow.Size.Height) / 2);
            App.CurrentApp.RootWindow.AppWindow.Move(CenteredPosition);

        }

        public void GetAppWindowAndPresenter()
        {
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.CurrentApp.RootWindow);
            WindowId myWndId = Win32Interop.GetWindowIdFromWindow(hwnd);
            _apw = AppWindow.GetFromWindowId(myWndId);
            _presenter = _apw.Presenter as OverlappedPresenter;
        }

        public MainBodyViewModel ViewModel { get; } = App.GetService<MainBodyViewModel>();

        private INavigation navigation = App.GetService<INavigation>();

        private void NavigateLoaded(object sender, RoutedEventArgs e)
        {
            //Set navigate default page
            Navigate.SelectedItem = Navigate.MenuItems[0];

            //Add navigate handler
            contentFrame.Navigated += OnNavigated;

            // If navigation occurs on SelectionChanged, this isn't needed.
            // Because we use ItemInvoked to navigate, we need to call Navigate
            // here to load the home page.
            NavigateTo(typeof(HomePage), new EntranceNavigationTransitionInfo());
        }

        private void OnNavigated(object sender, NavigationEventArgs e)
        {
            Navigate.IsBackEnabled = contentFrame.CanGoBack;

            if (contentFrame.SourcePageType == typeof(SettingPage))
            {
                // SettingsItem is not part of NavView.MenuItems, and doesn't have a Tag.
                Navigate.SelectedItem = (NavigationViewItem)Navigate.SettingsItem;
                navigation.ResetBreadPath();
                navigation.PushBreadPath(Localizer.Get().GetLocalizedString("BreadNavigate_Settings"));
            }
            else if (contentFrame.SourcePageType != null)
            {
                // Select the nav view item that corresponds to the page being navigated to.

                Navigate.SelectedItem 
                    = Navigate.MenuItems
                              .OfType<NavigationViewItem>()
                              .FirstOrDefault(i => i.Tag.Equals(contentFrame.SourcePageType.FullName.ToString()))
                              ??
                      Navigate.FooterMenuItems
                              .OfType<NavigationViewItem>()
                              .FirstOrDefault(i => i.Tag.Equals(contentFrame.SourcePageType.FullName.ToString()))
                              ??
                      Navigate.MenuItems[0];
                navigation.ResetBreadPath();
                navigation.PushBreadPath(Localizer.Get().GetLocalizedString(contentFrame.SourcePageType.FullName.ToString() switch
                {
                    "NetifePanel.Views.HomePage" => "BreadNavigate_DashBoard",
                    "NetifePanel.Views.ComposerPage" => "BreadNavigate_Composer",
                    "NetifePanel.Views.LibraryPage" => "BreadNavigate_Library",
                    "NetifePanel.Views.MailPage" => "BreadNavigate_Mail",
                    "NetifePanel.Views.AccountPage" => "BreadNavigate_Account",
                    "NetifePanel.Views.HelpPage" => "BreadNavigate_Help",
                }));
            }
        }

        private bool TryGoBack()
        {
            if (!contentFrame.CanGoBack)
                return false;

            // Don't go back if the nav panel is overlayed.
            if (Navigate.IsPaneOpen &&
                (Navigate.DisplayMode == NavigationViewDisplayMode.Compact ||
                 Navigate.DisplayMode == NavigationViewDisplayMode.Minimal))
                return false;

            contentFrame.GoBack();
            return true;
        }

        private void NavigateBackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            TryGoBack();
        }


        private void NavigateTo(
            Type navPageType,
            NavigationTransitionInfo transitionInfo)
        {
            // Get the page type before navigation so you can prevent duplicate
            // entries in the backstack.
            Type preNavPageType = contentFrame.CurrentSourcePageType;

            // Only navigate if the selected page isn't currently loaded.
            if (navPageType is not null && !Type.Equals(preNavPageType, navPageType))
            {
                contentFrame.Navigate(navPageType, null, transitionInfo);
            }
        }

        private void NavgateItemInvoked(NavigationView sender,
                                 NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked == true)
            {
                NavigateTo(typeof(SettingPage), args.RecommendedNavigationTransitionInfo);
            }
            else if (args.InvokedItemContainer != null)
            {
                Type navPageType = Type.GetType(args.InvokedItemContainer.Tag.ToString());
                NavigateTo(navPageType, args.RecommendedNavigationTransitionInfo);
            }
        }
    }
}
