using Cluster.Controls;
using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Wpf.Ui.Controls;
using Button = Wpf.Ui.Controls.Button;
using MenuItem = Wpf.Ui.Controls.MenuItem;
using MessageBox = Wpf.Ui.Controls.MessageBox;
using MessageBoxResult = Wpf.Ui.Controls.MessageBoxResult;

namespace Cluster;

public partial class ComputersPage : CustomPage
{
    public List<Computer> Computers;
    public ComputersPage()
    {
        InitializeComponent();

        Computers = [];
        LoadData();
    }
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

    private void UpdateFiltering()
    {
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
        CardAction cardControl = (CardAction)sender;
        Computer computer = (Computer)cardControl.DataContext;

        _window.RootNavigation.NavigateWithHierarchy(typeof(ComputerDetailsPage), computer);
    }

    private void Edit_OnClick(object sender, RoutedEventArgs e)
    {
        e.Handled = true;

        Button button = (Button)sender;
        Computer computer = (Computer)button.DataContext;

        _window.RootNavigation.NavigateWithHierarchy(typeof(ModifyComputerPage), computer);
    }

    private void Delete_OnClick(object sender, RoutedEventArgs e)
    {
        e.Handled = true;

        Button button = (Button)sender;
        Computer computer = (Computer)button.DataContext;

        if (computer.processes.Count > 0)
        {
            List<string>? res = computer.OutSourcePrograms();
            if (res == null)
                return;

            ControlAppearance controlAppearance = ControlAppearance.Success;

            if (Enum.TryParse(res[1], out ControlAppearance parsedAppearance))
            {
                controlAppearance = parsedAppearance;
            }

            _window.RootSnackbarService.Show(
                res[1],
                res[0],
                controlAppearance,
                new SymbolIcon(controlAppearance == ControlAppearance.Danger ? SymbolRegular.Warning24 : SymbolRegular.Checkmark24),
                TimeSpan.FromSeconds(10));
        }
        else
        {
            string? error = computer.Delete();
            if (error != null)
            {
                _window.RootSnackbarService.Show(TranslationSource.T("Errors.Error"), error, ControlAppearance.Danger,
                    new SymbolIcon(SymbolRegular.Warning24), TimeSpan.FromSeconds(10));
                return;
            }
            _window.RootSnackbarService.Show(TranslationSource.T("ComputerDetailsPage.DeleteSuccess.Title"), $"'{computer.Name}' {TranslationSource.T("ComputerDetailsPage.DeleteSuccess.Text")}",
                ControlAppearance.Success, new SymbolIcon(SymbolRegular.Checkmark24), TimeSpan.FromSeconds(10));
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
            string[] lines = ["Name;ProcessorCapacity;ProcessorUsage;MemoryCapacity;MemoryUsage", .. Computers.Select(x => x.CsvRow)];
            File.WriteAllLines(sfd.FileName, lines);
            _window.RootSnackbarService.Show(TranslationSource.T("Export.Success.Title"),
                TranslationSource.Instance.WithParam("Export.Success.Text", sfd.FileName),
                ControlAppearance.Success, new SymbolIcon(SymbolRegular.Checkmark24), TimeSpan.FromSeconds(10));
            Log.WriteLog(["Computers", sfd.FileName], LogType.ExportCSV);
        }
    }

    private void MenuItemSort_Click(object sender, RoutedEventArgs e)
    {
        foreach (object item in MenuItemSort.Items)
        {
            if (item is MenuItem otherItem && otherItem.Tag != null)
            {
                otherItem.FontWeight = FontWeights.Normal;
            }
        }

        MenuItem menuItem = (MenuItem)sender;
        menuItem.FontWeight = FontWeights.Bold;

        sort = (ComputersPageSort)Enum.Parse(typeof(ComputersPageSort), (string)menuItem.Tag);
        UpdateFiltering();
    }
    private void MenuItemOptimize_Click(object sender, RoutedEventArgs e)
    {
        MainWindow window = (MainWindow)Application.Current.MainWindow!;
        OptimizeDialog optimizeDialog = new();
        MessageBox mgbox = new()
        {
            Title = TranslationSource.T("Optimize.Title"),
            Content = optimizeDialog,
            IsPrimaryButtonEnabled = true,
            IsSecondaryButtonEnabled = false,
            PrimaryButtonText = TranslationSource.T("Optimize.Button"),
            CloseButtonText = TranslationSource.T("Cancel"),
            Width = 500,
            MaxWidth = 500,
            MaxHeight = 1000
        };
        MessageBoxResult result = mgbox.ShowDialogAsync().GetAwaiter().GetResult();

        // Hit cancel
        if (result != MessageBoxResult.Primary)
            return;

        if (!Computer.CanOptimizeComputers(optimizeDialog.Minimum, optimizeDialog.Maximum))
        {
            mgbox = new()
            {
                Title = TranslationSource.T("Errors.Error"),
                Content = TranslationSource.T("Optimize.SpreadQuestion"),
                IsPrimaryButtonEnabled = true,
                IsSecondaryButtonEnabled = false,
                PrimaryButtonText = TranslationSource.T("Optimize.Spread"),
                CloseButtonText = TranslationSource.T("Cancel"),
            };
            result = mgbox.ShowDialogAsync().GetAwaiter().GetResult();

            // Hit cancel
            if (result != MessageBoxResult.Primary)
                return;

            string? spreadRes = Computer.SpreadProcesses(1);
            if (spreadRes != null)
            {
                window.RootSnackbarService.Show(TranslationSource.T("Errors.Error"), spreadRes, ControlAppearance.Danger,
                    new SymbolIcon(SymbolRegular.Warning24), TimeSpan.FromSeconds(10));
            }
            else
            {
                LoadData();
                window.RootSnackbarService.Show(TranslationSource.T("Success"), TranslationSource.T("Optimize.Spread.Success"), ControlAppearance.Success, new SymbolIcon(SymbolRegular.Checkmark24), TimeSpan.FromSeconds(10));
            }
            return;
        }
        string? optimizeRes = Computer.OptimizeComputers(optimizeDialog.Minimum, optimizeDialog.Maximum);

        if (optimizeRes != null)
        {
            window.RootSnackbarService.Show(TranslationSource.T("Errors.Error"), optimizeRes, ControlAppearance.Danger,
                new SymbolIcon(SymbolRegular.Warning24), TimeSpan.FromSeconds(10));
            return;
        }
        Log.WriteLog([$"{optimizeDialog.Minimum}", $"{optimizeDialog.Maximum}", Computers.Count.ToString()], LogType.OptimizeProgramInstances);
        LoadData();
        window.RootSnackbarService.Show(TranslationSource.T("Success"), TranslationSource.T("Optimize.Success"), ControlAppearance.Success, new SymbolIcon(SymbolRegular.Checkmark24), TimeSpan.FromSeconds(10));
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