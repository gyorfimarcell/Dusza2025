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
    public partial class ShutdownProgramInstancePage : Page
    {
        List<Process> programs = new();
        string path = string.Empty;
        public ShutdownProgramInstancePage()
        {
            InitializeComponent();
            path = MainWindow.path;
            programs = Computer.GetComputers(path)
                .Aggregate(new List<Process>(), (list, computer) => list.Concat(computer.processes).ToList());
            lbCurrentPrograms.ItemsSource = programs.Select(x => x.FileName).ToList();
        }

        private void lbCurrentPrograms_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnShutdown.Visibility = lbCurrentPrograms.SelectedItem != null ? Visibility.Visible : Visibility.Hidden;
        }

        private void btnShutdown_Click(object sender, RoutedEventArgs e)
        {
            string fileName = programs.Find(x => x.FileName == lbCurrentPrograms.SelectedItem.ToString()).FileName;
            string computerName = Computer.GetComputers(path).Find(x => x.processes.Select(x => x.FileName).Contains(fileName)).Name;
            File.Delete($@"{path}\{computerName}\{fileName}");
            programs = programs.Where(x => x.FileName != fileName).ToList();
            lbCurrentPrograms.ItemsSource = programs.Select(x => x.FileName).ToList();
        }
    }
}
