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
using System.Windows.Shapes;
using Wpf.Ui.Controls;

namespace Cluster
{
    /// <summary>
    /// Interaction logic for ModifyProgram.xaml
    /// </summary>
    public partial class ModifyProgramPage : CustomPage
    {
        ProgramType Program;

        public ModifyProgramPage()
        {
            InitializeComponent();

            Loaded += ModifyProgramPage_Loaded;
        }

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

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if(nbActive.Value == null || nbProcessor.Value == null || nbMemory.Value == null)
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

            bool result = ProgramType.ModifyProgram(Program, (int)nbActive.Value, (int)nbProcessor.Value, (int)nbMemory.Value);
            if (result)
            {
                Log.WriteLog([Program.ProgramName, $"{nbProcessor.Value}", $"{nbMemory.Value}", $"{nbActive.Value}"], LogType.ModifyProgram);
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
