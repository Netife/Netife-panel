using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using NetifePanel.Interface;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetifePanel.Serivces
{
    class NavigateService : INavigation
    {
        private readonly IDictionary<string, Type> _pages = new ConcurrentDictionary<string, Type>();

        public const string RootPage = "(Root)";

        public const string UnKnownPage = "(UnKnownPage)";
        private static Frame AppFrame => (Frame)Window.Current.Content;
        
        public void Configure(string page, Type type)
        {
            if (_pages.Keys.Any(sp => sp == page))
            {
                throw new ArgumentException("Page cannot have the same key");
            }

            _pages[page] = type;
        }

        public string CurrentPage 
        {
            get
            {
                if (AppFrame.BackStackDepth == 0)
                    return RootPage;

                if (AppFrame.Content == null)
                    return UnKnownPage;

                var type = AppFrame.Content.GetType();

                if (_pages.Values.Any(v => v != type))
                    return UnKnownPage;

                var item = _pages.Single(i => i.Value == type);

                return item.Key;
            }
        
        }

        public bool CanGoBack => AppFrame.CanGoBack;

        public void GoBack()
        {
            if (AppFrame?.CanGoBack == true)
            {
                AppFrame.GoBack();
            }
        }

        public void NavigateTo(string page, object paramters)
        {
            if (!_pages.ContainsKey(page))
            {
                throw new ArgumentException($"Cannot navigate to a UNKNOWN page : {nameof(NavigateTo)}.");
            }

            AppFrame.Navigate(_pages[page], paramters);
        }

        public void NavigateTo(string page) => NavigateTo(page, null);
    }
}
