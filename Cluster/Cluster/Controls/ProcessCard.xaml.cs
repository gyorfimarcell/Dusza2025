using System.ComponentModel;
using System.Windows;
using Wpf.Ui.Controls;

namespace Cluster.Controls
{
    /// <summary>
    /// Interaction logic for ProcessCard.xaml
    /// </summary>
    public partial class ProcessCard : INotifyPropertyChanged
    {
        public ProcessCard()
        {
            InitializeComponent();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public static readonly DependencyProperty ProcessProperty =
            DependencyProperty.Register(nameof(Process), typeof(Process), typeof(ProcessCard),
                new PropertyMetadata(null));

        public Process Process
        {
            get => (Process)GetValue(ProcessProperty);
            set
            {
                SetValue(ProcessProperty, value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Process)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Title)));
            }
        }

        public static readonly DependencyProperty ShowComputerProperty =
            DependencyProperty.Register(nameof(ShowComputer), typeof(bool), typeof(ProcessCard),
                new PropertyMetadata(false));

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

        public event ProcessShutdownHandler OnProcessShutdown = null!;

        public delegate void ProcessActivateHandler(object sender, EventArgs e);

        public event ProcessActivateHandler OnProcessActivate = null!;

        private void btnShutdown_Click(object sender, RoutedEventArgs e)
        {
            string? res = Process.Shutdown();

            OnProcessShutdown?.Invoke(this, EventArgs.Empty);

            if (res == null)
            {
                ((MainWindow)Application.Current.MainWindow!).RootSnackbarService.Show(TranslationSource.T("Success"), $"'{Process.FileName}' {TranslationSource.T("ProgramsPage.Shutdown.Success")}",
                    ControlAppearance.Success, new SymbolIcon(SymbolRegular.Checkmark24), TimeSpan.FromSeconds(10));
                return;
            }
            ((MainWindow)Application.Current.MainWindow!).RootSnackbarService.Show(TranslationSource.T("Errors.Error"), res,
                ControlAppearance.Danger, new SymbolIcon(SymbolRegular.Warning24), TimeSpan.FromSeconds(10));
        }

        private void btnActivate_Click(object sender, RoutedEventArgs e)
        {
            bool res = Process.ToggleActive();
            if (!res)
            {
                var window = (MainWindow)Application.Current.MainWindow!;
                window.RootSnackbarService.Show(TranslationSource.T("Errors.Error"),
                    TranslationSource.T("Errors.NotEnoughResources"),
                    ControlAppearance.Danger, new SymbolIcon(SymbolRegular.Warning24), TimeSpan.FromSeconds(10));
                return;
            }

            OnProcessActivate?.Invoke(this, EventArgs.Empty);
        }
    }
}