using System.Windows;
using System.Windows.Controls;

namespace Cluster.Controls
{
    /// <summary>
    /// Interaction logic for OptimizeDialog.xaml
    /// </summary>
    public partial class OptimizeDialog
    {
        public int Minimum { get; private set; }
        public int Maximum { get; private set; }

        public OptimizeDialog()
        {
            InitializeComponent();

            sliMaximumPercentage.ValueChanged += volume_ValueChanged;
            sliMinimumPercentage.ValueChanged += volume_ValueChanged;

            Minimum = (int)sliMinimumPercentage.Value;
            Maximum = (int)sliMaximumPercentage.Value;
        }

        private void volume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> args)
        {
            var slider = (Slider)sender;
            double value = slider.Value;

            switch (slider.Name)
            {
                case "sliMinimumPercentage":
                    txtMinimumPercentage.Text = value + " %";
                    sliMaximumPercentage.Minimum = value;
                    break;
                case "sliMaximumPercentage":
                    txtMaximumPercentage.Text = value + " %";
                    sliMinimumPercentage.Maximum = value;
                    break;
            }

            Minimum = (int)sliMinimumPercentage.Value;
            Maximum = (int)sliMaximumPercentage.Value;
        }
    }
}