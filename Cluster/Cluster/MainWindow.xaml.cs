using System.Collections;
using Cluster.Pages;
using Microsoft.Win32;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Wpf.Ui;
using Wpf.Ui.Controls;
using MessageBox = Wpf.Ui.Controls.MessageBox;

namespace Cluster
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public static string ClusterPath { get; private set; } = "";
        
        public SnackbarService RootSnackbarService { get; private set; }
        
        private IEnumerable originalBreadcrumbs;

        public MainWindow()
        {
            InitializeComponent();
            
            RootSnackbarService = new SnackbarService();
            RootSnackbarService.SetSnackbarPresenter(RootSnackbarPresenter);
            
            Loaded += MainWindow_Loaded;
            Log.WriteLog([], LogType.OpenProgram);
        }

        private void LoadLastOpenedCluster()
        {
            string directoryPath = Log.GetLogDirectoryPath();

            if (Directory.Exists(directoryPath))
            {
                string[] files = Directory.GetFiles(directoryPath, "*.log");

                if (files != null)
                {
                    var latestFile = files.Select(file => new { FilePath = file, Date = DateTime.TryParseExact(Path.GetFileNameWithoutExtension(file), "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out DateTime date) ? date : (DateTime?)null}).Where(f => f.Date.HasValue).OrderByDescending(f => f.Date).FirstOrDefault();

                    string[] clusterLines = File.ReadAllLines(latestFile.FilePath).Where(x => x.StartsWith("LoadCluster")).ToArray();

                    if (clusterLines.Length == 0) 
                    {
                        return;
                    }

                    ClusterPath = clusterLines.Last().Split(" - ").Last();

                    if (ClusterPath != null)
                    {
                        lblPath.Content = $"Cluster: {Path.GetFileName(ClusterPath)}";
                        loadNavItem.Content = "Load another Cluster";
                        foreach (var item in RootNavigation.MenuItems)
                        {
                            if (item is NavigationViewItem)
                            {
                                NavigationViewItem navItem = (NavigationViewItem)item;
                                if (!navItem.IsEnabled) navItem.IsEnabled = true;
                            }
                        }

                        RootNavigation.ClearJournal();
                        RootNavigation.Navigate(typeof(ClusterHealthPage));
                        Log.WriteLog([ClusterPath], LogType.LoadCluster);
                    }
                }
            }
        }

        public void OpenClusterSelectionDialog() {
            OpenFolderDialog ofd = new OpenFolderDialog();
            if (ofd.ShowDialog() == true)
            {
                ClusterPath = ofd.FolderName;
                List<ProgramType> programs = ProgramType.ReadClusterFile(ofd.FolderName);
                if (programs == null)
                {
                    MessageBox msg = new() { Title = "Invalid folder", Content = "The chosen folder doesn't contain a .klaszter file." };
                    msg.ShowDialogAsync();
                }
                else
                {
                    lblPath.Content = $"Cluster: {Path.GetFileName(ClusterPath)}";
                    loadNavItem.Content = "Load another Cluster";
                    foreach (var item in RootNavigation.MenuItems)
                    {
                        if (item is NavigationViewItem)
                        {
                            NavigationViewItem navItem = (NavigationViewItem)item;
                            if (!navItem.IsEnabled) navItem.IsEnabled = true;
                        }
                    }

                    RootNavigation.ClearJournal();
                    RootNavigation.Navigate(typeof(ClusterHealthPage));
                    Log.WriteLog([ClusterPath], LogType.LoadCluster);
                }
            }
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            RootNavigation.Navigated += RootNavigationOnNavigated;

            originalBreadcrumbs = BreadcrumbBar.ItemsSource;
            
            RootNavigation.Navigate(typeof(StartPage));
            RootNavigation.ClearJournal();
            LoadLastOpenedCluster();
        }

        private void RootNavigationOnNavigated(NavigationView sender, NavigatedEventArgs args)
        {
            BreadcrumbBar.ItemsSource = originalBreadcrumbs;
            
            if (args.Page is CustomPage page && page.HeaderControls.Any())
            {
                icHeaderControls.ItemsSource = page.HeaderControls;
            }
            else
            {
                icHeaderControls.ItemsSource = null;
            }
        }

        private void loadNavItem_Click(object sender, RoutedEventArgs e)
        {
            OpenClusterSelectionDialog();
        }

        private void FluentWindow_Closed(object sender, EventArgs e)
        {
            Log.WriteLog([], LogType.CloseProgram);
        }
    }
}