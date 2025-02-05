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

namespace Cluster.Controls
{
    /// <summary>
    /// Interaction logic for OptimizeDialog.xaml
    /// </summary>
    public partial class OptimizeDialog : UserControl
    {
        string path;
        MainWindow window;
        public OptimizeDialog()
        {
            InitializeComponent();

            path = MainWindow.ClusterPath;
            window = (MainWindow)Application.Current.MainWindow!;
            sliMaximumPercentage.ValueChanged += volume_ValueChanged;
            sliMinimumPercentage.ValueChanged += volume_ValueChanged;
        }

        private void volume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> args)
        {
            Slider slider = (Slider)sender;
            double value = (double)slider.Value;

            if (slider.Name == "sliMinimumPercentage")
            {
                txtMinimumPercentage.Text = value.ToString() + " %";
                sliMaximumPercentage.Minimum = value;
            }

            else if (slider.Name == "sliMaximumPercentage")
            {
                txtMaximumPercentage.Text = value.ToString() + " %";
                sliMinimumPercentage.Maximum = value;
            }


            //sp.Children.OfType<StackPanel>().ToList()
            //    .SelectMany(x => x.Children.OfType<TextBlock>().Select(x => (Run)x.Inlines.FirstInline))
            //    .ToList()[slider.Name == "txtMinimumPercentage" ? 0 : 1].Text = value.ToString();

        }
    }
}
