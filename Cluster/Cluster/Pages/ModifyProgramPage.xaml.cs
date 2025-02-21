using System.Windows;
using Wpf.Ui.Controls;

namespace Cluster
{
    /// <summary>
    /// Interaction logic for ModifyProgram.xaml
    /// </summary>
    public partial class ModifyProgramPage
    {
        private ProgramType Program = null!;

        public ModifyProgramPage()
        {
            InitializeComponent();

            Loaded += ModifyProgramPage_Loaded;
        }

        /// <summary>
        /// Load the program data into the form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ModifyProgramPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is not ProgramType program)
            {
                _window.RootNavigation.GoBack();
                return;
            }

            Program = program;
            ChangeTitle(TranslationSource.Instance.WithParam("ModifyProgramPage.Title", program.ProgramName));
            nbActive.Value = Program.ActivePrograms;
            nbProcessor.Value = Program.CpuMilliCore;
            nbMemory.Value = Program.Memory;
        }

        /// <summary>
        /// Save the modified program
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (nbActive.Value == null || nbProcessor.Value == null || nbMemory.Value == null)
            {
                _window.RootSnackbarService.Show(
                    TranslationSource.T("Errors.Error"),
                    TranslationSource.T("Errors.MissingFields"),
                    ControlAppearance.Danger,
                    new SymbolIcon { Symbol = SymbolRegular.Warning24 },
                    TimeSpan.FromSeconds(10)
                );
                return;
            }

            bool result = ProgramType.ModifyProgram(Program, (int)nbActive.Value, (int)nbProcessor.Value,
                (int)nbMemory.Value);
            if (result)
            {
                Log.WriteLog([Program.ProgramName, $"{nbProcessor.Value}", $"{nbMemory.Value}", $"{nbActive.Value}"],
                    LogType.ModifyProgram);
                _window.RootSnackbarService.Show(
                    TranslationSource.T("Success"),
                    $"'{Program.ProgramName}' {TranslationSource.T("ModifyProgramPage.Success.Text")}",
                    ControlAppearance.Success,
                    new SymbolIcon { Symbol = SymbolRegular.Checkmark24 },
                    TimeSpan.FromSeconds(10)
                );
                _window.RootNavigation.Navigate(typeof(ProgramsPage));
            }
        }
    }
}