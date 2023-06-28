using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace NetifePanel.Windows
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class QueryWindow : Window
    {
        public QueryWindow()
        {
            this.InitializeComponent();
            SystemBackdrop = new MicaBackdrop()
            { Kind = MicaKind.BaseAlt };

            
        }
        public static QueryWindow Instance { get; set; }
    }
}
