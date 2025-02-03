using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for ProcessCard.xaml
    /// </summary>
    public partial class ProcessCard : UserControl, INotifyPropertyChanged
    {
        public ProcessCard()
        {
            InitializeComponent();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public static readonly DependencyProperty ProcessProperty =
        DependencyProperty.Register(nameof(Process), typeof(Process), typeof(ProcessCard), new PropertyMetadata(null));

        public Process Process
        {
            get => (Process)GetValue(ProcessProperty);
            set {
                SetValue(ProcessProperty, value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Process)));
            }
        }
    }
}
