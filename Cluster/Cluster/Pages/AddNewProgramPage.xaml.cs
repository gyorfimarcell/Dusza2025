using System.Windows;
using Wpf.Ui.Controls;

namespace Cluster
{
    /// <summary>
    /// Interaction logic for AddNewProgramPage.xaml
    /// </summary>
    public partial class AddNewProgramPage
    {
        private readonly string path;
        private readonly MainWindow _window;

        public AddNewProgramPage()
        {
            InitializeComponent();

            path = MainWindow.ClusterPath;
            _window = (MainWindow)Application.Current.MainWindow!;
        }

        /// <summary>
        /// Saves the new program to the cluster.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!Validate.ValidateFileName(tbProgramName.Text) || nbActive.Value == null ||
                nbProcessor.Value == null || nbMemory.Value == null)
            {
                _window.RootSnackbarService.Show(TranslationSource.T("Errors.Error"),
                    TranslationSource.T("Errors.MissingFields"), ControlAppearance.Danger,
                    new SymbolIcon(SymbolRegular.Warning24), TimeSpan.FromSeconds(10));
                return;
            }

            ProgramType program = new(tbProgramName.Text, (int)nbActive.Value, (int)nbProcessor.Value,
                (int)nbMemory.Value);

            try
            {
                if (ProgramType.ReadClusterFile(path).Select(x => x.ProgramName).Contains(program.ProgramName))
                {
                    _window.RootSnackbarService.Show(
                        TranslationSource.T("Errors.Error"),
                        TranslationSource.T("Errors.ProgramAlreadyExists"),
                        ControlAppearance.Danger,
                        new SymbolIcon { Symbol = SymbolRegular.Warning24 },
                        TimeSpan.FromSeconds(10)
                    );

                    return;
                }

                program.AddNewProgramToCluster(path);

                Log.WriteLog(
                    [program.ProgramName, $"{program.CpuMilliCore}", $"{program.Memory}", $"{program.ActivePrograms}"],
                    LogType.AddProgram);
            }
            catch (Exception ex)
            {
                _window.RootSnackbarService.Show(
                    TranslationSource.T("Errors.Error"),
                    ex.Message,
                    ControlAppearance.Danger,
                    new SymbolIcon { Symbol = SymbolRegular.Warning24 },
                    TimeSpan.FromSeconds(10)
                );

                return;
            }

            tbProgramName.Clear();
            nbActive.Clear();
            nbProcessor.Clear();
            nbMemory.Clear();
            _window.RootSnackbarService.Show(
                TranslationSource.T("AddProgramPage.Success.Title"),
                $"'{program.ProgramName}' {TranslationSource.T("AddProgramPage.Success.Text")}",
                ControlAppearance.Success,
                new SymbolIcon { Symbol = SymbolRegular.Checkmark24 },
                TimeSpan.FromSeconds(10)
            );

            tbProgramName.Focus();
        }
    }
}