using Cluster.ChartModels;
using LiveChartsCore.Kernel;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using Wpf.Ui.Controls;

namespace Cluster;

public partial class ComputerDetailsPage : CustomPage, INotifyPropertyChanged
{
    public Computer PageComputer { get; set; }

    public string ProcessesText =>
        PageComputer == null ? "" : $"{PageComputer.processes.Count} processes ({PageComputer.processes.Count(x => x.Active)} active)";

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

    private void _window_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
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

    private void UpdateCharts()
    {
        ComputerDetailsPageCharts data = new(PageComputer);
        piePrograms.Series = data.ProgramsSeries;
        pieProcessor.Series = data.ProcessorSeries;
        pieProcessor.MaxValue = PageComputer.ProcessorCore;
        pieMemory.Series = data.MemorySeries;
        pieMemory.MaxValue = PageComputer.RamCapacity;
    }

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
            string? res = PageComputer.OutSourcePrograms();
            if (res != null)
            {
                if (res.Length == 0) return;
                _window.RootSnackbarService.Show("Error", res, ControlAppearance.Danger,
                        new SymbolIcon(SymbolRegular.Warning24), TimeSpan.FromSeconds(3));
                return;
            }
            _window.RootSnackbarService.Show("Success", @$"Outsourcing succeed! You can delete now the '{PageComputer.Name}' safely.", ControlAppearance.Success,
                        new SymbolIcon(SymbolRegular.Check24), TimeSpan.FromSeconds(3));

            SetData(Computer.GetComputers(MainWindow.ClusterPath).Find(x => x.Name == PageComputer.Name));
        }
        else
        {
            string? error = PageComputer.Delete();
            if (error != null)
            {
                _window.RootSnackbarService.Show("Error", error, ControlAppearance.Danger,
                    new SymbolIcon(SymbolRegular.Warning24), TimeSpan.FromSeconds(3));
                return;
            }
            _window.RootSnackbarService.Show("Computer deleted", $"Computer '{PageComputer.Name}' successfully deleted.",
                ControlAppearance.Success, new SymbolIcon(SymbolRegular.Check24), TimeSpan.FromSeconds(3));
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
        SetData(Computer.GetComputers(MainWindow.ClusterPath).Find(x => x.Name == PageComputer.Name));
    }
}