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
        readonly string path;
        readonly List<ProgramType> instances;

        public NewInstancePage()
        {
            InitializeComponent();

            path = MainWindow.ClusterPath;

            instances = ProgramType.ReadClusterFile(path).ToList();
            cbProgram.ItemsSource = instances.Select(x => x.ProgramName).ToList();
            cbComputer.ItemsSource = Computer.GetComputers(path).Select(x => x.Name);
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (cbComputer.SelectedIndex == -1 || cbProgram.SelectedIndex == -1)
            {
                MessageBox.Show("Please fill out all the fields!");
                return;
            }

            ProgramType program = ProgramType.ReadClusterFile(path).Find(x => x.ProgramName == cbProgram.SelectedItem.ToString());

            Computer computer = Computer.GetComputers(path).Find(x => x.Name == cbComputer.SelectedItem.ToString());

            int cpuUsage = computer.processes.Where(x => x.Active).Sum(x => x.ProcessorUsage);
            int memoryUsage = computer.processes.Where(x => x.Active).Sum(x => x.MemoryUsage);

            if (cpuUsage + program.CpuMilliCore > computer.ProcessorCore || memoryUsage + program.Memory > computer.RamCapacity)
            {
                MessageBox.Show("This computer doesn't have enough resources!");
                return;
            }

            Process process = new(program.ProgramName, program.CpuMilliCore, program.Memory);
            process.Write(Path.Combine(path, computer.Name));

            Log.WriteLog([$"{process.FileName}", $"{process.StartTime:yyyy.MM.dd. HH:mm:ss}", $"{process.Active}", $"{process.ProcessorUsage}", $"{process.MemoryUsage}"], LogType.RunProgramInstance);
        }

        private void cbProgram_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ProgramType program = instances.Find(x => x.ProgramName == cbProgram.SelectedItem.ToString());
            cbComputer.ItemsSource = Computer.GetComputers(path).Where(x => x.HasEnoughCore(program.CpuMilliCore) && x.HasEnoughRam(program.Memory)).Select(x => x.Name);
        }
    }
}
