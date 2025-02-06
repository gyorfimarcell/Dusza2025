using Microsoft.Win32;
using System;
using System.Collections;
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
using static Cluster.ProgramsPage;
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

        ProcessesPageSort sort = ProcessesPageSort.Id;
        ProcessesPageStatus statusFilter = ProcessesPageStatus.All;

        public void LoadData(bool skipFilterReload = false) {
            List<Computer> computers = Computer.GetComputers(MainWindow.ClusterPath);
            List<ProgramType> programs = ProgramType.ReadClusterFile(MainWindow.ClusterPath);

            Processes = computers.SelectMany(x => x.processes).ToList();

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

        internal enum ProcessesPageSort {
            Program,
            Id,
            ProcessorUsage,
            MemoryUsage,
            Start
        }
        
        internal enum  ProcessesPageStatus
        {
            All,
            Active,
            Inactive,
        }

        private void FilterProcesses() {
            List<string> programNames = GetProgramMenuItems().Where(x => x.IsChecked).Select(x => (string)x.Header).ToList();

            IEnumerable<Process> filtered = Processes;
            filtered = filtered.Where(x => programNames.Contains(x.ProgramName)).ToList();
            filtered = filtered.Where(x => x.FileName.Contains(tbFilter.Text, StringComparison.InvariantCultureIgnoreCase));

            filtered = statusFilter switch
            {
                ProcessesPageStatus.All => filtered,
                ProcessesPageStatus.Active => filtered.Where(x => x.Active),
                ProcessesPageStatus.Inactive => filtered.Where(x => !x.Active),
                _ => throw new NotImplementedException()
            };

            filtered = sort switch
            {
                ProcessesPageSort.Program => filtered.OrderBy(x => x.ProgramName),
                ProcessesPageSort.Id => filtered.OrderBy(x => x.ProcessId),
                ProcessesPageSort.ProcessorUsage => filtered.OrderBy(x => x.ProcessorUsage),
                ProcessesPageSort.MemoryUsage => filtered.OrderBy(x => x.MemoryUsage),
                ProcessesPageSort.Start => filtered.OrderBy(x => x.StartTime),
                _ => throw new NotImplementedException(),
            };

            if (MenuItemSortOrder.IsChecked) filtered = filtered.Reverse();
            
            List<Process> filteredList = filtered.ToList();

            icProcesses.ItemsSource = filteredList;
            tbCount.Text = $"{filteredList.Count} processes ({filteredList.Count(x => x.Active)} active)";
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
            }
        }

        private void MenuItemNew_Click(object sender, RoutedEventArgs e)
        {
            _window.RootNavigation.NavigateWithHierarchy(typeof(NewInstancePage));
        }

        private void ProcessCard_OnProcessChange(object sender, EventArgs e)
        {
            LoadData(true);
        }

        private void tbFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterProcesses();
        }

        private void MenuItemSort_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;
            sort = (ProcessesPageSort)Enum.Parse(typeof(ProcessesPageSort), (string)menuItem.Tag);
            FilterProcesses();
        }

        private void MenuItemSortOrder_Click(object sender, RoutedEventArgs e)
        {
            FilterProcesses();
        }

        private void MenuItemStatus_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;
            statusFilter = (ProcessesPageStatus)Enum.Parse(typeof(ProcessesPageStatus), (string)menuItem.Tag);
            FilterProcesses();
        }
    }
}
