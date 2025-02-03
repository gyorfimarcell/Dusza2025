using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Wpf.Ui.Controls;
using Button = Wpf.Ui.Controls.Button;
using MessageBox = Wpf.Ui.Controls.MessageBox;
using MessageBoxResult = Wpf.Ui.Controls.MessageBoxResult;

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

        PageComputer = computer;

        ChangeTitle(PageComputer.Name);

        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PageComputer)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ProcessesText)));
    }

    private void Delete_OnClick(object sender, RoutedEventArgs e)
    {
        e.Handled = true;

        Button button = (Button)sender;
        Computer computer = (Computer)button.DataContext;

        string? error = computer.Delete();
        if (error == null)
        {
            _window.RootSnackbarService.Show("Computer deleted", $"Computer '{PageComputer.Name}' successfully deleted.",
                ControlAppearance.Success, new SymbolIcon(SymbolRegular.Checkmark24), TimeSpan.FromSeconds(3));

            _window.RootNavigation.Navigate(typeof(ComputersPage));
            return;
        }

        if (computer.CanOutSourcePrograms())
        {
            MessageBox mgbox = new()
            {
                Title = "Error",
                Content = "Deletion failed as this computer is running programs, but they can be outsourced to other machines. Would you like to proceed?",
                IsPrimaryButtonEnabled = true,
                IsSecondaryButtonEnabled = false,
                //Background = new SolidColorBrush(Color.FromRgb(244, 66, 54)),
                PrimaryButtonText = "Yes",
                CloseButtonText = "Cancel"

            };
            MessageBoxResult result = mgbox.ShowDialogAsync().GetAwaiter().GetResult();
            if (result == MessageBoxResult.Primary)
            {
                //TODO: Implement outsource programs
                bool isSuccess = computer.OutSourcePrograms();
                if (!isSuccess)
                {
                    _window.RootSnackbarService.Show("Error", "Outsourcing failed! Please try again later.", ControlAppearance.Danger,
                        new SymbolIcon(SymbolRegular.Warning24), TimeSpan.FromSeconds(3));
                    return;
                }
                _window.RootSnackbarService.Show("Success", @$"Outsourcing succeed! You can delete now the '{computer.Name}' safely.", ControlAppearance.Danger,
                        new SymbolIcon(SymbolRegular.Warning24), TimeSpan.FromSeconds(3));
            }
            return;
        }
        _window.RootSnackbarService.Show("Error", error, ControlAppearance.Danger,
            new SymbolIcon(SymbolRegular.Warning24), TimeSpan.FromSeconds(3));
    }
}