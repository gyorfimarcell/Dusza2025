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
        SnackbarService snackbarService;
        public AddNewProgramPage()
        {
            InitializeComponent();

            path = MainWindow.ClusterPath;

            snackbarService = new();
            snackbarService.SetSnackbarPresenter(snackbarPresenter);

        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (txtProgramName.Text.Length < 1 ||
                !int.TryParse(txtActivePrograms.Text, out int activePrograms) || activePrograms < 1 ||
                !uint.TryParse(txtCpuMilliCore.Text, out uint cpuMilliCore) || cpuMilliCore < 1 ||
                !uint.TryParse(txtMemory.Text, out uint memory) || memory < 1)
            {
                snackbarService.Show(
                    "Error",
                    "Please fill out all the fields in proper format!",
                    ControlAppearance.Danger,
                    new SymbolIcon { Symbol = SymbolRegular.ErrorCircle24 },
                    TimeSpan.FromSeconds(5)
                );
                return;
            }

            ProgramType program = new(txtProgramName.Text, int.Parse(txtActivePrograms.Text), int.Parse(txtCpuMilliCore.Text), int.Parse(txtMemory.Text));

            try
            {
                if (ProgramType.ReadClusterFile(path).Select(x => x.ProgramName).Contains(program.ProgramName))
                {
                    snackbarService.Show(
                        "Error",
                        "This program already exists in the cluster!",
                        ControlAppearance.Danger,
                        new SymbolIcon { Symbol = SymbolRegular.ErrorCircle24 },
                        TimeSpan.FromSeconds(5)
                    );

                    return;
                }

                program.AddNewProgramToCluster(path);

                Log.WriteLog([program.ProgramName, $"{program.CpuMilliCore}", $"{program.Memory}", $"{program.ActivePrograms}"], LogType.AddProgram);

            }
            catch (Exception ex)
            {
                snackbarService.Show(
                    "Error",
                    ex.Message,
                    ControlAppearance.Danger,
                    new SymbolIcon { Symbol = SymbolRegular.ErrorCircle24 },
                    TimeSpan.FromSeconds(5)
                );

                return;
            }
            txtProgramName.Text = txtProgramName.Text = txtActivePrograms.Text = txtCpuMilliCore.Text = txtMemory.Text = "";
            snackbarService.Show(
                "Success",
                "Program added successfully!",
                ControlAppearance.Success,
                new SymbolIcon { Symbol = SymbolRegular.Check24 },
                TimeSpan.FromSeconds(5)
            );

            txtProgramName.Focus();

        }
    }
}
