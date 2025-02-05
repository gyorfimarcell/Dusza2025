using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        MainWindow mainWindow;
        StackPanel sp;
        public OptimizingComputersPage()
        {
            InitializeComponent();
            mainWindow = (MainWindow)Application.Current.MainWindow;

            sp = (StackPanel)Resources["DialogContent"];

            sp.Children.OfType<StackPanel>().ToList().SelectMany(x => x.Children.OfType<Slider>()).ToList().ForEach(x => x.ValueChanged += volume_ValueChanged);
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
                Content = Resources["DialogContent"],
                IsPrimaryButtonEnabled = true,
                IsSecondaryButtonEnabled = false,
                //Background = new SolidColorBrush(Color.FromRgb(244, 66, 54)),
                PrimaryButtonText = "Yes",
                CloseButtonText = "Cancel"

            };
            MessageBoxResult result = mgbox.ShowDialogAsync().GetAwaiter().GetResult();
            mgbox.ShowDialogAsync();
        }

        private void volume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> args)
        {
            Slider slider = (Slider)sender;
            double value = (double)slider.Value;

            sp.Children.OfType<StackPanel>().ToList()
                .SelectMany(x => x.Children.OfType<TextBlock>().Select(x => (Run)x.Inlines.FirstInline))
                .ToList()[slider.Name == "txtMinimumPercentage" ? 0 : 1].Text = value.ToString();

        }
    }
}
