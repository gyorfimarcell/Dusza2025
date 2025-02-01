using System.Windows.Controls;

namespace Cluster;

public partial class ComputersPage : Page
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
}

public class ComputerRow(Computer computer)
{
    public string Name => computer.Name;

    public int ProcessorCapacity => computer.ProcessorCore;
    public int ProcessorUsage => computer.processes.Where(x => x.Active).Sum(x => x.ProcessorUsage);

    public int MemoryCapacity => computer.RamCapacity;
    public int MemoryUsage => computer.processes.Where(x => x.Active).Sum(x => x.MemoryUsage);
}