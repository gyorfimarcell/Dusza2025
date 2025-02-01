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

namespace Cluster
{
    /// <summary>
    /// Interaction logic for AddNewProgramPage.xaml
    /// </summary>
    public partial class AddNewProgramPage : Page
    {
        string path;
        public AddNewProgramPage()
        {
            InitializeComponent();

            path = MainWindow.ClusterPath;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (txtProgramName.Text.Length < 1 || !int.TryParse(txtActivePrograms.Text, out _) || !int.TryParse(txtCpuMilliCore.Text, out _) || !int.TryParse(txtMemory.Text, out _))
            {
                MessageBox.Show("Please fill out all the fields!");
                return;
            }

            ProgramType program = new(txtProgramName.Text, int.Parse(txtActivePrograms.Text), int.Parse(txtCpuMilliCore.Text), int.Parse(txtMemory.Text));

            if (ProgramType.ReadClusterFile(path).Select(x => x.ProgramName).Contains(program.ProgramName))
            {
                MessageBox.Show("This program already exists in the cluster!");
                return;
            }

            program.AddNewProgramToCluster(path);
            //TODO: Run the program on computers

            txtProgramName.Text = txtProgramName.Text = txtActivePrograms.Text = txtCpuMilliCore.Text = txtMemory.Text = "";
            txtProgramName.Focus();

        }
    }
}
