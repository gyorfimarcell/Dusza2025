﻿using System.IO;
using System.Windows;
using Microsoft.Win32;
using Wpf.Ui.Controls;
using Directory = System.IO.Directory;

namespace Cluster
{
    /// <summary>
    /// Interaction logic for GenerateClusterPage.xaml
    /// </summary>
    public partial class GenerateClusterPage
    {
        private string chosenPath = null!;

        public GenerateClusterPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Generates a cluster based on the user input
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGenerate_Click(object sender, RoutedEventArgs e)
        {
            if (nbComputer.Value == null || nbProgram.Value == null || nbProcess.Value == null || chosenPath == null)
            {
                _window.RootSnackbarService.Show(
                    TranslationSource.T("Errors.Error"),
                    TranslationSource.T("Errors.MissingFields"),
                    ControlAppearance.Danger,
                    new SymbolIcon { Symbol = SymbolRegular.Warning24 },
                    TimeSpan.FromSeconds(10)
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

            if ((bool)rbConsistent.IsChecked!)
            {
                int baseProcessCount = totalProcesses / numPrograms;
                int remainingProcesses = totalProcesses % numPrograms;
                for (int i = 0; i < numPrograms; i++)
                {
                    int extraProcess = (remainingProcesses > 0) ? 1 : 0;
                    remainingProcesses -= extraProcess;
                    int assignedProcesses = baseProcessCount + extraProcess;

                    clusterLines.AddRange(new List<string>()
                    {
                        $"program{i + 1}",
                        assignedProcesses.ToString(),
                        (rnd.Next(10, 50) * 10).ToString(),
                        (rnd.Next(10, 50) * 10).ToString()
                    });

                    programInstances[$"program{i + 1}"] = assignedProcesses;
                }
            }
            else
            {
                int remainingProcesses = totalProcesses;
                for (int i = 0; i < numPrograms; i++)
                {
                    int assignedProcesses;

                    if (i == numPrograms - 1)
                    {
                        assignedProcesses = remainingProcesses;
                    }
                    else if (remainingProcesses <= numPrograms - i)
                    {
                        assignedProcesses = 1;
                    }
                    else
                    {
                        assignedProcesses = rnd.Next(1, remainingProcesses - (numPrograms - i - 1));
                    }

                    clusterLines.AddRange(new List<string>()
                    {
                        $"program{i + 1}",
                        assignedProcesses.ToString(),
                        (rnd.Next(10, 50) * 10).ToString(),
                        (rnd.Next(10, 50) * 10).ToString()
                    });

                    programInstances[$"program{i + 1}"] = assignedProcesses;
                    remainingProcesses -= assignedProcesses;
                }
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
            foreach (KeyValuePair<string, int> programEntry in programInstances)
            {
                string programName = programEntry.Key;
                int instances = programEntry.Value;

                for (int i = 0; i < instances; i++)
                {
                    ProgramType program = programs.Find(p => p.ProgramName == programName)!;
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
                                new DateTime(2020, 1, 1)
                                    .AddDays(rnd.Next(0, (int)(DateTime.Now - new DateTime(2020, 1, 1)).TotalDays))
                                    .AddHours(rnd.Next(0, 24))
                                    .AddMinutes(rnd.Next(0, 60))
                                    .AddSeconds(rnd.Next(0, 60))
                                    .ToString("yyyy-MM-dd HH:mm:ss"),
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

                            if (computerIndex == 0 && !computerResources.Any(c =>
                                    c["CPU"] >= program.CpuMilliCore && c["Memory"] >= program.Memory))
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
                            new DateTime(2020, 1, 1)
                            .AddDays(rnd.Next(0, (int)(DateTime.Now - new DateTime(2020, 1, 1)).TotalDays))
                            .AddHours(rnd.Next(0, 24))
                            .AddMinutes(rnd.Next(0, 60))
                            .AddSeconds(rnd.Next(0, 60))
                            .ToString("yyyy-MM-dd HH:mm:ss"),
                            "INAKTÍV",
                            program.CpuMilliCore.ToString(),
                            program.Memory.ToString()
                        });
                    }
                }
            }

            Log.WriteLog(
                [rootPath, nbComputer.Value.ToString()!, nbProgram.Value.ToString()!, nbProcess.Value.ToString()!],
                LogType.GenerateCluster);

            MainWindow.ClusterPath = rootPath;
            _window.RefreshLblPath();
            Log.WriteLog([rootPath], LogType.LoadCluster);
            _window.EnableNavigationItems();
            _window.RootNavigation.Navigate(typeof(ClusterHealthPage));
        }

        /// <summary>
        /// Opens a dialog to choose the path for the cluster
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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