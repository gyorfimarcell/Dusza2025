using Microsoft.Win32;
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
using System.Windows.Shapes;

namespace Cluster
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public static string path = "";

        List<ProgramType> programs = new();
        public MainWindow()
        {
            InitializeComponent();
            OpenFolderDialog ofd = new OpenFolderDialog();
            if (ofd.ShowDialog() == true)
            {
                path = ofd.FolderName;
                programs = ProgramType.ReadClusterFile(ofd.FolderName);
                if (programs == null)
                {
                    MessageBox.Show("The chosen folder doesn't contain a .klaszter file.");
                    Close();
                }
                else
                {
                    //MessageBox.Show(programs.Count().ToString());
                    //lblPath.Content = $"Path: {path}";
                    ClusterHealth health = new(Computer.GetComputers(path), programs);
                    if (!health.Ok) {
                        MessageBox.Show($"This cluster has errors:\n{String.Join("\n", health.Errors.Select(x => $" - {x}"))}");
                    }
                }
            }
            else
            {
                Close();
            }
        }
    }
}