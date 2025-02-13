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
    public partial class ClusterHealthPage : CustomPage
    {
        ClusterHealth health;
        public ClusterHealthPage()
        {
            InitializeComponent();
            health = new(Computer.GetComputers(MainWindow.ClusterPath), ProgramType.ReadClusterFile(MainWindow.ClusterPath));
            Loaded += ClusterHealthPage_Loaded;
        }

        private void ClusterHealthPage_Loaded(object sender, RoutedEventArgs e)
        {
            health = new(Computer.GetComputers(MainWindow.ClusterPath), ProgramType.ReadClusterFile(MainWindow.ClusterPath));
            if (health.Ok)
            {
                //System.Windows.MessageBox.Show("OK");
                HealthyInfobar.IsOpen = true;
                spFixIssues.Visibility = Visibility.Hidden;
            }
            else {
                spFixIssues.Visibility = Visibility.Visible;
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

        private void FixIssues_Click(object sender, RoutedEventArgs e)
        {
            ClusterHealth.FixIssues();
            var clusterOk = spErrors.Children[0];
            spErrors.Children.Clear();
            spErrors.Children.Add(clusterOk);
            ClusterHealthPage_Loaded(new(), new());
        }
    }
}
