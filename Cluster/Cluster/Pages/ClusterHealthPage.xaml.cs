using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Wpf.Ui.Controls;

namespace Cluster
{
    /// <summary>
    /// Interaction logic for ClusterHealthPage.xaml
    /// </summary>
    public partial class ClusterHealthPage : Page
    {
        public ClusterHealthPage()
        {
            InitializeComponent();
            Loaded += ClusterHealthPage_Loaded;
        }

        /// <summary>
        /// Checks the health of the cluster and displays the results
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClusterHealthPage_Loaded(object sender, RoutedEventArgs e)
        {
            ClusterHealth health = new(Computer.GetComputers(MainWindow.ClusterPath), ProgramType.ReadClusterFile(MainWindow.ClusterPath));
            if (health.Ok)
            {
                HealthyInfobar.IsOpen = true;
            }
            else {
                foreach (string error in health.Errors)
                {
                    InfoBar infoBar = new()
                    {
                        Title = "Error",
                        Message = error,
                        IsClosable = false,
                        Severity = InfoBarSeverity.Error,
                        IsOpen = true,
                        Margin = new Thickness(0, 0, 0, 8)
                    };
                    spErrors.Children.Add(infoBar);
                }
            }
        }
    }
}
