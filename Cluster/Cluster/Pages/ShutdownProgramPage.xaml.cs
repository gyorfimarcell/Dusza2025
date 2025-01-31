using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Shapes;

namespace Cluster
{
    /// <summary>
    /// Interaction logic for ShutdownProgram.xaml
    /// </summary>
    public partial class ShutdownProgramPage : Page
    {
        List<ProgramType> programs = new();
        string path = string.Empty;
        public ShutdownProgramPage()
        {
            InitializeComponent();
            path = MainWindow.ClusterPath;
            programs = ProgramType.ReadClusterFile(path);
            lbCurrentPrograms.ItemsSource = programs.Select(x => x.ProgramName).ToList();
        }

        private void lbCurrentPrograms_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnShutdown.Visibility = lbCurrentPrograms.SelectedItem != null ? Visibility.Visible : Visibility.Hidden;
        }

        private void btnShutdown_Click(object sender, RoutedEventArgs e)
        {
            bool result = ProgramType.ShutdownProgram(path, programs, lbCurrentPrograms.SelectedItem.ToString());
            if (result)
            {
                programs = ProgramType.ReadClusterFile(path);
                lbCurrentPrograms.ItemsSource = programs.Select(x => x.ProgramName).ToList();
                MessageBox.Show("Program has been successfully shut down!");
            }
        }
    }
}
