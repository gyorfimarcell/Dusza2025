using Cluster.ChartModels;
using LiveChartsCore;
using LiveChartsCore.Kernel;
using LiveChartsCore.SkiaSharpView.Painting;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Wpf.Ui.Controls;
using static Cluster.ComputersPage;
using Button = Wpf.Ui.Controls.Button;
using MenuItem = Wpf.Ui.Controls.MenuItem;
using MessageBox = Wpf.Ui.Controls.MessageBox;
using MessageBoxResult = Wpf.Ui.Controls.MessageBoxResult;
using TextBlock = Wpf.Ui.Controls.TextBlock;

namespace Cluster
{
    public partial class ProgramsPage : CustomPage
    {
        List<ProgramType> Programs;
        ProgramsPageSort sort = ProgramsPageSort.Name;

        public ProgramsPage()
        {
            InitializeComponent();
            LoadData();

            _window.PropertyChanged += _window_PropertyChanged;
        }

        private void _window_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MainWindow.DarkMode))
            {
                barRequested.CoreChart.Update(new ChartUpdateParams { IsAutomaticUpdate = false, Throttling = false });
                barRequested.LegendTextPaint = (SolidColorPaint?)LiveCharts.DefaultSettings.LegendTextPaint;
            }
        }

        public void LoadData()
        {
            Programs = ProgramType.ReadClusterFile(MainWindow.ClusterPath);
            UpdateFiltering();
            UpdateCharts();
        }

        internal enum ProgramsPageSort
        {
            Name,
            Active,
            ProcessorUsage,
            MemoryUsage
        }

        private void UpdateFiltering()
        {
            IEnumerable<ProgramType> filtered = [..Programs];

            filtered = filtered.Where(x => tbFilter.Text == "" || x.ProgramName.Contains(tbFilter.Text, StringComparison.InvariantCultureIgnoreCase));

            filtered = sort switch
            {
                ProgramsPageSort.Name => filtered.OrderBy(x => x.ProgramName),
                ProgramsPageSort.Active => filtered.OrderBy(x => x.ActivePrograms),
                ProgramsPageSort.ProcessorUsage => filtered.OrderBy(x => x.CpuMilliCore),         
                ProgramsPageSort.MemoryUsage => filtered.OrderBy(x => x.Memory),
                _ => throw new NotImplementedException(),
            };

            if (MenuItemSortOrder.IsChecked) filtered = filtered.Reverse();

            icPrograms.ItemsSource = filtered;
        }

        private void UpdateCharts() {
            ProgramsPageCharts data = new(Programs);

            barRequested.Series = data.RequestedSeries;
            barRequested.XAxes = data.RequestedXAxis;
            barRequested.YAxes = data.RequestedYAxis;

            chartRow.Height = Programs.Count == 0 ? new GridLength(0) : new GridLength(150);
        }

        private void CardAction_Click(object sender, RoutedEventArgs e)
        {
            CardAction card = (CardAction)sender;
            ProgramType program = (ProgramType)card.DataContext;
            _window.RootNavigation.Navigate(typeof(ProcessesPage), program.ProgramName);
        }

        private void MenuItemNew_Click(object sender, RoutedEventArgs e)
        {
            _window.RootNavigation.NavigateWithHierarchy(typeof(AddNewProgramPage));
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;

            Button card = (Button)sender;
            ProgramType program = (ProgramType)card.DataContext;
            _window.RootNavigation.NavigateWithHierarchy(typeof(ModifyProgramPage), program);
        }

        private async void btnShutdown_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;

            Button card = (Button)sender;
            ProgramType program = (ProgramType)card.DataContext;
            int processCount = Computer.GetComputers(MainWindow.ClusterPath)
                .Sum(x => x.processes.Count(y => y.ProgramName == program.ProgramName));

            MessageBox messageBox = new MessageBox()
            {
                Title = $"Shut down {program.ProgramName}?",
                Content = $"This will shut down all {processCount} processes.",
                PrimaryButtonText = "Shutdown",
                PrimaryButtonAppearance = ControlAppearance.Danger,
            };


            MessageBoxResult result = await messageBox.ShowDialogAsync();
            if (result == MessageBoxResult.Primary)
            {
                string instancesCount = program.ActivePrograms.ToString();
                if (ProgramType.ShutdownProgram(program))
                { 
                    Log.WriteLog([program.ProgramName, $"{program.CpuMilliCore}", $"{program.Memory}", instancesCount], LogType.ShutdownProgram);
                    _window.RootSnackbarService.Show(
                        "Success",
                        $"Program '{program.ProgramName}' successfully shut down!",
                        ControlAppearance.Success,
                        new SymbolIcon { Symbol = SymbolRegular.Checkmark24 },
                        TimeSpan.FromSeconds(3)
                    );
                    LoadData();
                }
            }
        }

        private void tbFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateFiltering();
        }

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
            
            sort = (ProgramsPageSort)Enum.Parse(typeof(ProgramsPageSort), (string)menuItem.Tag);
            UpdateFiltering();
        }

        private void MenuItemSortOrder_Click(object sender, RoutedEventArgs e)
        {
            UpdateFiltering();
        }
    }
}
