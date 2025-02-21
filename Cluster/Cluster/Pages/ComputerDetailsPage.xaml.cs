using System.ComponentModel;
using System.Windows;
using Cluster.ChartModels;
using LiveChartsCore;
using LiveChartsCore.Kernel;
using LiveChartsCore.SkiaSharpView.Painting;
using Wpf.Ui.Controls;

namespace Cluster;

public partial class ComputerDetailsPage : INotifyPropertyChanged
{
    public Computer PageComputer { get; set; } = null!;

    public string ProcessesText =>
        PageComputer == null
            ? ""
            : $"{PageComputer.processes.Count} {TranslationSource.T("ComputerDetailsPage.Processes")} ({PageComputer.processes.Count(x => x.Active)} {TranslationSource.T("Active")})";

    public event PropertyChangedEventHandler? PropertyChanged;

    public ComputerDetailsPage()
    {
        InitializeComponent();
        Loaded += OnLoaded;

        _window.PropertyChanged += _window_PropertyChanged;
    }

    /// <summary>
    /// Sets the data of the page if the DataContext is a Computer
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is not Computer computer)
        {
            _window.RootNavigation.GoBack();
            return;
        }

        SetData(computer);
    }

    /// <summary>
    /// Changing values of the charts
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void _window_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(MainWindow.DarkMode))
        {
            piePrograms.CoreChart.Update(new ChartUpdateParams { IsAutomaticUpdate = false, Throttling = false });
            pieProcessor.CoreChart.Update(new ChartUpdateParams { IsAutomaticUpdate = false, Throttling = false });
            pieMemory.CoreChart.Update(new ChartUpdateParams { IsAutomaticUpdate = false, Throttling = false });

            pieProcessor.Series.Last().DataLabelsPaint = (SolidColorPaint?)LiveCharts.DefaultSettings.LegendTextPaint;
            pieMemory.Series.Last().DataLabelsPaint = (SolidColorPaint?)LiveCharts.DefaultSettings.LegendTextPaint;
        }
    }

    /// <summary>
    /// Sets the data of the page
    /// </summary>
    /// <param name="computer">Instance of a computer</param>
    private void SetData(Computer computer)
    {
        PageComputer = computer;

        ChangeTitle(PageComputer.Name);

        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PageComputer)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ProcessesText)));

        UpdateCharts();
        chartsRow.Height = PageComputer.processes.Count != 0 ? new GridLength(175) : new GridLength(0);
    }

    /// <summary>
    /// Updating the charts
    /// </summary>
    private void UpdateCharts()
    {
        ComputerDetailsPageCharts data = new(PageComputer);
        piePrograms.Series = data.ProgramsSeries;
        pieProcessor.Series = data.ProcessorSeries;
        pieProcessor.MaxValue = PageComputer.ProcessorCore;
        pieMemory.Series = data.MemorySeries;
        pieMemory.MaxValue = PageComputer.RamCapacity;
    }

    /// <summary>
    /// Editing computer click event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Edit_OnClick(object sender, RoutedEventArgs e)
    {
        _window.RootNavigation.GoBack();
        _window.RootNavigation.NavigateWithHierarchy(typeof(ModifyComputerPage), PageComputer);
    }

    /// <summary>
    /// Deleting computer or outsourcing programs
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Delete_OnClick(object sender, RoutedEventArgs e)
    {
        e.Handled = true;

        if (PageComputer.processes.Count > 0)
        {
            List<string>? res = PageComputer.OutSourcePrograms();
            if (res == null)
                return;

            var controlAppearance = ControlAppearance.Success;

            if (Enum.TryParse(res[1], out ControlAppearance parsedAppearance))
            {
                controlAppearance = parsedAppearance;
            }

            _window.RootSnackbarService.Show(
                res[1],
                res[0],
                controlAppearance,
                new SymbolIcon(controlAppearance == ControlAppearance.Danger
                    ? SymbolRegular.Warning24
                    : SymbolRegular.Checkmark24),
                TimeSpan.FromSeconds(10));

            if (res[0].Contains(TranslationSource.T("Outsourcing.DeleteSuccess")))
            {
                _window.RootNavigation.GoBack();
                return;
            }

            SetData(Computer.GetComputers(MainWindow.ClusterPath).Find(x => x.Name == PageComputer.Name)!);
        }
        else
        {
            string? error = PageComputer.Delete();
            if (error != null)
            {
                _window.RootSnackbarService.Show(TranslationSource.T("Errors.Error"), error, ControlAppearance.Danger,
                    new SymbolIcon(SymbolRegular.Warning24), TimeSpan.FromSeconds(10));
                return;
            }

            _window.RootSnackbarService.Show(TranslationSource.T("ComputerDetailsPage.DeleteSuccess.Title"),
                $"'{PageComputer.Name}' {TranslationSource.T("ComputerDetailsPage.DeleteSuccess.Text")}",
                ControlAppearance.Success, new SymbolIcon(SymbolRegular.Checkmark24), TimeSpan.FromSeconds(10));
            _window.RootNavigation.Navigate(typeof(ComputersPage));
        }
    }

    /// <summary>
    /// Set data with the right computers
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ProcessCard_OnProcessChange(object sender, EventArgs e)
    {
        SetData(Computer.GetComputers(MainWindow.ClusterPath).Find(x => x.Name == PageComputer.Name)!);
    }
}