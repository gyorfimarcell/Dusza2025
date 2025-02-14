using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
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
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Title)));
            }
        }

        public static readonly DependencyProperty ShowComputerProperty =
        DependencyProperty.Register(nameof(ShowComputer), typeof(bool), typeof(ProcessCard), new PropertyMetadata(false));

        public bool ShowComputer
        {
            get => (bool)GetValue(ShowComputerProperty);
            set
            {
                SetValue(ShowComputerProperty, value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ShowComputer)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Title)));
            }
        }

        public string Title => ShowComputer ? $"{Process.FileName} ({Process.HostComputer.Name})" : Process.FileName;

        public delegate void ProcessShutdownHandler(object sender, EventArgs e);
        public event ProcessShutdownHandler OnProcessShutdown;

        public delegate void ProcessActivateHandler(object sender, EventArgs e);
        public event ProcessShutdownHandler OnProcessActivate;

        private void btnShutdown_Click(object sender, RoutedEventArgs e)
        {
            Process.Shutdown();

            if (OnProcessShutdown == null) return;
            OnProcessShutdown(this, new());
        }

        private void btnActivate_Click(object sender, RoutedEventArgs e)
        {
            if (Process.Active == false) {
                Computer host = Process.HostComputer;

                if (host.ProcessorUsage + Process.ProcessorUsage > host.ProcessorCore ||
                    host.MemoryUsage + Process.MemoryUsage > host.RamCapacity) {

                    MainWindow window = (MainWindow)Application.Current.MainWindow!;
                    window.RootSnackbarService.Show("Error", $"Computer '{host.Name}' doesn't have enough resources!",
                        ControlAppearance.Danger, new SymbolIcon(SymbolRegular.Warning24), TimeSpan.FromSeconds(3));
                    return;
                }
            }
            Process.ToggleActive();

            if (OnProcessActivate == null) return;
            OnProcessActivate(this, new());
        }
    }
}
