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
using Wpf.Ui;
using Wpf.Ui.Controls;
using Wpf.Ui.Markup;

namespace Cluster
{
    /// <summary>
    /// Interaction logic for AddNewProgramPage.xaml
    /// </summary>
    public partial class AddNewProgramPage : Page
    {
        string path;
        MainWindow _window;
        public AddNewProgramPage()
        {
            InitializeComponent();

            path = MainWindow.ClusterPath;
            _window = (MainWindow)Application.Current.MainWindow!;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!Validate.ValidateFileName(tbProgramName.Text) || nbActive.Value == null || 
                nbProcessor.Value == null || nbMemory.Value == null)
            {
                _window.RootSnackbarService.Show("Error", "You must fill out all fields!", ControlAppearance.Danger,
                   new SymbolIcon(SymbolRegular.Warning24), TimeSpan.FromSeconds(3));
                return;
            }

            ProgramType program = new(tbProgramName.Text, (int)nbActive.Value, (int)nbProcessor.Value, (int)nbMemory.Value);

            try
            {
                if (ProgramType.ReadClusterFile(path).Select(x => x.ProgramName).Contains(program.ProgramName))
                {
                    _window.RootSnackbarService.Show(
                        "Error",
                        "This program already exists in the cluster!",
                        ControlAppearance.Danger,
                        new SymbolIcon { Symbol = SymbolRegular.Warning24 },
                        TimeSpan.FromSeconds(3)
                    );

                    return;
                }

                program.AddNewProgramToCluster(path);

                Log.WriteLog([program.ProgramName, $"{program.CpuMilliCore}", $"{program.Memory}", $"{program.ActivePrograms}"], LogType.AddProgram);

            }
            catch (Exception ex)
            {
                _window.RootSnackbarService.Show(
                    "Error",
                    ex.Message,
                    ControlAppearance.Danger,
                    new SymbolIcon { Symbol = SymbolRegular.Warning24 },
                    TimeSpan.FromSeconds(3)
                );

                return;
            }
            tbProgramName.Clear();
            nbActive.Clear();
            nbProcessor.Clear();
            nbMemory.Clear();
            _window.RootSnackbarService.Show(
                "Success",
                "Program added successfully!",
                ControlAppearance.Success,
                new SymbolIcon { Symbol = SymbolRegular.Checkmark24 },
                TimeSpan.FromSeconds(3)
            );

            tbProgramName.Focus();

        }
    }
}
