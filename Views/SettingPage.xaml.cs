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
using Microsoft.UI.Xaml.Media.Animation;
using NetifePanel.Interface;
using WinUI3Localizer;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace NetifePanel.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingPage : Page
    {
        public SettingPage()
        {
            this.InitializeComponent();
        }

        public SettingViewModel ViewModel { get; } = App.GetService<SettingViewModel>();

        private void NavigateLoaded(object sender, RoutedEventArgs e)
        {
            //Set navigate default page
            Navigate.SelectedItem = Navigate.MenuItems[0];

            //Add navigate handler
            contentFrame.Navigated += OnNavigated;

            // If navigation occurs on SelectionChanged, this isn't needed.
            // Because we use ItemInvoked to navigate, we need to call Navigate
            // here to load the home page.
            NavigateTo(typeof(SettingAppearancePage), new EntranceNavigationTransitionInfo());
        }

        private void OnNavigated(object sender, NavigationEventArgs e)
        {
            Navigate.IsBackEnabled = contentFrame.CanGoBack;
            
            if (contentFrame.SourcePageType != null)
            {
                // Select the nav view item that corresponds to the page being navigated to.

                Navigate.SelectedItem
                    = Navigate.MenuItems
                              .OfType<NavigationViewItem>()
                              .FirstOrDefault(i => i.Tag.Equals(contentFrame.SourcePageType.FullName.ToString()))
                              ??
                      Navigate.MenuItems[0];
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
            if (args.InvokedItemContainer != null)
            {
                Type navPageType = Type.GetType(args.InvokedItemContainer.Tag.ToString());
                NavigateTo(navPageType, args.RecommendedNavigationTransitionInfo);
            }
        }
    }
}
