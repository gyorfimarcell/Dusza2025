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
using System.Windows.Media.Media3D;
using System.ComponentModel;

namespace Cluster
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged
    {
        public static string ClusterPath { get; set; } = "";
        
        public SnackbarService RootSnackbarService { get; private set; }
        public readonly ContentDialogService _dialogService;

        private bool _darkMode = false;
        public bool DarkMode
        {
            get => _darkMode; set
            {
                _darkMode = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DarkMode)));
                Wpf.Ui.Appearance.ApplicationThemeManager.Apply(
                    _darkMode ? Wpf.Ui.Appearance.ApplicationTheme.Dark : Wpf.Ui.Appearance.ApplicationTheme.Light,
                    WindowBackdropType.None,
                    true
                );
                Registry.SetValue(SETTINGS_KEY, "darkMode", _darkMode);
            }
        }
        private const string SETTINGS_KEY = @"HKEY_CURRENT_USER\SOFTWARE\kibirodKolega\Cluster\";

        private IEnumerable originalBreadcrumbs;

        public event PropertyChangedEventHandler? PropertyChanged;

        public MainWindow()
        {
            InitializeComponent();
            
            RootSnackbarService = new SnackbarService();
            RootSnackbarService.SetSnackbarPresenter(RootSnackbarPresenter);

            _dialogService = new ContentDialogService();
            _dialogService.SetDialogHost(RootContentDialog);
            
            Loaded += MainWindow_Loaded;
            Log.WriteLog([], LogType.OpenProgram);
        }

        public void RefreshLblPath()
        {
            lblPath.Content = $"Cluster: {Path.GetFileName(ClusterPath)}";
        }

        public void EnableNavigationItems()
        {
            foreach (var item in RootNavigation.MenuItems)
            {
                if (item is NavigationViewItem)
                {
                    NavigationViewItem navItem = (NavigationViewItem)item;
                    if (!navItem.IsEnabled) navItem.IsEnabled = true;
                }
            }
        }

        private void LoadLastOpenedCluster()
        {
            string directoryPath = Log.GetLogDirectoryPath();

            if (Directory.Exists(directoryPath))
            {
                string[] files = Directory.GetFiles(directoryPath, "*.log");

                if (files != null)
                {
                    var latestFile = files.Select(file => new
                    {
                        FilePath = file,
                    })
                    .Where(f => f.FilePath != null)
                    .OrderByDescending(f => Path.GetFileNameWithoutExtension(f.FilePath))
                    .FirstOrDefault(f => File.ReadAllLines(f.FilePath).Any(x => x.StartsWith("LoadCluster")));


                    string[] clusterLines = File.ReadAllLines(latestFile.FilePath).Where(x => x.StartsWith("LoadCluster")).ToArray();

                    if (clusterLines.Length == 0) 
                    {
                        return;
                    }

                    ClusterPath = clusterLines.Last().Split(" - ").Last();

                    if (ClusterPath != null && ProgramType.ReadClusterFile(ClusterPath) != null)
                    {
                        RefreshLblPath();
                        loadNavItem.Content = "Load another Cluster";
                        EnableNavigationItems();

                        RootNavigation.ClearJournal();
                        RootNavigation.Navigate(typeof(ClusterHealthPage));
                        Log.WriteLog([ClusterPath], LogType.LoadCluster);
                    } 
                    else
                    {
                        ClusterPath = null;
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
                    EnableNavigationItems();

                    RootNavigation.ClearJournal();
                    RootNavigation.Navigate(typeof(ClusterHealthPage));
                    Log.WriteLog([ClusterPath], LogType.LoadCluster);
                }
            }
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            bool savedDarkMode = Registry.GetValue(SETTINGS_KEY, "darkMode", false) is string s && s == "True";
            DarkMode = savedDarkMode;

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

        private void MenuItemTheme_Click(object sender, RoutedEventArgs e)
        {
            DarkMode = !DarkMode;
        }
    }
}