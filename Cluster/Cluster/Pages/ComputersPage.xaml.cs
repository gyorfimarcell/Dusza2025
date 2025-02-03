using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Wpf.Ui.Controls;
using Button = Wpf.Ui.Controls.Button;
using MessageBox = System.Windows.MessageBox;

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

    public List<Computer> Computers;

    private void LoadData()
    {
        Computers = Computer.GetComputers(MainWindow.ClusterPath).OrderBy(x => x.Name).ToList();
        icComputers.ItemsSource = Computers;
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
        if (error != null)
        {
            _window.RootSnackbarService.Show("Error", error, ControlAppearance.Danger,
                new SymbolIcon(SymbolRegular.Warning24), TimeSpan.FromSeconds(3));
        }
        else
        {
            _window.RootSnackbarService.Show("Computer deleted", $"Computer '{computer.Name}' successfully deleted.",
                ControlAppearance.Success, new SymbolIcon(SymbolRegular.Checkmark24), TimeSpan.FromSeconds(3));

            LoadData();
        }
    }

    private void MenuItemExport_Click(object sender, RoutedEventArgs e)
    {
        SaveFileDialog sfd = new SaveFileDialog();
        sfd.Filter = "CSV Files | *.csv";
        sfd.DefaultExt = "csv";
        if (sfd.ShowDialog() == true)
        {
            string[] lines = ["Name;ProcessorCapacity;ProcessorUsage;MemoryCapacity;MemoryUsage", ..Computers.Select(x => x.CsvRow)];
            File.WriteAllLines(sfd.FileName, lines);
            _window.RootSnackbarService.Show("Export complete", $"File saved to '{sfd.FileName}'",
                ControlAppearance.Success, new SymbolIcon(SymbolRegular.Checkmark24), TimeSpan.FromSeconds(3));
        }
    }
}