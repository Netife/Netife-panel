using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;


// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace NetifePanel
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            SystemBackdrop = new MicaBackdrop()
            { Kind = MicaKind.BaseAlt };
        }

        //WindowsSystemDispatcherQueueHelper m_wsdqHelper; // See separate sample below for implementation
        //DesktopAcrylicController m_acrylicController;
        //SystemBackdropConfiguration m_configurationSource;
        //MicaController m_micaController;
        //
        //public bool TrySetAcrylicBackdrop()
        //{
        //    if (DesktopAcrylicController.IsSupported())
        //    {
        //        m_wsdqHelper = new WindowsSystemDispatcherQueueHelper();
        //        m_wsdqHelper.EnsureWindowsSystemDispatcherQueueController();
        //
        //        // Hooking up the policy object
        //        m_configurationSource = new SystemBackdropConfiguration();
        //        this.Activated += Window_Activated;
        //        this.Closed += Window_Closed;
        //        ((FrameworkElement)this.Content).ActualThemeChanged += Window_ThemeChanged;
        //
        //        // Initial configuration state.
        //        m_configurationSource.IsInputActive = true;
        //        SetConfigurationSourceTheme();
        //
        //        m_acrylicController = new DesktopAcrylicController();
        //
        //        // Enable the system backdrop.
        //        // Note: Be sure to have "using WinRT;" to support the Window.As<...>() call.
        //        m_acrylicController.AddSystemBackdropTarget(this.As<Microsoft.UI.Composition.ICompositionSupportsSystemBackdrop>());
        //        m_acrylicController.SetSystemBackdropConfiguration(m_configurationSource);
        //        return true; // succeeded
        //    }
        //
        //    return false; // Acrylic is not supported on this system
        //}
        //
        //public bool TrySetMicaBackdrop()
        //{
        //    if (MicaController.IsSupported())
        //    {
        //        m_wsdqHelper = new WindowsSystemDispatcherQueueHelper();
        //        m_wsdqHelper.EnsureWindowsSystemDispatcherQueueController();
        //
        //        // Hooking up the policy object
        //        m_configurationSource = new SystemBackdropConfiguration();
        //        this.Activated += Window_Activated;
        //        this.Closed += Window_Closed;
        //        ((FrameworkElement)this.Content).ActualThemeChanged += Window_ThemeChanged;
        //
        //        // Initial configuration state.
        //        m_configurationSource.IsInputActive = true;
        //        SetConfigurationSourceTheme();
        //
        //        m_micaController = new MicaController();
        //
        //        // Enable the system backdrop.
        //        // Note: Be sure to have "using WinRT;" to support the Window.As<...>() call.
        //        m_micaController.AddSystemBackdropTarget(this.As<Microsoft.UI.Composition.ICompositionSupportsSystemBackdrop>());
        //        m_micaController.SetSystemBackdropConfiguration(m_configurationSource);
        //        return true; // succeeded
        //    }
        //
        //    return false; // Mica is not supported on this system
        //}
        //
        //private void Window_Activated(object sender, WindowActivatedEventArgs args)
        //{
        //    m_configurationSource.IsInputActive = args.WindowActivationState != WindowActivationState.Deactivated;
        //}
        //
        //private void Window_Closed(object sender, WindowEventArgs args)
        //{
        //    // Make sure any Mica/Acrylic controller is disposed so it doesn't try to
        //    // use this closed window.
        //    if (m_acrylicController != null)
        //    {
        //        m_acrylicController.Dispose();
        //        m_acrylicController = null;
        //    }
        //    this.Activated -= Window_Activated;
        //    m_configurationSource = null;
        //}
        //
        //private void Window_ThemeChanged(FrameworkElement sender, object args)
        //{
        //    if (m_configurationSource != null)
        //    {
        //        SetConfigurationSourceTheme();
        //    }
        //}
        //
        //private void SetConfigurationSourceTheme()
        //{
        //    switch (((FrameworkElement)this.Content).ActualTheme)
        //    {
        //        case ElementTheme.Dark: m_configurationSource.Theme = SystemBackdropTheme.Dark; break;
        //        case ElementTheme.Light: m_configurationSource.Theme = SystemBackdropTheme.Light; break;
        //        case ElementTheme.Default: m_configurationSource.Theme = SystemBackdropTheme.Default; break;
        //    }
        //}
    }

    //class WindowsSystemDispatcherQueueHelper
    //{
    //    [StructLayout(LayoutKind.Sequential)]
    //    struct DispatcherQueueOptions
    //    {
    //        internal int dwSize;
    //        internal int threadType;
    //        internal int apartmentType;
    //    }
    //
    //    [DllImport("CoreMessaging.dll")]
    //    private static extern int CreateDispatcherQueueController([In] DispatcherQueueOptions options, [In, Out, MarshalAs(UnmanagedType.IUnknown)] ref object dispatcherQueueController);
    //
    //    object m_dispatcherQueueController = null;
    //    public void EnsureWindowsSystemDispatcherQueueController()
    //    {
    //        if (Windows.System.DispatcherQueue.GetForCurrentThread() != null)
    //        {
    //            // one already exists, so we'll just use it.
    //            return;
    //        }
    //
    //        if (m_dispatcherQueueController == null)
    //        {
    //            DispatcherQueueOptions options;
    //            options.dwSize = Marshal.SizeOf(typeof(DispatcherQueueOptions));
    //            options.threadType = 2;    // DQTYPE_THREAD_CURRENT
    //            options.apartmentType = 2; // DQTAT_COM_STA
    //
    //            CreateDispatcherQueueController(options, ref m_dispatcherQueueController);
    //        }
    //    }
    //}
}
