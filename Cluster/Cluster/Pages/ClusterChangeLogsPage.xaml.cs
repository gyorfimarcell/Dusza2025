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

namespace Cluster
{
    /// <summary>
    /// Interaction logic for ClusterChangeLogsPage.xaml
    /// </summary>
    public partial class ClusterChangeLogsPage : Page
    {
        public ClusterChangeLogsPage()
        {
            InitializeComponent();
            GenerateLogView();
        }

        private void GenerateLogView()
        {
            try
            {
                string directoryPath = Log.GetLogDirectoryPath();

                if (Directory.Exists(directoryPath))
                {
                    string[] files = Directory.GetFiles(directoryPath, "*.log");

                    trLogs.Items.Clear();

                    foreach (string file in files)
                    {
                        string fileName = System.IO.Path.GetFileName(file);

                        TreeViewItem trViewItem = new TreeViewItem
                        {
                            Header = fileName
                        };

                        trLogs.Items.Add(trViewItem);

                        string[] lines = File.ReadAllLines(file);

                        foreach(var line in lines)
                        {
                            TreeViewItem trViewSubitem = new TreeViewItem
                            {
                                Header = line
                            };
                            trViewItem.Items.Add(trViewSubitem);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while loading: " + ex.Message);
            }
        }

    }
}
