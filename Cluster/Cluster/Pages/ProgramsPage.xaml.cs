using System;
using System.Collections.Generic;
using System.IO;
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
using Button = Wpf.Ui.Controls.Button;
using MessageBox = Wpf.Ui.Controls.MessageBox;
using MessageBoxResult = Wpf.Ui.Controls.MessageBoxResult;
using TextBlock = Wpf.Ui.Controls.TextBlock;

namespace Cluster
{
    public partial class ProgramsPage : CustomPage
    {
        List<ProgramType> Programs;

        public ProgramsPage()
        {
            InitializeComponent();
            LoadData();
        }

        public void LoadData()
        {
            Programs = ProgramType.ReadClusterFile(MainWindow.ClusterPath);
            icPrograms.ItemsSource = Programs;
        }

        private void CardAction_Click(object sender, RoutedEventArgs e)
        {
            CardAction card = (CardAction)sender;
            ProgramType program = (ProgramType)card.DataContext;
            _window.RootNavigation.Navigate(typeof(ProcessesPage), program.ProgramName);
        }

        private void MenuItemNew_Click(object sender, RoutedEventArgs e)
        {
            _window.RootNavigation.NavigateWithHierarchy(typeof(AddNewProgramPage));
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;

            Button card = (Button)sender;
            ProgramType program = (ProgramType)card.DataContext;
            _window.RootNavigation.NavigateWithHierarchy(typeof(ModifyProgramPage), program);
        }

        private async void btnShutdown_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;

            Button card = (Button)sender;
            ProgramType program = (ProgramType)card.DataContext;
            int processCount = Computer.GetComputers(MainWindow.ClusterPath)
                .Sum(x => x.processes.Count(y => y.ProgramName == program.ProgramName));

            MessageBox messageBox = new MessageBox()
            {
                Title = $"Shut down {program.ProgramName}?",
                Content = $"This will shut down all {processCount} processes.",
                PrimaryButtonText = "Shutdown",
                PrimaryButtonAppearance = ControlAppearance.Danger,
            };


            MessageBoxResult result = await messageBox.ShowDialogAsync();
            if (result == MessageBoxResult.Primary)
            {
                if (ProgramType.ShutdownProgram(program))
                { 
                    Log.WriteLog([program.ProgramName, $"{program.CpuMilliCore}", $"{program.Memory}"], LogType.ShutdownProgram);
                    _window.RootSnackbarService.Show(
                        "Success",
                        $"Program '{program.ProgramName}' successfully shut down!",
                        ControlAppearance.Success,
                        new SymbolIcon { Symbol = SymbolRegular.Checkmark24 },
                        TimeSpan.FromSeconds(3)
                    );
                    LoadData();
                }
            }
        }
    }
}
