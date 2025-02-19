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
    public partial class ModifyComputerPage : CustomPage
    {
        Computer PageComputer;

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
            ChangeTitle($"Edit {PageComputer.Name}");

            nbProcessor.Value = PageComputer.ProcessorCore;
            nbMemory.Value = PageComputer.RamCapacity;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if(nbProcessor.Value == null || nbMemory.Value == null)
            {
                _window.RootSnackbarService.Show(
                        "Error",
                        "You must fill out all fields!",
                        ControlAppearance.Danger,
                        new SymbolIcon { Symbol = SymbolRegular.Warning24 },
                        TimeSpan.FromSeconds(3)
                    );
                return;
            }

            string? result = PageComputer.Modify((int)nbProcessor.Value, (int)nbMemory.Value);
            if (result == null)
            {
                Log.WriteLog([PageComputer.Name, $"{nbProcessor.Value}", $"{nbMemory.Value}"], LogType.ModifyComputer);
                _window.RootSnackbarService.Show(
                    "Success",
                    $"Computer '{PageComputer.Name}' successfully modified!",
                    ControlAppearance.Success,
                    new SymbolIcon { Symbol = SymbolRegular.Checkmark24 },
                    TimeSpan.FromSeconds(3)
                );
                _window.RootNavigation.Navigate(typeof(ComputersPage));
            }
            else
            {
                _window.RootSnackbarService.Show(
                    "Error",
                    result,
                    ControlAppearance.Danger,
                    new SymbolIcon { Symbol = SymbolRegular.Warning24 },
                    TimeSpan.FromSeconds(3)
                );
            }
        }
    }
}
