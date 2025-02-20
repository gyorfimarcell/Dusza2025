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
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.Themes;
using SkiaSharp;
using LiveChartsCore.SkiaSharpView.Painting;
using System.Globalization;

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

                Wpf.Ui.Appearance.ApplicationThemeManager.Apply(
                    _darkMode ? Wpf.Ui.Appearance.ApplicationTheme.Dark : Wpf.Ui.Appearance.ApplicationTheme.Light,
                    WindowBackdropType.None,
                    true
                );
                LiveCharts.Configure(config => {
                    if (_darkMode) config.AddDarkTheme(theme => theme.Colors = ColorPalletes.MaterialDesign500);
                    else
                    {
                        config.AddLightTheme();
                        config.LegendTextPaint = new SolidColorPaint(new SKColor(35, 35, 35));
                    }
                });

                Registry.SetValue(SETTINGS_KEY, "darkMode", _darkMode);

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DarkMode)));
            }
        }
        public const string SETTINGS_KEY = @"HKEY_CURRENT_USER\SOFTWARE\kibirodKolega\Cluster\";

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

        /// <summary>
        /// Refreshes the label that shows the path of the currently loaded cluster
        /// </summary>
        public void RefreshLblPath()
        {
            lblPath.Text = Path.GetFileName(ClusterPath);
            loadNavItem.Content = TranslationSource.T("Menu.LoadAnother");
        }

        /// <summary>
        /// Enables all navigation items in the NavigationView
        /// </summary>
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

        /// <summary>
        /// Loads the last opened cluster from the log files
        /// </summary>
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

                    if (latestFile == null)
                    {
                        return;
                    }   

                    string[] clusterLines = File.ReadAllLines(latestFile.FilePath).Where(x => x.StartsWith("LoadCluster")).ToArray();

                    if (clusterLines.Length == 0) 
                    {
                        return;
                    }

                    ClusterPath = clusterLines.Last().Split(" - ").Last();

                    if (ClusterPath != null && ProgramType.ReadClusterFile(ClusterPath) != null)
                    {
                        RefreshLblPath();
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

        /// <summary>
        /// Opens a dialog to select a cluster folder
        /// </summary>
        public void OpenClusterSelectionDialog() {
            OpenFolderDialog ofd = new OpenFolderDialog();
            if (ofd.ShowDialog() == true)
            {
                ClusterPath = ofd.FolderName;
                List<ProgramType> programs = ProgramType.ReadClusterFile(ofd.FolderName);
                if (programs == null)
                {
                    MessageBox msg = new() { Title = TranslationSource.T("Menu.Invalid.Title"), Content = TranslationSource.T("Menu.Invalid.Text"), CloseButtonText = TranslationSource.T("Close") };
                    msg.ShowDialogAsync();
                }
                else
                {
                    lblPath.Text = Path.GetFileName(ClusterPath);
                    loadNavItem.Content = TranslationSource.T("Menu.LoadAnother");
                    EnableNavigationItems();

                    RootNavigation.ClearJournal();
                    RootNavigation.Navigate(typeof(ClusterHealthPage));
                    Log.WriteLog([ClusterPath], LogType.LoadCluster);
                }
            }
        }

        /// <summary>
        /// If MainWindow loaded sets light or dark mode then switches to the last opened cluster
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            bool savedDarkMode = Registry.GetValue(SETTINGS_KEY, "darkMode", false) is string s && s == "True";
            DarkMode = savedDarkMode;

            object? languageSetting = Registry.GetValue(SETTINGS_KEY, "language", "hu-HU");
            if (languageSetting is string savedLanguage)
            {
                TranslationSource.Instance.CurrentCulture = new CultureInfo(savedLanguage);
            }
            else {
                TranslationSource.Instance.CurrentCulture = new CultureInfo("hu-HU");
            }

            RootNavigation.Navigated += RootNavigationOnNavigated;

            originalBreadcrumbs = BreadcrumbBar.ItemsSource;
            
            RootNavigation.Navigate(typeof(StartPage));
            RootNavigation.ClearJournal();
            LoadLastOpenedCluster();
        }

        /// <summary>
        /// Sets the header controls and the breadcrumbs for the current page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
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

        /// <summary>
        /// Opens the cluster selection dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loadNavItem_Click(object sender, RoutedEventArgs e)
        {
            OpenClusterSelectionDialog();
        }

        /// <summary>
        /// Closes the program
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FluentWindow_Closed(object sender, EventArgs e)
        {
            Log.WriteLog([], LogType.CloseProgram);
        }

        /// <summary>
        /// Switches between light and dark mode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemTheme_Click(object sender, RoutedEventArgs e)
        {
            DarkMode = !DarkMode;
        }

        private void LblPath_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (ClusterPath != "")
            {
                System.Diagnostics.Process.Start("explorer.exe", ClusterPath);
            }
        }

        private void LblPath_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key is Key.Enter or Key.Space && ClusterPath != "")
            {
                System.Diagnostics.Process.Start("explorer.exe", ClusterPath);
            }
        }
    }
}