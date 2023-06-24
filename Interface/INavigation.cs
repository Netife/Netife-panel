namespace NetifePanel.Interface
{
    interface INavigation
    {
        public string CurrentPage { get; }

        public bool CanGoBack { get; }

        void NavigateTo(string page, object paramters);
        void NavigateTo(string page);
        void GoBack();

    }
}
