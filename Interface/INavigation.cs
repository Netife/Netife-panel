using Microsoft.UI.Dispatching;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NetifePanel.Interface
{
    public interface INavigation
    {
        public string CurrentPage { get; }

        public bool CanGoBack { get; }

        public ObservableCollection<string> BreadStack { get; }

        void NavigateTo(string page, object paramters);
        void NavigateTo(string page);
        void GoBack();
        void NavigateInUIThread(DispatcherQueue dispatcherQueue, string page, object paramters = null);

        void PopBreadPath();

        void PushBreadPath(string page);

        public void ResetBreadPath();
    }
}
