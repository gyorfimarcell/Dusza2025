﻿using System.IO;
using System.Windows;
using System.Windows.Controls;
using Wpf.Ui.Controls;

namespace Cluster
{
    /// <summary>
    /// Interaction logic for NewInstanceWindow.xaml
    /// </summary>
    public partial class NewInstancePage
    {
        private readonly MainWindow _window;
        private readonly string path;
        private readonly List<ProgramType> instances;

        public NewInstancePage()
        {
            InitializeComponent();

            _window = (MainWindow)Application.Current.MainWindow!;
            path = MainWindow.ClusterPath;

            instances = ProgramType.ReadClusterFile(path).ToList();
            cbProgram.ItemsSource = instances.Select(x => x.ProgramName).ToList();
            cbComputer.ItemsSource = Computer.GetComputers(path).Select(x => x.Name);
        }

        /// <summary>
        /// Add new instance to the selected computer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (cbComputer.SelectedIndex == -1 || cbProgram.SelectedIndex == -1)
            {
                _window.RootSnackbarService.Show(TranslationSource.T("Errors.Error"),
                    TranslationSource.T("Errors.MissingFields"), ControlAppearance.Danger,
                    new SymbolIcon(SymbolRegular.Warning24), TimeSpan.FromSeconds(10));
                return;
            }

            ProgramType program = ProgramType.ReadClusterFile(path)
                .Find(x => x.ProgramName == cbProgram.SelectedItem.ToString())!;

            Computer computer = Computer.GetComputers(path).Find(x => x.Name == cbComputer.SelectedItem.ToString())!;

            int cpuUsage = computer.processes.Where(x => x.Active).Sum(x => x.ProcessorUsage);
            int memoryUsage = computer.processes.Where(x => x.Active).Sum(x => x.MemoryUsage);

            if ((cpuUsage + program.CpuMilliCore > computer.ProcessorCore ||
                 memoryUsage + program.Memory > computer.RamCapacity) && tsActive.IsChecked == true)
            {
                _window.RootSnackbarService.Show(TranslationSource.T("Errors.Error"),
                    TranslationSource.T("Errors.NotEnoughResources"), ControlAppearance.Danger,
                    new SymbolIcon(SymbolRegular.Warning24), TimeSpan.FromSeconds(10));
                return;
            }

            Process process = new(program.ProgramName, program.CpuMilliCore, program.Memory,
                tsActive.IsChecked == true);
            process.Write(Path.Combine(path, computer.Name));

            _window.RootSnackbarService.Show(TranslationSource.T("NewInstancePage.Success.Title"),
                TranslationSource.Instance.WithParam("NewInstancePage.Success.Text", process.FileName, computer.Name),
                ControlAppearance.Success, new SymbolIcon(SymbolRegular.Checkmark24), TimeSpan.FromSeconds(10));

            Log.WriteLog(
            [
                $"{process.FileName}", $"{process.StartTime:yyyy.MM.dd. HH:mm:ss}", $"{process.Active}",
                $"{process.ProcessorUsage}", $"{process.MemoryUsage}", computer.Name
            ], LogType.RunProgramInstance);
        }

        /// <summary>
        /// Changing the items in the computer combobox based on the selected program
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbProgram_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ProgramType program = instances.Find(x => x.ProgramName == cbProgram.SelectedItem.ToString())!;
            cbComputer.ItemsSource = tsActive.IsChecked == true
                ? Computer.GetComputers(path)
                    .Where(x => x.HasEnoughCore(program.CpuMilliCore) && x.HasEnoughRam(program.Memory))
                    .Select(x => x.Name)
                : Computer.GetComputers(path).Select(x => x.Name);
        }

        private void tsActive_CheckChanged(object sender, RoutedEventArgs e)
        {
            if (cbProgram.SelectedIndex == -1)
            {
                return;
            }

            ProgramType program = instances.Find(x => x.ProgramName == cbProgram.SelectedItem.ToString())!;
            cbComputer.ItemsSource = tsActive.IsChecked == true
                ? Computer.GetComputers(path)
                    .Where(x => x.HasEnoughCore(program.CpuMilliCore) && x.HasEnoughRam(program.Memory))
                    .Select(x => x.Name)
                : Computer.GetComputers(path).Select(x => x.Name);
        }
    }
}