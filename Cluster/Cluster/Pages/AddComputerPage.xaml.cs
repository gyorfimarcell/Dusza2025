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
using Wpf.Ui;
using Wpf.Ui.Controls;
using MessageBox = System.Windows.MessageBox;

namespace Cluster
{
    /// <summary>
    /// Interaction logic for AddComputerWindow.xaml
    /// </summary>
    public partial class AddComputerPage : Page
    {
        string path;
        private MainWindow window;

        public AddComputerPage()
        {
            InitializeComponent();
            path = MainWindow.ClusterPath;
            window = (MainWindow)Application.Current.MainWindow!;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (tbName.Text == "" || nbProcessor.Value == null || nbMemory.Value == null)
            {
                window.RootSnackbarService.Show("Error", "You must fill out all fields!", ControlAppearance.Danger,
                    new SymbolIcon(SymbolRegular.Warning24), TimeSpan.FromSeconds(3));
                return;
            }

            int processor = Convert.ToInt32(nbProcessor.Value);
            int memory = Convert.ToInt32(nbMemory.Value);

            string? error = Computer.AddComputer(path, tbName.Text, processor, memory);
            if (error != null)
            {
                window.RootSnackbarService.Show("Error", error, ControlAppearance.Danger,
                    new SymbolIcon(SymbolRegular.Warning24), TimeSpan.FromSeconds(3));
            }
            else
            {
                window.RootSnackbarService.Show("Computer created", $"Computer '{tbName.Text}' successfully created.",
                    ControlAppearance.Success, new SymbolIcon(SymbolRegular.Checkmark24), TimeSpan.FromSeconds(3));

                tbName.Clear();
                nbProcessor.Clear();
                nbMemory.Clear();

                Log.WriteLog([tbName.Text, $"{processor}", $"{memory}"], LogType.AddComputer);
            }
        }
    }
}