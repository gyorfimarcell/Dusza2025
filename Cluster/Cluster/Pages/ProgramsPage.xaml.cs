using System.Windows;
using System.Windows.Controls;
using Cluster.ChartModels;
using LiveChartsCore;
using LiveChartsCore.Kernel;
using LiveChartsCore.SkiaSharpView.Painting;
using Wpf.Ui.Controls;
using Button = Wpf.Ui.Controls.Button;
using MenuItem = Wpf.Ui.Controls.MenuItem;
using MessageBox = Wpf.Ui.Controls.MessageBox;
using MessageBoxResult = Wpf.Ui.Controls.MessageBoxResult;

namespace Cluster
{
    public partial class ProgramsPage
    {
        private List<ProgramType> Programs = null!;
        private ProgramsPageSort sort = ProgramsPageSort.Name;

        public ProgramsPage()
        {
            InitializeComponent();
            LoadData();

            _window.PropertyChanged += _window_PropertyChanged;
        }

        /// <summary>
        /// Updates charts if window loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _window_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MainWindow.DarkMode))
            {
                barRequested.CoreChart.Update(new ChartUpdateParams { IsAutomaticUpdate = false, Throttling = false });
                barRequested.LegendTextPaint = (SolidColorPaint?)LiveCharts.DefaultSettings.LegendTextPaint;
            }
        }

        /// <summary>
        /// Loading the programs of the cluster
        /// </summary>
        private void LoadData()
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

        /// <summary>
        /// Updates the filtering of the programs
        /// </summary>
        /// <exception cref="NotImplementedException">If filtering is not implemented</exception>
        private void UpdateFiltering()
        {
            IEnumerable<ProgramType> filtered = [..Programs];

            filtered = filtered.Where(x =>
                tbFilter.Text == "" ||
                x.ProgramName.Contains(tbFilter.Text, StringComparison.InvariantCultureIgnoreCase));

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

        /// <summary>
        /// Updates the charts of the programs
        /// </summary>
        private void UpdateCharts()
        {
            ProgramsPageCharts data = new(Programs);

            barRequested.Series = data.RequestedSeries;
            barRequested.XAxes = data.RequestedXAxis;
            barRequested.YAxes = data.RequestedYAxis;

            chartRow.Height = Programs.Count == 0 ? new GridLength(0) : new GridLength(150);
        }

        /// <summary>
        /// Opens the processes page for the program
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CardAction_Click(object sender, RoutedEventArgs e)
        {
            var card = (CardAction)sender;
            var program = (ProgramType)card.DataContext;
            _window.RootNavigation.Navigate(typeof(ProcessesPage), program.ProgramName);
        }

        /// <summary>
        /// Opens the AddNewProgramPage
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemNew_Click(object sender, RoutedEventArgs e)
        {
            _window.RootNavigation.NavigateWithHierarchy(typeof(AddNewProgramPage));
        }

        /// <summary>
        /// Opens the ModifyProgramPage based on the program
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;

            var card = (Button)sender;
            var program = (ProgramType)card.DataContext;
            _window.RootNavigation.NavigateWithHierarchy(typeof(ModifyProgramPage), program);
        }

        /// <summary>
        /// Shuts down the program
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnShutdown_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;

            var card = (Button)sender;
            var program = (ProgramType)card.DataContext;
            int processCount = Computer.GetComputers(MainWindow.ClusterPath)
                .Sum(x => x.processes.Count(y => y.ProgramName == program.ProgramName));

            var messageBox = new MessageBox()
            {
                Title = TranslationSource.Instance.WithParam("ProgramsPage.Shutdown.Title", program.ProgramName),
                Content = TranslationSource.Instance.WithParam("ProgramsPage.Shutdown.Text", processCount.ToString()),
                PrimaryButtonText = TranslationSource.T("ProgramsPage.Shutdown"),
                CloseButtonText = TranslationSource.T("Cancel"),
                PrimaryButtonAppearance = ControlAppearance.Danger,
            };


            MessageBoxResult result = await messageBox.ShowDialogAsync();
            if (result == MessageBoxResult.Primary)
            {
                var instancesCount = program.ActivePrograms.ToString();
                if (ProgramType.ShutdownProgram(program))
                {
                    Log.WriteLog([program.ProgramName, $"{program.CpuMilliCore}", $"{program.Memory}", instancesCount],
                        LogType.ShutdownProgram);
                    _window.RootSnackbarService.Show(
                        TranslationSource.T("Success"),
                        $"'{program.ProgramName}' {TranslationSource.T("ProgramsPage.Shutdown.Success")}",
                        ControlAppearance.Success,
                        new SymbolIcon { Symbol = SymbolRegular.Checkmark24 },
                        TimeSpan.FromSeconds(10)
                    );
                    LoadData();
                }
            }
        }

        /// <summary>
        /// Filter updating
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateFiltering();
        }

        /// <summary>
        /// Sorting the menu items by program name
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemSort_Click(object sender, RoutedEventArgs e)
        {
            foreach (object item in MenuItemSort.Items)
            {
                if (item is MenuItem { Tag: not null } otherItem)
                {
                    otherItem.FontWeight = FontWeights.Normal;
                }
            }

            MenuItem menuItem = (MenuItem)sender;
            menuItem.FontWeight = FontWeights.Bold;

            sort = (ProgramsPageSort)Enum.Parse(typeof(ProgramsPageSort), (string)menuItem.Tag);
            UpdateFiltering();
        }

        /// <summary>
        /// Sorting the menu items by program name
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemSortOrder_Click(object sender, RoutedEventArgs e)
        {
            UpdateFiltering();
        }
    }
}