using Microsoft.Win32;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;
using MenuItem = Wpf.Ui.Controls.MenuItem;

namespace Cluster
{
    /// <summary>
    /// Interaction logic for ProgramInstancesPage.xaml
    /// </summary>
    public partial class ProcessesPage : CustomPage
    {
        public ProcessesPage()
        {
            InitializeComponent();

            Loaded += ProcessesPage_Loaded;
        }

        private void ProcessesPage_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData();
            if (DataContext != null && DataContext is string programName) {
                List<MenuItem> menuItems = GetProgramMenuItems();
                menuItems.ForEach(x => x.IsChecked = x.Header.ToString() == programName);
                FilterProcesses();
            }
        }

        List<Process> Processes;

        public void LoadData(bool skipFilterReload = false) {
            List<Computer> computers = Computer.GetComputers(MainWindow.ClusterPath);
            List<ProgramType> programs = ProgramType.ReadClusterFile(MainWindow.ClusterPath);

            Processes = computers.Aggregate(new List<Process>(), (list, computer) => list.Concat(computer.processes).ToList());

             if (!skipFilterReload) UpdateProgramsMenuItem(programs);
            FilterProcesses();
        }

        public void UpdateProgramsMenuItem(List<ProgramType> programs) {
            menuItemPrograms.Items.Clear();

            MenuItem allItem = new MenuItem() { Header = "All", StaysOpenOnClick = true };
            allItem.Click += ProgramsAllClick;
            menuItemPrograms.Items.Add(allItem);

            menuItemPrograms.Items.Add(new Separator());

            foreach (ProgramType program in programs)
            {
                MenuItem item = new MenuItem() {
                    Header = program.ProgramName,
                    IsCheckable = true,
                    IsChecked = true,
                    StaysOpenOnClick = true
                };
                item.Click += ProgramChecked;
                menuItemPrograms.Items.Add(item);
            }
        }

        private void FilterProcesses() {
            List<string> programNames = GetProgramMenuItems().Where(x => x.IsChecked).Select(x => (string)x.Header).ToList();
            List<Process> filtered = Processes.Where(x => programNames.Contains(x.ProgramName)).ToList();
            icProcesses.ItemsSource = filtered;
            tbCount.Text = $"{filtered.Count} processes ({filtered.Count(x => x.Active)} active)";
        }

        private List<MenuItem> GetProgramMenuItems() {
            List<MenuItem> menuItems = [];

            foreach (var item in menuItemPrograms.Items)
            {
                if (item is MenuItem menuItem && menuItem.IsCheckable)
                {
                    menuItems.Add(menuItem);
                }
            }

            return menuItems;
        }

        private void ProgramChecked(object sender, RoutedEventArgs e)
        {
            List<MenuItem> menuItems = GetProgramMenuItems();
            FilterProcesses();
        }

        private void ProgramsAllClick(object sender, RoutedEventArgs e)
        {
            List<MenuItem> menuItems = GetProgramMenuItems();

            if (menuItems.All(x => x.IsChecked))
            {
                menuItems.ForEach(x => x.IsChecked = false);
            }
            else
            {
                menuItems.ForEach(x => x.IsChecked = true);
            }

            FilterProcesses();
        }

        private void MenuItemExport_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "CSV Files | *.csv";
            sfd.DefaultExt = "csv";
            if (sfd.ShowDialog() == true)
            {
                string[] lines = ["Name;Computer;Status;ProcessorUsage;MemoryUsage", ..Processes.Select(x => x.GetCSVRow())];
                File.WriteAllLines(sfd.FileName, lines);
                _window.RootSnackbarService.Show("Export complete", $"File saved to '{sfd.FileName}'",
                    ControlAppearance.Success, new SymbolIcon(SymbolRegular.Checkmark24), TimeSpan.FromSeconds(3));
                Log.WriteLog(["Processes"], LogType.ExportCSV);
            }
        }

        private void MenuItemNew_Click(object sender, RoutedEventArgs e)
        {
            _window.RootNavigation.NavigateWithHierarchy(typeof(NewInstancePage));
        }

        private void ProcessCard_OnProcessShutdown(object sender, EventArgs e)
        {
            LoadData(true);
        }
    }
}
