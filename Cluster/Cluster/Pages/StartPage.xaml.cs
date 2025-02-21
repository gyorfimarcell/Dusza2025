using System.Windows;

namespace Cluster
{
    /// <summary>
    /// Interaction logic for StartPage.xaml
    /// </summary>
    public partial class StartPage
    {
        public StartPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Open the cluster selection dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EmptyStatus_OnClick(object sender, EventArgs e)
        {
            MainWindow window = (MainWindow)Window.GetWindow(this)!;
            window.OpenClusterSelectionDialog();
        }
    }
}