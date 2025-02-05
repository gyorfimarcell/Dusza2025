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
            MessageBox mgbox = new()
            {
                Title = "Error",
                Content = new OptimizeDialog(),
                IsPrimaryButtonEnabled = true,
                IsSecondaryButtonEnabled = false,
                //Background = new SolidColorBrush(Color.FromRgb(244, 66, 54)),
                PrimaryButtonText = "Optimize",
                CloseButtonText = "Cancel"

            };
            MessageBoxResult result = mgbox.ShowDialogAsync().GetAwaiter().GetResult();
            mgbox.ShowDialogAsync();
        }

        
    }
}
