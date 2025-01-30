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
    /// Interaction logic for ModifyProgram.xaml
    /// </summary>
    public partial class ModifyProgram : Window
    {
        List<ProgramType> programs = new List<ProgramType>();
        string path = string.Empty;
        public ModifyProgram(List<ProgramType> programs, string path)
        {
            InitializeComponent();
            this.programs = programs;
            this.path = path;
            lbCurrentPrograms.ItemsSource = programs.Select(x => x.ProgramName).ToList();
        }

        private void lbCurrentPrograms_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnModify.Visibility = lbCurrentPrograms.SelectedItem != null ? Visibility.Visible : Visibility.Hidden;
        }

        private void btnModify_Click(object sender, RoutedEventArgs e)
        {
            stEdit.Visibility = Visibility.Visible;
            ProgramType foundProgram = programs.First(x => x.ProgramName == lbCurrentPrograms.SelectedItem.ToString());
            txtProgram.Text = foundProgram.ProgramName;
            txtActivePrograms.Text = foundProgram.ActivePrograms.ToString();
            txtCpuMilliCore.Text = foundProgram.CpuMilliCore.ToString();
            txtMemory.Text = foundProgram.Memory.ToString();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            ProgramType foundProgram = programs.First(x => x.ProgramName == lbCurrentPrograms.SelectedItem.ToString());
            if(!int.TryParse(txtActivePrograms.Text, out int result1) || !int.TryParse(txtCpuMilliCore.Text, out int result2) || !int.TryParse(txtMemory.Text, out int result3))
            {
                MessageBox.Show("Please give numbers to all fields!");
                return;
            } else if(result1 < 0 || result2 < 0 || result3 < 0)
            {
                MessageBox.Show("Please give numbers greater than 0!");
                return;
            }
            List<string> inputValues = new List<string>() { txtActivePrograms.Text, txtCpuMilliCore.Text, txtMemory.Text };
            bool result = ProgramType.ModifyProgram(path, programs, foundProgram.ProgramName, inputValues);
            if (result)
            {
                programs = ProgramType.ReadClusterFile(path);
                lbCurrentPrograms.ItemsSource = programs.Select(x => x.ProgramName).ToList();
                stEdit.Visibility = Visibility.Hidden;
                MessageBox.Show("Program has been successfully modified!");
            }
        }
    }
}
