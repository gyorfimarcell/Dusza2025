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

namespace Cluster
{
    /// <summary>
    /// Interaction logic for DeleteComputerWindow.xaml
    /// </summary>
    public partial class DeleteComputerWindow : Window
    {
        string path;

        public DeleteComputerWindow(string path)
        {
            InitializeComponent();

            this.path = path;

            btnDelete.IsEnabled = false;

            lbComputers.ItemsSource = Computer.GetComputers(path).Select(x => x.Name);
            lbComputers.SelectedIndex = 0;
        }

        private void lbComputers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbComputers.SelectedIndex == -1)
            {
                return;
            }

            Computer c = Computer.GetComputers(path).Find(x => x.Name == lbComputers.SelectedItem.ToString());
            dgProcesses.ItemsSource = c.processes.Select(x => new ToDeleteProcessRow(x));

            btnDelete.IsEnabled = c.processes.Count == 0;
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            Computer c = Computer.GetComputers(path).Find(x => x.Name == lbComputers.SelectedItem.ToString());
            bool success = c.Delete(path);
            if (success) {
                Close();
            }
        }
    }

    public class ToDeleteProcessRow {
        Process process;

        public string Name => process.FileName;
        public DateTime StartTime => process.StartTime;
        public string Status => process.Active ? "Active" : "Inactive";


        public ToDeleteProcessRow(Process process)
        {
            this.process = process;
        }
    }
}
