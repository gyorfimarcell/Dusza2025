using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
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
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is not Computer computer)
        {
            throw new ArgumentException("A computer must be passed as this page's DataContext!");
        }

        SetData(computer);
    }

    private void SetData(Computer computer)
    {
        PageComputer = computer;

        ChangeTitle(PageComputer.Name);

        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PageComputer)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ProcessesText)));
    }

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

    private void ProcessCard_OnProcessChange(object sender, EventArgs e)
    {
        SetData(Computer.GetComputers(MainWindow.ClusterPath).Find(x => x.Name == PageComputer.Name));
    }
}