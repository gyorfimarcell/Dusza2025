using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Wpf.Ui.Controls;
using Button = Wpf.Ui.Controls.Button;
using MessageBox = Wpf.Ui.Controls.MessageBox;
using MessageBoxResult = Wpf.Ui.Controls.MessageBoxResult;

namespace Cluster;

public partial class ComputersPage : CustomPage
{
    MainWindow _window;

    public ComputersPage()
    {
        InitializeComponent();

        _window = (MainWindow)Application.Current.MainWindow!;

        LoadData();
    }

    private void LoadData()
    {
        List<Computer> computers = Computer.GetComputers(MainWindow.ClusterPath);
        icComputers.ItemsSource = computers.OrderBy(x => x.Name);
    }

    private void MenuItemNew_OnClick(object sender, RoutedEventArgs e)
    {
        _window.RootNavigation.NavigateWithHierarchy(typeof(AddComputerPage));
    }

    private void ComputerCard_OnClick(object sender, RoutedEventArgs e)
    {
        CardControl cardControl = (CardControl)sender;
        Computer computer = (Computer)cardControl.DataContext;

        _window.RootNavigation.NavigateWithHierarchy(typeof(ComputerDetailsPage), computer);
    }

    private void Delete_OnClick(object sender, RoutedEventArgs e)
    {
        e.Handled = true;

        Button button = (Button)sender;
        Computer computer = (Computer)button.DataContext;

        string? error = computer.Delete();
        if (error == null)
        {
            _window.RootSnackbarService.Show("Computer deleted", $"Computer '{computer.Name}' successfully deleted.",
                ControlAppearance.Success, new SymbolIcon(SymbolRegular.Check24), TimeSpan.FromSeconds(3));

            LoadData();
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

            }
            return;
        }
        _window.RootSnackbarService.Show("Error", error, ControlAppearance.Danger,
            new SymbolIcon(SymbolRegular.Warning24), TimeSpan.FromSeconds(3));
    }
}