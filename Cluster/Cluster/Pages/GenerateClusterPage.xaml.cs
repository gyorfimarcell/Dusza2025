using Microsoft.Win32;
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
using System.IO;
using Path = System.IO.Path;
using Directory = System.IO.Directory;
using System.Security.Cryptography;

namespace Cluster
{
    /// <summary>
    /// Interaction logic for GenerateClusterPage.xaml
    /// </summary>
    public partial class GenerateClusterPage : CustomPage
    {
        string chosenPath;
        MainWindow _window;
        public GenerateClusterPage()
        {
            _window = (MainWindow)Application.Current.MainWindow!;
            InitializeComponent();
        }

        private void btnGenerate_Click(object sender, RoutedEventArgs e)
        {
            if (nbComputer.Value == null || nbProgram.Value == null || nbProcess.Value == null || chosenPath == null)
            {
                _window.RootSnackbarService.Show(
                        "Error",
                        "You must fill out all fields!",
                        ControlAppearance.Danger,
                        new SymbolIcon { Symbol = SymbolRegular.Warning24 },
                        TimeSpan.FromSeconds(3)
                    );
                return;
            }

            string rootPath = chosenPath + "\\cluster-" + Process.GenerateId();
            Directory.CreateDirectory(rootPath);
            Random rnd = new();
            List<string> clusterLines = new();
            Dictionary<string, int> programInstances = new();

            int totalProcesses = Convert.ToInt32(nbProcess.Value);
            int numPrograms = Convert.ToInt32(nbProgram.Value);
            int baseProcessCount = totalProcesses / numPrograms;
            int remainingProcesses = totalProcesses % numPrograms;

            for (int i = 0; i < numPrograms; i++)
            {
                int extraProcess = (remainingProcesses > 0) ? 1 : 0;
                remainingProcesses -= extraProcess;
                int assignedProcesses = baseProcessCount + extraProcess;

                clusterLines.AddRange(new List<string>() {
                    $"program{i + 1}",
                    assignedProcesses.ToString(),
                    (rnd.Next(10, 50) * 10).ToString(),
                    (rnd.Next(10, 50) * 10).ToString()
                });

                programInstances[$"program{i + 1}"] = assignedProcesses;
            }

            File.WriteAllLines(rootPath + "\\.klaszter", clusterLines);
            List<Dictionary<string, int>> computerResources = new();
            for (int i = 0; i < nbComputer.Value; i++)
            {
                string computerDirectory = $"{rootPath}\\szamitogep{i + 1}";
                Directory.CreateDirectory(computerDirectory);

                int randomCpu = rnd.Next(100, 1000) * 10;
                int randomMemory = rnd.Next(100, 1000) * 10;

                File.WriteAllLines(computerDirectory + "\\.szamitogep_konfig",
                    new List<string> { randomCpu.ToString(), randomMemory.ToString() });

                computerResources.Add(new Dictionary<string, int>
                {
                    { "CPU", randomCpu },
                    { "Memory", randomMemory }
                });
            }

            List<ProgramType> programs = ProgramType.ReadClusterFile(rootPath);

            int computerIndex = 0;
            foreach (var programEntry in programInstances)
            {
                string programName = programEntry.Key;
                int instances = programEntry.Value;

                for (int i = 0; i < instances; i++)
                {
                    ProgramType program = programs.Find(p => p.ProgramName == programName);
                    if (program == null) continue;

                    bool assigned = false;
                    bool isInactive = false;
                    while (!assigned)
                    {
                        if (computerResources[computerIndex]["CPU"] >= program.CpuMilliCore &&
                            computerResources[computerIndex]["Memory"] >= program.Memory)
                        {
                            string computerDir = $"{rootPath}\\szamitogep{computerIndex + 1}";
                            string filePath = $"{computerDir}\\{programName}-{Process.GenerateId()}";
                            File.WriteAllLines(filePath, new List<string>
                            {
                                new DateTime(2020, 1, 1).AddDays(rnd.Next(0, (int)(DateTime.Now - new DateTime(2020, 1, 1)).TotalDays)).ToString(),
                                "AKTÍV",
                                program.CpuMilliCore.ToString(),
                                program.Memory.ToString()
                            });

                            computerResources[computerIndex]["CPU"] -= program.CpuMilliCore;
                            computerResources[computerIndex]["Memory"] -= program.Memory;

                            assigned = true;
                        }
                        else
                        {
                            computerIndex = (computerIndex + 1) % Convert.ToInt32(nbComputer.Value);

                            if (computerIndex == 0 && !computerResources.Any(c => c["CPU"] >= program.CpuMilliCore && c["Memory"] >= program.Memory))
                            {
                                isInactive = true;
                                assigned = true;
                            }
                        }
                    }

                    if (isInactive)
                    {
                        string computerDir = $"{rootPath}\\szamitogep{computerIndex + 1}";
                        string filePath = $"{computerDir}\\{programName}-{Process.GenerateId()}";
                        File.WriteAllLines(filePath, new List<string>
                        {
                            new DateTime(2020, 1, 1).AddDays(rnd.Next(0, (int)(DateTime.Now - new DateTime(2020, 1, 1)).TotalDays)).ToString(),
                            "INAKTÍV",
                            program.CpuMilliCore.ToString(),
                            program.Memory.ToString()
                        });
                    }
                }
            }

            MainWindow.ClusterPath = rootPath;
            _window.RefreshLblPath();
            Log.WriteLog([rootPath], LogType.LoadCluster);
            _window.EnableNavigationItems();
            _window.RootNavigation.Navigate(typeof(ClusterHealthPage));
        }


        private void btnChoosePath_Click(object sender, RoutedEventArgs e)
        {
            OpenFolderDialog ofd = new();
            if (ofd.ShowDialog() == true)
            {
                chosenPath = ofd.FolderName;
                PathCard.Description = chosenPath;
            }
        }
    }
}
