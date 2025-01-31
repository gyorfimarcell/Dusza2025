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
    /// Interaction logic for AddComputerWindow.xaml
    /// </summary>
    public partial class AddComputerPage : Page
    {
        string path;

        public AddComputerPage()
        {
            InitializeComponent();
            path = MainWindow.path;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            int processor, memory;

            if (tbName.Text == "" || tbMemory.Text == "" || tbProcessor.Text == "") {
                MessageBox.Show("You must fill out all fields");
                return;
            }
            if (!Int32.TryParse(tbProcessor.Text, out processor))
            {
                MessageBox.Show("Processor capacity must be a number!");
                return;
            }
            if (!Int32.TryParse(tbMemory.Text, out memory))
            {
                MessageBox.Show("Memory capacity must be a number!");
                return;
            }

            bool success = Computer.AddComputer(path, tbName.Text, processor, memory);
            if (success) {
                //Close();
            }
        }
    }
}
