using System.Windows;
using System.Windows.Controls;
using Wpf.Ui.Controls;
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
}