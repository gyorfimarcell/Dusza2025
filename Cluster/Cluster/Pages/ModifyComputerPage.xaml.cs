using System.Windows;
using Wpf.Ui.Controls;

namespace Cluster
{
    /// <summary>
    /// Interaction logic for ModifyProgram.xaml
    /// </summary>
    public partial class ModifyComputerPage
    {
        private Computer PageComputer = null!;

        public ModifyComputerPage()
        {
            InitializeComponent();

            Loaded += ModifyComputerPage_Loaded;
        }

        /// <summary>
        /// If page loaded get computer data and set it to the page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ModifyComputerPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is not Computer computer)
            {
                _window.RootNavigation.GoBack();
                return;
            }

            PageComputer = computer;
            ChangeTitle(TranslationSource.Instance.WithParam("ModifyComputerPage.Title", PageComputer.Name));

            nbProcessor.Value = PageComputer.ProcessorCore;
            nbMemory.Value = PageComputer.RamCapacity;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (nbProcessor.Value == null || nbMemory.Value == null)
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

            string? result = PageComputer.Modify((int)nbProcessor.Value, (int)nbMemory.Value);
            if (result == null)
            {
                Log.WriteLog([PageComputer.Name, $"{nbProcessor.Value}", $"{nbMemory.Value}"], LogType.ModifyComputer);
                _window.RootSnackbarService.Show(
                    TranslationSource.T("Success"),
                    $"'{PageComputer.Name}' {TranslationSource.T("ModifyComputerPage.Success.Text")}",
                    ControlAppearance.Success,
                    new SymbolIcon { Symbol = SymbolRegular.Checkmark24 },
                    TimeSpan.FromSeconds(10)
                );
                _window.RootNavigation.Navigate(typeof(ComputersPage));
            }
            else
            {
                _window.RootSnackbarService.Show(
                    TranslationSource.T("Errors.Error"),
                    result,
                    ControlAppearance.Danger,
                    new SymbolIcon { Symbol = SymbolRegular.Warning24 },
                    TimeSpan.FromSeconds(10)
                );
            }
        }
    }
}