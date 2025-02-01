using System.Windows;
using System.Windows.Controls;

namespace Cluster;

public partial class ComputersPage : CustomPage
{
    public ComputersPage()
    {
        InitializeComponent();
        LoadData();
    }

    private void LoadData()
    {
        List<Computer> computers = Computer.GetComputers(MainWindow.ClusterPath);
        icComputers.ItemsSource = computers.OrderBy(x => x.Name).Select(x => new ComputerRow(x));
    }

    private void MenuItemNew_OnClick(object sender, RoutedEventArgs e)
    {
        MainWindow window = (MainWindow)Application.Current.MainWindow!;
        window.RootNavigation.NavigateWithHierarchy(typeof(AddComputerPage));
    }
}

public class ComputerRow(Computer computer)
{
    public string Name => computer.Name;

    public int ProcessorCapacity => computer.ProcessorCore;
    public int ProcessorUsage => computer.processes.Where(x => x.Active).Sum(x => x.ProcessorUsage);

    public int MemoryCapacity => computer.RamCapacity;
    public int MemoryUsage => computer.processes.Where(x => x.Active).Sum(x => x.MemoryUsage);
}