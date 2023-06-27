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
            var requestHeaders = new Dictionary<string, string>();
            var cookie = string.Empty;
            var setCookie = new List<string>();
            foreach (var row in requestSplit)
            {
                if (row.Contains(": "))
                {
                    var split = row.Split(": ");
                    requestHeaders.Add(split[0], split[1]);
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
            InspectorRequestHeaders.ItemsSource = requestHeaders;

            var responseSplit = InspectorResponseRaw.Text.Split("\r").Skip(1);
            var responseHeaders = new Dictionary<string, string>();
            foreach (var row in responseSplit)
            {
                if (row.Contains(": "))
                {
                    var split = row.Split(": ");
                    responseHeaders.Add(split[0], split[1]);
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
            InspectorResponseHeaders.ItemsSource = responseHeaders;

            //Params
            var query = ViewModel.SelectPacket.ApiEndPoint.Split("?");
            if (query.Length >= 2)
            {
                var queryGroups = HttpUtility.UrlDecode(query[1]).Split("&");
                var requestParams = new Dictionary<string, string>();
                foreach (var group in queryGroups)
                {
                    var split = group.Split("=");
                    if (split.Length >= 2)
                    {
                        requestParams.Add(split[0], split[1]);
                    }
                }
                InspectorRequestParams.ItemsSource = requestParams;
            }

            //Cookie
            if (!string.IsNullOrEmpty(cookie))
            {
                var cookieGroups = cookie.Split(";");
                var cookies = new Dictionary<string, string>();
                foreach (var group in cookieGroups)
                {
                    var split = group.Trim().Split("=");
                    cookies.Add(split[0], split[1]);
                }
                InspectorRequestCookies.ItemsSource = cookies;
            }

            if (setCookie.Count != 0)
            {
                foreach (var slice in setCookie)
                {
                    var cookieGroups = slice.Split(";");
                    var cookies = new Dictionary<string, string>();
                    foreach (var group in cookieGroups)
                    {
                        var split = group.Trim().Split("=");
                        cookies.Add(split[0], split[1]);
                    }
                    InspectorResponseCookies.ItemsSource = cookies;
                }
            }
        }
    }
}
