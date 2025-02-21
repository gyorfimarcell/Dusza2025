using System.Windows;
using Wpf.Ui.Controls;

namespace Cluster
{
    /// <summary>
    /// Interaction logic for ClusterHealthPage.xaml
    /// </summary>
    public partial class ClusterHealthPage
    {
        private ClusterHealth health;

        public ClusterHealthPage()
        {
            InitializeComponent();
            health = new ClusterHealth(Computer.GetComputers(MainWindow.ClusterPath),
                ProgramType.ReadClusterFile(MainWindow.ClusterPath));
            Loaded += ClusterHealthPage_Loaded;
        }

        /// <summary>
        /// Checks the health of the cluster and displays the results
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClusterHealthPage_Loaded(object sender, RoutedEventArgs e)
        {
            health = new(Computer.GetComputers(MainWindow.ClusterPath),
                ProgramType.ReadClusterFile(MainWindow.ClusterPath));
            if (health.Ok)
            {
                HealthyInfobar.IsOpen = true;
                spFixIssues.Visibility = Visibility.Hidden;
            }
            else
            {
                spFixIssues.Visibility = Visibility.Visible;
                foreach (string error in health.Errors)
                {
                    InfoBar infoBar = new()
                    {
                        Title = TranslationSource.T("Errors.Error"),
                        Message = error,
                        IsClosable = false,
                        Severity = InfoBarSeverity.Error,
                        IsOpen = true,
                        Margin = new Thickness(0, 0, 0, 8)
                    };
                    spErrors.Children.Add(infoBar);
                }
            }
        }

        /// <summary>
        /// Fixes the issues in the cluster
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FixIssues_Click(object sender, RoutedEventArgs e)
        {
            int allResCount = 0;
            while (!health.Ok)
            {
                int res = ClusterHealth.FixIssues();
                UIElement? clusterOk = spErrors.Children[0];
                spErrors.Children.Clear();
                spErrors.Children.Add(clusterOk);
                ClusterHealthPage_Loaded(new(), new());

                if (res == 0)
                {
                    _window.RootSnackbarService.Show(TranslationSource.T("Errors.Error"),
                        TranslationSource.T("Errors.FixFail"), ControlAppearance.Danger,
                        new SymbolIcon(SymbolRegular.Warning24), TimeSpan.FromSeconds(10));
                    return;
                }

                allResCount += res;
            }

            Log.WriteLog([$"{allResCount}"], LogType.FixIssues);
            _window.RootSnackbarService.Show(TranslationSource.T("Success"),
                TranslationSource.Instance.WithParam("HealthPage.Fixed", allResCount.ToString()),
                ControlAppearance.Success, new SymbolIcon(SymbolRegular.Checkmark24), TimeSpan.FromSeconds(10));
        }
    }
}