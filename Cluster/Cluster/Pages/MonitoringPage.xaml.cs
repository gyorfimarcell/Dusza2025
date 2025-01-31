using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Interaction logic for MonitoringWindow.xaml
    /// </summary>
    public partial class MonitoringPage : Page
    {
        List<Computer> computers;
        List<Process> processes;

        public MonitoringPage()
        {
            this.computers = Computer.GetComputers(MainWindow.path);
            List<ProgramType> programs = ProgramType.ReadClusterFile(MainWindow.path);

            processes = computers.Aggregate(new List<Process>(), (list, computer) => list.Concat(computer.processes).ToList());

            InitializeComponent();

            lblActive.Content = $"Active processes: {processes.Count(x => x.Active)}";
            lblInctive.Content = $"Inactive processes: {processes.Count(x => !x.Active)}";

            ClusterHealth health = new(computers, programs);
            lblStatus.Content = $"Cluster {(health.Ok ? "is healthy." : "has errors!")}";

            dgComputers.ItemsSource = computers.Select(x => new ComputerRow(x));

            cbProgram.ItemsSource = programs.Select(x => x.ProgramName).Order().Prepend("--All--");
            cbProgram.SelectedIndex = 0;
        }

        private void cbProgram_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            dgPrograms.ItemsSource = processes.Where(x => cbProgram.SelectedIndex == 0 ? true : x.ProgramName == cbProgram.SelectedItem.ToString()).Select(x => new ProcessRow(x, computers));
            lblProgramCount.Content = $"{dgPrograms.Items.Count} process{(dgPrograms.Items.Count > 1 ? "es" : "")}";
        }
    }

    public class ComputerRow {
        Computer computer;

        public string Name => computer.Name;

        public int ProcessorCapacity => computer.ProcessorCore;
        private int _processorUsage => computer.processes.Where(x => x.Active).Sum(x => x.ProcessorUsage);
        public string ProcessorUsage => $"{_processorUsage} ({Math.Round(_processorUsage / (double)ProcessorCapacity * 100)}%)";

        public int MemoryCapacity => computer.RamCapacity;
        private int _memoryUsage => computer.processes.Where(x => x.Active).Sum(x => x.MemoryUsage);
        public string MemoryUsage => $"{_memoryUsage} ({Math.Round(_memoryUsage / (double)MemoryCapacity * 100)}%)";

        public ComputerRow(Computer computer) {
            this.computer = computer;
        }
    }

    public class ProcessRow {
        Process process;
        Computer computer;

        public string Name => process.FileName;
        public string Computer => computer.Name;
        public string Status => process.Active ? "Active" : "Inactive";
        public int ProcessorUsage => process.ProcessorUsage;
        public int MemoryUsage => process.MemoryUsage;


        public ProcessRow(Process process, List<Computer> computers) {
            this.process = process;
            this.computer = computers.Find(x => x.processes.Contains(process));
        }
    }
}
