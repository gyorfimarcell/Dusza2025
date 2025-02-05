using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Wpf.Ui.Controls;
using Button = Wpf.Ui.Controls.Button;
using MenuItem = Wpf.Ui.Controls.MenuItem;
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

    public List<Computer> Computers;
    ComputersPageSort sort = ComputersPageSort.Name;

    private void LoadData()
    {
        Computers = Computer.GetComputers(MainWindow.ClusterPath);
        UpdateFiltering();
    }

    internal enum ComputersPageSort
    {
        Name,
        ProcessorUsage,
        ProcessorUsagePercent,
        ProcessorCapacity,
        MemoryUsage,
        MemoryUsagePercent,
        MemoryCapacity
    }

    private void UpdateFiltering() {
        IEnumerable<Computer> filtered = [.. Computers];

        filtered = filtered.Where(x => tbFilter.Text == "" || x.Name.Contains(tbFilter.Text, StringComparison.InvariantCultureIgnoreCase));

        filtered = sort switch
        {
            ComputersPageSort.Name => filtered.OrderBy(x => x.Name),
            ComputersPageSort.ProcessorUsage => filtered.OrderBy(x => x.ProcessorUsage),
            ComputersPageSort.ProcessorUsagePercent => filtered.OrderBy(x => x.ProcessorUsage / (double)x.ProcessorCore),
            ComputersPageSort.ProcessorCapacity => filtered.OrderBy(x => x.ProcessorCore),
            ComputersPageSort.MemoryUsage => filtered.OrderBy(x => x.MemoryUsage),
            ComputersPageSort.MemoryUsagePercent => filtered.OrderBy(x => x.MemoryUsage / (double)x.RamCapacity),
            ComputersPageSort.MemoryCapacity => filtered.OrderBy(x => x.RamCapacity),
            _ => throw new NotImplementedException(),
        };

        if (MenuItemSortOrder.IsChecked) filtered = filtered.Reverse();

        icComputers.ItemsSource = filtered;
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

        if (computer.processes.Count > 0)
        {
            string? res = computer.OutSourcePrograms();
            if (res != null)
            {
                if (res.Length == 0) return;
                _window.RootSnackbarService.Show("Error", res, ControlAppearance.Danger,
                        new SymbolIcon(SymbolRegular.Warning24), TimeSpan.FromSeconds(3));
                return;
            }
            _window.RootSnackbarService.Show("Success", @$"Outsourcing succeed! You can delete now the '{computer.Name}' safely.", ControlAppearance.Success,
                        new SymbolIcon(SymbolRegular.Check24), TimeSpan.FromSeconds(3));
        }
        else
        {
            string? error = computer.Delete();
            if (error != null)
            {
                _window.RootSnackbarService.Show("Error", error, ControlAppearance.Danger,
                    new SymbolIcon(SymbolRegular.Warning24), TimeSpan.FromSeconds(3));
                return;
            }
            _window.RootSnackbarService.Show("Computer deleted", $"Computer '{computer.Name}' successfully deleted.",
                ControlAppearance.Success, new SymbolIcon(SymbolRegular.Check24), TimeSpan.FromSeconds(3));
        }
        LoadData();
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

    private void MenuItemSort_Click(object sender, RoutedEventArgs e)
    {
        MenuItem menuItem = (MenuItem)sender;
        sort = (ComputersPageSort)Enum.Parse(typeof(ComputersPageSort), (string)menuItem.Tag);
        UpdateFiltering();
    }

    

    private void tbFilter_TextChanged(object sender, TextChangedEventArgs e)
    {
        UpdateFiltering();
    }

    private void MenuItemSortOrder_Click(object sender, RoutedEventArgs e)
    {
        UpdateFiltering();
    }
}