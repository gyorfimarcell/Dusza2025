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
    public partial class MainWindow : Window
    {
        List<ProgramType> programs = new();
        string path = "";
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
                    lblPath.Content = $"Path: {path}";
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

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            System.Environment.Exit(0);
        }

        private void btnMonitoring_Click(object sender, RoutedEventArgs e)
        {
            programs = ProgramType.ReadClusterFile(path);
            MonitoringWindow w = new(Computer.GetComputers(path), programs);
            w.Show();
        }

        private void btnShutdownProgram_Click(object sender, RoutedEventArgs e)
        {
            programs = ProgramType.ReadClusterFile(path);
            ShutdownProgram shutdownProgram = new(programs, path);
            shutdownProgram.Show();
        }

        private void btnModifyProgram_Click(object sender, RoutedEventArgs e)
        {
            programs = ProgramType.ReadClusterFile(path);
            ModifyProgram modifyProgram = new(programs, path);
            modifyProgram.Show();
        }

        private void btnShutdownInstance_Click(object sender, RoutedEventArgs e)
        {
            programs = ProgramType.ReadClusterFile(path);
            ShutdownProgramInstance shutdownProgramInstance = new(Computer.GetComputers(path)
                .Aggregate(new List<Process>(), (list, computer) => list.Concat(computer.processes).ToList()), path);
            shutdownProgramInstance.Show();
        }

        private void btnAddComputer_Click(object sender, RoutedEventArgs e)
        {
            programs = ProgramType.ReadClusterFile(path);
            AddComputerWindow w = new(path);
            w.Show();
        }

        private void btnDeleteComputer_Click(object sender, RoutedEventArgs e)
        {
            programs = ProgramType.ReadClusterFile(path);
            DeleteComputerWindow w = new(path);
            w.Show();
        }

        private void btnRunInstance_Click(object sender, RoutedEventArgs e)
        {
            programs = ProgramType.ReadClusterFile(path);
            NewInstanceWindow w = new(path);
            w.Show();
        }
    }
}