using Microsoft.Win32;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace Cluster
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public static String ClusterPath { get; private set; } = "";

        public MainWindow()
        {
            InitializeComponent();
            
        }

        private void loadNavItem_Click(object sender, RoutedEventArgs e)
        {
            OpenFolderDialog ofd = new OpenFolderDialog();
            if (ofd.ShowDialog() == true)
            {
                ClusterPath = ofd.FolderName;
                List<ProgramType> programs = ProgramType.ReadClusterFile(ofd.FolderName);
                if (programs == null)
                {
                    MessageBox.Show("The chosen folder doesn't contain a .klaszter file.");
                }
                else
                {
                    lblPath.Content = $"Cluster: {Path.GetFileName(ClusterPath)}";
                    ClusterHealth health = new(Computer.GetComputers(ClusterPath), programs);
                    if (!health.Ok)
                    {
                        MessageBox.Show($"This cluster has errors:\n{String.Join("\n", health.Errors.Select(x => $" - {x}"))}");
                    }
                }
            }
        }
    }
}