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
using Wpf.Ui.Controls;
using Button = Wpf.Ui.Controls.Button;

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

        public void LoadData() {
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
    }
}
