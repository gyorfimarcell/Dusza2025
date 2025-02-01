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

namespace Cluster
{
    /// <summary>
    /// Interaction logic for NewInstanceWindow.xaml
    /// </summary>
    public partial class NewInstancePage : Page
    {
        string path;
        List<string> instances;

        public NewInstancePage()
        {
            InitializeComponent();

            path = MainWindow.ClusterPath;

            instances = ProgramType.ReadClusterFile(path).Select(x => x.ProgramName).ToList();
            cbProgram.ItemsSource = instances;
            cbComputer.ItemsSource = Computer.GetComputers(path).Select(x => x.Name);
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (cbProgram.SelectedIndex == -1 || cbComputer.SelectedIndex == -1) {
                return;
            }

            ProgramType program = ProgramType.ReadClusterFile(path).Find(x => x.ProgramName == cbProgram.SelectedItem.ToString());
            Computer computer = Computer.GetComputers(path).Find(x => x.Name == cbComputer.SelectedItem.ToString());

            int cpuUsage = computer.processes.Where(x => x.Active).Sum(x => x.ProcessorUsage);
            int memoryUsage = computer.processes.Where(x => x.Active).Sum(x => x.MemoryUsage);

            if (cpuUsage + program.CpuMilliCore > computer.ProcessorCore || memoryUsage + program.Memory > computer.RamCapacity) {
                MessageBox.Show("This computer doesn't have enough resources!");
                return;
            }

            Process process = new(program.ProgramName, program.CpuMilliCore, program.Memory);
            process.Write(Path.Combine(path, computer.Name));
            //Close();
        }

        private void cbProgram_KeyUp(object sender, KeyEventArgs e)
        {
            spNewInstance.Visibility = !instances.Contains(cbProgram.Text) && cbProgram.Text != string.Empty ? Visibility.Visible : Visibility.Collapsed;
        }

        private void cbProgram_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            spNewInstance.Visibility = !instances.Contains(cbProgram.SelectedValue) && cbProgram.Text != string.Empty ? Visibility.Visible : Visibility.Collapsed;
        }

        private void NewInstanceDetailsChanged(object sender, TextChangedEventArgs e)
        {
            //cbComputer.ItemsSource = Computer.GetComputers(path).Where(x => x.HasEnoughCore()).Select(x => x.Name);
        }
    }
}
