using Cluster.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Wpf.Ui.Controls;
using Button = Wpf.Ui.Controls.Button;
using MessageBox = Wpf.Ui.Controls.MessageBox;
using MessageBoxResult = Wpf.Ui.Controls.MessageBoxResult;
using TextBlock = Wpf.Ui.Controls.TextBlock;

namespace Cluster
{
    /// <summary>
    /// Interaktionslogik für OptimizingComputersPage.xaml
    /// </summary>
    public partial class OptimizingComputersPage : Page
    {
        string path;
        MainWindow window;
        public OptimizingComputersPage()
        {
            InitializeComponent();
            path = MainWindow.ClusterPath;
            window = (MainWindow)Application.Current.MainWindow!;

            //sp = (StackPanel)Resources["DialogContent"];
            //
            //sp.Children.OfType<StackPanel>().ToList().SelectMany(x => x.Children.OfType<Slider>()).ToList().ForEach(x => x.ValueChanged += volume_ValueChanged);
        }

        private void btnOptimizeDialog_Click(object sender, RoutedEventArgs e)
        {
            //var dialog = new ContentDialog
            //{
            //    Title = "WPF UI Dialog",
            //    Content = "This is a modern dialog using ContentDialogService.",
            //    PrimaryButtonText = "OK",
            //    SecondaryButtonText = "Cancel",
            //    CloseButtonText = "Close"
            //};
            //
            //// Show the dialog asynchronously
            //var result = await mainWindow._dialogService.ShowAsync(dialog, );

            OptimizeDialog optimizeDialog = new();
            MessageBox mgbox = new()
            {
                Title = "Set Optimizing Values",
                Content = optimizeDialog,
                IsPrimaryButtonEnabled = true,
                IsSecondaryButtonEnabled = false,
                PrimaryButtonText = "Optimize",
                CloseButtonText = "Cancel",
                Width = 500,
                MaxWidth = 500,
                MaxHeight = 1000
                //Icon = new BitmapImage(new Uri("pack://application:,,,/Resources/Icons/optimizing.png"))

            };
            MessageBoxResult result = mgbox.ShowDialogAsync().GetAwaiter().GetResult();

            // Hit cancel
            if (result != MessageBoxResult.Primary)
                return;

            if (!Computer.CanOptimizeComputers(optimizeDialog.Minimum, optimizeDialog.Maximum))
            {
                mgbox = new()
                {
                    Title = "Error",
                    Content = "Optimizing cannot be done with the given values! Would you like to spread the processes equally?",
                    IsPrimaryButtonEnabled = true,
                    IsSecondaryButtonEnabled = false,
                    PrimaryButtonText = "Spread",
                    CloseButtonText = "Cancel",
                };
                result = mgbox.ShowDialogAsync().GetAwaiter().GetResult();

                // Hit cancel
                if (result != MessageBoxResult.Primary)
                    return;

                string? spreadRes = Computer.SpreadProcesses();
                if (spreadRes != null)
                {
                    window.RootSnackbarService.Show("Error", spreadRes, ControlAppearance.Danger,
                        new SymbolIcon(SymbolRegular.Warning24), TimeSpan.FromSeconds(3));
                }
                else
                {
                    window.RootSnackbarService.Show("Success", "Processes were spread equally!", ControlAppearance.Success, new SymbolIcon(SymbolRegular.Check24), TimeSpan.FromSeconds(3));
                }
                return;
            }
            string? optimizeRes = Computer.OptimizeComputers(optimizeDialog.Minimum, optimizeDialog.Maximum);

            if (optimizeRes != null)
            {
                window.RootSnackbarService.Show("Error", optimizeRes, ControlAppearance.Danger,
                    new SymbolIcon(SymbolRegular.Warning24), TimeSpan.FromSeconds(3));
                return;
            }
            window.RootSnackbarService.Show("Success", "Optimization was successful!", ControlAppearance.Success, new SymbolIcon(SymbolRegular.Check24), TimeSpan.FromSeconds(3));
        }

    }
}
