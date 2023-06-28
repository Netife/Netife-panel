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
using WinUI3Localizer;
using NetifePanel.Models;
using System.Web;
using Google.Protobuf.WellKnownTypes;
using CommunityToolkit.WinUI.UI.Controls;
using Windows.ApplicationModel.DataTransfer;
using Microsoft.UI;
using static PInvoke.User32;
using System.Collections.ObjectModel;
using NetifePanel.Utils;
using CommunityToolkit.Mvvm.ComponentModel;
using NetifePanel.Windows;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace NetifePanel.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HomePage : Page
    {
        public HomePage()
        {
            this.InitializeComponent();
            
            //Set Dispatcher Queue
            ViewModel.DispatcherQueue = DispatcherQueue;
            //Set DataContext
            this.packetDataGrid.DataContext = ViewModel.Packets;

            //Register DataGrid Header
            //I18N
            RegisterDataGridHeader();
            var localizer = Localizer.Get();
            localizer.LanguageChanged += LanguageChanged;

        }

        private void RegisterDataGridHeader()
        {
            var localizer = Localizer.Get();
            MainPage_DataGrid_No.Header = localizer.GetLocalizedString("MainPage_DataGrid_No");
            MainPage_DataGrid_ApiEndPoint.Header = localizer.GetLocalizedString("MainPage_DataGrid_ApiEndPoint");
            MainPage_DataGrid_RequestType.Header = localizer.GetLocalizedString("MainPage_DataGrid_RequestType");
            MainPage_DataGrid_Protocal.Header = localizer.GetLocalizedString("MainPage_DataGrid_Protocal");
            MainPage_DataGrid_DstServer.Header = localizer.GetLocalizedString("MainPage_DataGrid_DstServer");
            MainPage_DataGrid_Pid.Header = localizer.GetLocalizedString("MainPage_DataGrid_Pid");
            MainPage_DataGrid_ProcessName.Header = localizer.GetLocalizedString("MainPage_DataGrid_ProcessName");
            MainPage_Inspector_Request_Params_Key.Header = localizer.GetLocalizedString("MainPage_Inspector_Key");
            MainPage_Inspector_Request_Params_Value.Header = localizer.GetLocalizedString("MainPage_Inspector_Value");
            MainPage_Inspector_Request_Header_Key.Header = localizer.GetLocalizedString("MainPage_Inspector_Key");
            MainPage_Inspector_Request_Header_Value.Header = localizer.GetLocalizedString("MainPage_Inspector_Value");
            MainPage_Inspector_Request_Cookies_Key.Header = localizer.GetLocalizedString("MainPage_Inspector_Key");
            MainPage_Inspector_Request_Cookies_Value.Header = localizer.GetLocalizedString("MainPage_Inspector_Value");
            MainPage_Inspector_Response_Header_Key.Header = localizer.GetLocalizedString("MainPage_Inspector_Key");
            MainPage_Inspector_Response_Header_Value.Header = localizer.GetLocalizedString("MainPage_Inspector_Value");
            MainPage_Inspector_Response_Cookie_Key.Header = localizer.GetLocalizedString("MainPage_Inspector_Key");
            MainPage_Inspector_Response_Cookie_Value.Header = localizer.GetLocalizedString("MainPage_Inspector_Value");
        }

        private void LanguageChanged(object sender, LanguageChangedEventArgs e)
        {
            RegisterDataGridHeader();
        }

        public HomeViewModel ViewModel { get; } = App.GetService<HomeViewModel>();

        private void packetDataGridSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = packetDataGrid.SelectedItem;
            ViewModel.SelectPacket = item as WrappedPacket;

            //RawText
            InspectorRequestRaw.Text = ViewModel.SelectPacket.Packet.NetworkSiglePackets.FirstOrDefault()?.RawText ?? "(Blank)" ?? "(Blank)";
            if (ViewModel.SelectPacket.Packet.NetworkSiglePackets.Count < 2)
            {
                InspectorResponseRaw.Text = "(Blank)";
            }
            else
            {
                InspectorResponseRaw.Text = ViewModel.SelectPacket.Packet.NetworkSiglePackets[1].RawText;
            }

            //Body
            if (InspectorRequestRaw.Text.Contains("\r\r"))
            {
                InspectorRequestBody.Text = InspectorRequestRaw.Text.Split("\r\r").Last();
            }

            if (InspectorResponseRaw.Text.Contains("\r\r"))
            {
                InspectorResponseBody.Text = InspectorResponseRaw.Text.Split("\r\r").Last();
            }


            //Headers
            var requestSplit = InspectorRequestRaw.Text.Split("\r").Skip(1);
            var cookie = string.Empty;
            var setCookie = new List<string>();
            ViewModel.RequestHeaders.Clear();
            foreach (var row in requestSplit)
            {
                if (row.Contains(": "))
                {
                    var split = row.Split(": ");
                    ViewModel.RequestHeaders.Add(new(split[0], split[1]));
                    
                    if (split[0].ToLower() == "cookie")
                    {
                        cookie = split[1];
                    }
                }
                else
                {
                    break;
                }
            }
            
            var responseSplit = InspectorResponseRaw.Text.Split("\r").Skip(1);
            ViewModel.ResponseHeaders.Clear();
            foreach (var row in responseSplit)
            {
                if (row.Contains(": "))
                {
                    var split = row.Split(": ");
                    ViewModel.ResponseHeaders.Add(new (split[0], split[1]));
                    if (split[0].ToLower() == "set-cookie")
                    {
                        setCookie.Add(split[1]);
                    }
                }
                else
                {
                    break;
                }
            }

            //Params
            var query = ViewModel.SelectPacket.ApiEndPoint.Split("?");
            ViewModel.RequestParams.Clear();
            if (query.Length >= 2)
            {
                var queryGroups = HttpUtility.UrlDecode(query[1]).Split("&");
                foreach (var group in queryGroups)
                {
                    var split = group.Split("=");
                    if (split.Length >= 2)
                    {
                        ViewModel.RequestParams.Add(new(split[0], split[1]));
                    }
                }
            }

            //Cookie
            ViewModel.RequestCookies.Clear();
            if (!string.IsNullOrEmpty(cookie))
            {
                var cookieGroups = cookie.Split(";");
                foreach (var group in cookieGroups)
                {
                    var split = group.Trim().Split("=");
                    ViewModel.RequestCookies.Add(new(split[0], split[1]));
                }
            }

            ViewModel.ResponseSetCookies.Clear();
            if (setCookie.Count != 0)
            {
                foreach (var slice in setCookie)
                {
                    var cookieGroups = slice.Split(";");
                    foreach (var group in cookieGroups)
                    {
                        var split = group.Trim().Split("=");
                        ViewModel.ResponseSetCookies.Add(new(split[0], split[1]));
                    }
                }
            }
        }

        private void packetDataGridRightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            MenuFlyout menu = new MenuFlyout();

            var localizer = App.GetService<ILocalizer>();

            //Copy CurrentCell
            MenuFlyoutItem copyCellItem = new MenuFlyoutItem
            {
                Text = localizer.GetLocalizedString("MainPage_DataGridMenu_Copy"),
            };

            copyCellItem.Click += (s, args) =>
            {
                var originalSource = e.OriginalSource as FrameworkElement;
                var cell = FindParent<DataGridCell>(originalSource);
                if (cell is DataGridCell cellContent) 
                {
                    DataPackage dataPackage = new DataPackage();
                    dataPackage.SetText((cellContent.Content as TextBlock).Text);
                    Clipboard.SetContent(dataPackage);
                }
            };

            //Copy CurrentApiEndPoint
            MenuFlyoutItem copyApiEndPoint = new MenuFlyoutItem
            {
                Text = localizer.GetLocalizedString("MainPage_DataGridMenu_CopyApiEndPoint"),
            };

            copyApiEndPoint.Click += (s, args) =>
            {
                var row = packetDataGrid.SelectedItem;
                if (row is WrappedPacket packet)
                {
                    DataPackage dataPackage = new DataPackage();
                    dataPackage.SetText(packet.ApiEndPoint);
                    Clipboard.SetContent(dataPackage);
                }
            };

            //Delete Current Item
            MenuFlyoutItem deleteItem = new MenuFlyoutItem
            {
                Text = localizer.GetLocalizedString("MainPage_DataGridMenu_Delete"),
            };

            deleteItem.Click += (s, args) =>
            {
                var row = packetDataGrid.SelectedItem;
                if (row is WrappedPacket packet)
                {
                    ViewModel.Packets.Remove(packet);
                }
            };

            menu.Items.Add(copyCellItem);
            menu.Items.Add(copyApiEndPoint);
            menu.Items.Add(deleteItem);
            menu.ShowAt(packetDataGrid, e.GetPosition(packetDataGrid));
        }

        private static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            if (parentObject == null) return null;

            T parent = parentObject as T;
            if (parent != null)
                return parent;
            else
                return FindParent<T>(parentObject);
        }

        private async void StateChange(object sender, RoutedEventArgs e)
        {
            if (ViewModel.EnableDealWith)
            {
                //Pause
                var bar = sender as AppBarButton;
                FontIcon icon = new FontIcon();
                icon.Glyph = "\uE768";
                bar.Icon = icon;
                await ViewModel._netifeService.CloseProbeService();
            }
            else
            {
                //Start
                var bar = sender as AppBarButton;
                FontIcon icon = new FontIcon();
                icon.Glyph = "\uE769";
                bar.Icon = icon;
                await ViewModel._netifeService.StartProbeService();
            }
            ViewModel.EnableDealWith = !ViewModel.EnableDealWith;
        }

        private void InspectorDictionaryClicked(object sender, RightTappedRoutedEventArgs e)
        {
            //TODO Solve the selectItem cannot work
            MenuFlyout menu = new MenuFlyout();
            var pos = sender as DataGrid;
            
            var localizer = App.GetService<ILocalizer>();

            //CopyKey
            MenuFlyoutItem copyKey = new MenuFlyoutItem
            {
                Text = localizer.GetLocalizedString("MainPage_InspectorMenu_CopyKey"),
            };

            copyKey.Click += (s, args) =>
            {
                var row = pos.SelectedItem;
                if (row is ObservablePair<string, string> dic)
                {
                    DataPackage dataPackage = new DataPackage();
                    dataPackage.SetText(dic.Key);
                    Clipboard.SetContent(dataPackage);
                }
            };

            //CopyValue
            MenuFlyoutItem copyValue = new MenuFlyoutItem
            {
                Text = localizer.GetLocalizedString("MainPage_InspectorMenu_CopyValue"),
            };

            copyValue.Click += (s, args) =>
            {
                var row = packetDataGrid.SelectedItem;
                if (row is ObservablePair<string, string> dic)
                {
                    DataPackage dataPackage = new DataPackage();
                    dataPackage.SetText(dic.Value);
                    Clipboard.SetContent(dataPackage);
                }
            };

            menu.Items.Add(copyKey);
            menu.Items.Add(copyValue);
            menu.ShowAt(pos, e.GetPosition(pos));
        }

        private void CommandClick(object sender, RoutedEventArgs e)
        {
            CreateCommandQueryWindows();
        }

        private void CreateCommandQueryWindows()
        {
            var queryWindow = new QueryWindow();
            QueryWindow.Instance = queryWindow;
            var queryFrame = new Frame();
            queryWindow.Content = queryFrame;
            queryWindow.Activate();
            queryFrame.Navigate(typeof(QueryPage));
        }
    }
}
