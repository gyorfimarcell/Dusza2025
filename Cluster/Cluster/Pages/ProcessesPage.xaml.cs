using Cluster.ChartModels;
using LiveChartsCore.Kernel;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore;
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
            _window.PropertyChanged += _window_PropertyChanged;
        }

        /// <summary>
        /// Load data and filter processes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProcessesPage_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData();
            if (DataContext != null && DataContext is string programName)
            {
                List<MenuItem> menuItems = GetProgramMenuItems();
                menuItems.ForEach(x => x.IsChecked = x.Header.ToString() == programName);
                FilterProcesses();
            }
        }

        /// <summary>
        /// Update chart values
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _window_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MainWindow.DarkMode))
            {
                barPrograms.CoreChart.Update(new ChartUpdateParams { IsAutomaticUpdate = false, Throttling = false });
                barPrograms.LegendTextPaint = (SolidColorPaint?)LiveCharts.DefaultSettings.LegendTextPaint;

                pieComputers.CoreChart.Update(new ChartUpdateParams { IsAutomaticUpdate = false, Throttling = false });
            }
        }

        List<Process> Processes;

        ProcessesPageSort sort = ProcessesPageSort.Id;
        ProcessesPageStatus statusFilter = ProcessesPageStatus.All;

        /// <summary>
        /// Load data from cluster and filter processes
        /// </summary>
        /// <param name="skipFilterReload">Deciding to reload the program menu item or not</param>
        public void LoadData(bool skipFilterReload = false) 
        {
            List<Computer> computers = Computer.GetComputers(MainWindow.ClusterPath);
            List<ProgramType> programs = ProgramType.ReadClusterFile(MainWindow.ClusterPath);

            Processes = computers.SelectMany(x => x.processes).ToList();

            if (!skipFilterReload) UpdateProgramsMenuItem(programs);
            FilterProcesses();
        }

        /// <summary>
        /// Update the charts
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void UpdateCharts()
        {
            ProcessesPageCharts data = new(icProcesses.Items.Cast<Process>());
            barPrograms.Series = statusFilter switch
            {
                ProcessesPageStatus.All => [data.ProgramsActiveSeries, data.ProgramInactiveSeries],
                ProcessesPageStatus.Active => [data.ProgramsActiveSeries],
                ProcessesPageStatus.Inactive => [data.ProgramInactiveSeries],
                _ => throw new NotImplementedException()
            };
            barPrograms.LegendPosition = statusFilter == ProcessesPageStatus.All ? LiveChartsCore.Measure.LegendPosition.Bottom : LiveChartsCore.Measure.LegendPosition.Hidden;
            barPrograms.XAxes = data.ProgramsAxes;
            barPrograms.YAxes = data.ProgramsYAxes;
            pieComputers.Series = data.ComputersSeries;

            chartsRow.Height = icProcesses.Items.Count != 0 ? new GridLength(250) : new GridLength(0);
        }

        /// <summary>
        /// Update the program menu item
        /// </summary>
        /// <param name="programs">List of programs</param>
        public void UpdateProgramsMenuItem(List<ProgramType> programs) 
        {
            menuItemPrograms.Items.Clear();

            MenuItem allItem = new MenuItem() { Header = "All", StaysOpenOnClick = true };
            allItem.Click += ProgramsAllClick;
            menuItemPrograms.Items.Add(allItem);

            menuItemPrograms.Items.Add(new Separator());

            foreach (ProgramType program in programs)
            {
                MenuItem item = new MenuItem()
                {
                    Header = program.ProgramName,
                    IsCheckable = true,
                    IsChecked = true,
                    StaysOpenOnClick = true
                };
                item.Click += ProgramChecked;
                menuItemPrograms.Items.Add(item);
            }
        }

        internal enum ProcessesPageSort
        {
            Program,
            Id,
            ProcessorUsage,
            MemoryUsage,
            Start
        }

        internal enum ProcessesPageStatus
        {
            All,
            Active,
            Inactive,
        }

        /// <summary>
        /// Filter processes based on the selected program and filter text
        /// </summary>
        /// <exception cref="NotImplementedException">If filtering is not implemented</exception>
        private void FilterProcesses() 
        {
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
            UpdateCharts();
        }

        /// <summary>
        /// Get the program menu items
        /// </summary>
        /// <returns>The list of menu items</returns>
        private List<MenuItem> GetProgramMenuItems() 
        {
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

        /// <summary>
        /// Program checked event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProgramChecked(object sender, RoutedEventArgs e)
        {
            List<MenuItem> menuItems = GetProgramMenuItems();
            FilterProcesses();
        }

        /// <summary>
        /// All programs clicked event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Export the processes to a CSV file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemExport_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "CSV Files | *.csv";
            sfd.DefaultExt = "csv";
            if (sfd.ShowDialog() == true)
            {
                string[] lines = ["Name;Computer;Status;ProcessorUsage;MemoryUsage", .. Processes.Select(x => x.GetCSVRow())];
                File.WriteAllLines(sfd.FileName, lines);
                _window.RootSnackbarService.Show("Export complete", $"File saved to '{sfd.FileName}'",
                    ControlAppearance.Success, new SymbolIcon(SymbolRegular.Checkmark24), TimeSpan.FromSeconds(3));
                Log.WriteLog(["Processes", sfd.FileName], LogType.ExportCSV);
            }
        }

        /// <summary>
        /// Navigate to the new instance page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemNew_Click(object sender, RoutedEventArgs e)
        {
            _window.RootNavigation.NavigateWithHierarchy(typeof(NewInstancePage));
        }

        /// <summary>
        /// Load the data when a process is changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProcessCard_OnProcessChange(object sender, EventArgs e)
        {
            LoadData(true);
        }

        /// <summary>
        /// Filter the processes based on the filter text
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterProcesses();
        }

        /// <summary>
        /// Sort the processes based on the selected menu item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemSort_Click(object sender, RoutedEventArgs e)
        {
            foreach (object item in MenuItemSort.Items)
            {
                if (item is MenuItem otherItem && otherItem.Tag != null)
                {
                    otherItem.FontWeight = FontWeights.Normal;
                }
            }

            MenuItem menuItem = (MenuItem)sender;
            menuItem.FontWeight = FontWeights.Bold;

            sort = (ProcessesPageSort)Enum.Parse(typeof(ProcessesPageSort), (string)menuItem.Tag);
            FilterProcesses();
        }

        /// <summary>
        /// Sort the processes based on the selected menu item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemSortOrder_Click(object sender, RoutedEventArgs e)
        {
            FilterProcesses();
        }

        /// <summary>
        /// Filter the processes based on the selected status
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemStatus_Click(object sender, RoutedEventArgs e)
        {
            foreach (object item in MenuItemStatus.Items)
            {
                if (item is MenuItem otherItem && otherItem.Tag != null)
                {
                    otherItem.FontWeight = FontWeights.Normal;
                }
            }

            MenuItem menuItem = (MenuItem)sender;
            menuItem.FontWeight = FontWeights.Bold;

            statusFilter = (ProcessesPageStatus)Enum.Parse(typeof(ProcessesPageStatus), (string)menuItem.Tag);
            FilterProcesses();
        }
    }
}
