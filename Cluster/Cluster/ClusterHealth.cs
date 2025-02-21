using System.IO;
using Wpf.Ui.Controls;

namespace Cluster
{
    public class ClusterHealth
    {
        public List<string> Errors { get; }
        public bool Ok => Errors.Count == 0;

        /// <summary>
        /// ClusterHealth constructor
        /// </summary>
        /// <param name="computers">List of computers</param>
        /// <param name="programs">List of programs</param>
        public ClusterHealth(List<Computer> computers, List<ProgramType> programs)
        {
            Errors = [];
            List<Process> processes = computers.Aggregate(new List<Process>(),
                (list, computer) => list.Concat(computer.processes).ToList());

            foreach (ProgramType p in programs)
            {
                int active = processes.Count(x => x.ProgramName == p.ProgramName && x.Active);
                int inactive = processes.Count(x => x.ProgramName == p.ProgramName && !x.Active);

                // 1. 
                if (active < p.ActivePrograms)
                {
                    Errors.Add(TranslationSource.Instance.WithParam("Health.NotEnough", p.ProgramName,
                        p.ActivePrograms.ToString(), active.ToString(), inactive.ToString()));
                }

                //2. 
                if ((active + inactive) > p.ActivePrograms)
                {
                    Errors.Add(TranslationSource.Instance.WithParam("Health.TooMany", p.ProgramName,
                        p.ActivePrograms.ToString(), active.ToString(), inactive.ToString()));
                }
            }

            // 3.
            foreach (Computer c in computers)
            {
                int processorUsage = c.processes.Where(x => x.Active).Sum(x => x.ProcessorUsage);
                int memoryUsage = c.processes.Where(x => x.Active).Sum(x => x.MemoryUsage);

                if (processorUsage > c.ProcessorCore)
                {
                    Errors.Add(TranslationSource.Instance.WithParam("Health.Processor", c.Name,
                        processorUsage.ToString(), c.ProcessorCore.ToString()));
                }

                if (memoryUsage > c.RamCapacity)
                {
                    Errors.Add(TranslationSource.Instance.WithParam("Health.Memory", c.Name, memoryUsage.ToString(),
                        c.RamCapacity.ToString()));
                }
            }
        }

        /// <summary>
        /// Fix the issues in the cluster
        /// </summary>
        /// <returns>The number of changed processes</returns>
        public static int FixIssues()
        {
            List<Computer> computers = Computer.GetComputers(MainWindow.ClusterPath);
            List<ProgramType> programs = ProgramType.ReadClusterFile(MainWindow.ClusterPath);

            List<Process> processes = computers.Aggregate(new List<Process>(),
                (list, computer) => [.. list, .. computer.processes]);

            //Create a dictionary to store the missing processes, where the key is the program and the value is the number of missing processes
            //If the value is positive, then there are missing processes, if the value is negative, then there are too many processes
            Dictionary<ProgramType, int> missingProcesses = new();

            //Collect data about missing processes in a dictionary
            foreach (ProgramType p in programs)
            {
                int active = processes.Count(x => x.ProgramName == p.ProgramName && x.Active);
                int all = processes.Count(x => x.ProgramName == p.ProgramName);
                if (p.ActivePrograms - active != 0)
                {
                    missingProcesses.Add(p, p.ActivePrograms - active);
                }
                else if (all - p.ActivePrograms > 0)
                {
                    missingProcesses.Add(p, p.ActivePrograms - all);
                }
            }

            bool issuesFixable = true;
            foreach (KeyValuePair<ProgramType, int> missingProcess in missingProcesses)
            {
                ProgramType program = missingProcess.Key;
                int missingProcessNumber = missingProcess.Value;

                if (missingProcessNumber > 0)
                {
                    //If there are proper processes that are inactive, then activate them, if they can be activated
                    List<Process> inactiveProcesses = processes.Where(x =>
                            x.ProgramName == program.ProgramName && !x.Active &&
                            x.HostComputer.HasEnoughRam(x.MemoryUsage) &&
                            x.HostComputer.HasEnoughCore(x.ProcessorUsage))
                        .ToList();

                    foreach (Process process in inactiveProcesses)
                    {
                        int computerIndex = computers.FindIndex(x => x.Name == process.HostComputer.Name);
                        int processIndex = computers[computerIndex].processes
                            .FindIndex(x => x.FileName == process.FileName);

                        if (computers[computerIndex].HasEnoughCore(process.ProcessorUsage) &&
                            computers[computerIndex].HasEnoughRam(process.MemoryUsage))
                        {
                            computers[computerIndex].processes[processIndex].Active = true;
                            missingProcessNumber--;
                        }

                        if (missingProcessNumber == 0)
                        {
                            break;
                        }
                    }

                    //If there are still missing processes, then create new ones
                    for (int i = 0; i < missingProcessNumber; i++)
                    {
                        Computer? computer = computers.FirstOrDefault(x =>
                            x.HasEnoughRam(program.Memory) && x.HasEnoughCore(program.CpuMilliCore));
                        if (computer == null)
                        {
                            issuesFixable = false;
                            continue;
                        }

                        Process process = new(program.ProgramName, program.CpuMilliCore, program.Memory, true)
                        {
                            HostComputer = computer
                        };
                        computers[computers.FindIndex(x => x.Name == computer.Name)].processes.Add(process);
                    }
                }
                else
                {
                    //If there are too many processes, then delete them
                    List<Process> overflowingPrograms = [.. processes.Where(x => x.ProgramName == program.ProgramName)];
                    for (int i = 0; i < Math.Abs(missingProcessNumber); i++)
                    {
                        Process process = overflowingPrograms[i];
                        int computerIndex = computers.FindIndex(x => x.Name == process.HostComputer.Name);
                        int processIndex = computers[computerIndex].processes
                            .FindIndex(x => x.FileName == process.FileName);
                        computers[computerIndex].processes.RemoveAt(processIndex);
                    }
                }
            }

            if (!issuesFixable)
            {
                //Show a message box that the issues are not fixable and if the user wants to continue anyway
                MessageBox msgBox = new()
                {
                    Title = TranslationSource.T("HealthPage.NotFixable.Title"),
                    Content = TranslationSource.T("HealthPage.NotFixable.Text"),
                    IsPrimaryButtonEnabled = true,
                    IsSecondaryButtonEnabled = false,
                    PrimaryButtonText = TranslationSource.T("Continue"),
                    CloseButtonText = TranslationSource.T("Cancel"),
                    Width = 500,
                    MaxWidth = 500,
                    MaxHeight = 1000
                };
                MessageBoxResult result = msgBox.ShowDialogAsync().GetAwaiter().GetResult();

                // Hit cancel
                if (result != MessageBoxResult.Primary)
                    return -1;
            }

            //Save the changes
            //1. Update the active status of the processes
            List<Process> originalProcesses =
                Computer.GetComputers(MainWindow.ClusterPath).SelectMany(x => x.processes).ToList();
            List<Process> activeChangeProcesses = computers.SelectMany(x => x.processes)
                .Where(x => originalProcesses.Any(y => x.FileName == y.FileName && x.Active != y.Active)).ToList();
            //System.Windows.MessageBox.Show(activeChangeProcesses.Count.ToString());
            foreach (Process process in activeChangeProcesses)
            {
                process.Active = !process.Active;
                process.ToggleActive();
            }

            //2. Add new processes
            List<Process> newProcesses = computers.SelectMany(x => x.processes)
                .Where(x => originalProcesses.All(y => x.FileName != y.FileName)).ToList();
            foreach (Process process in newProcesses)
            {
                process.Write(Path.Combine(MainWindow.ClusterPath, process.HostComputer.Name));
                Log.WriteLog(
                [
                    $"{process.FileName}", $"{process.StartTime:yyyy.MM.dd. HH:mm:ss}", $"{process.Active}",
                    $"{process.ProcessorUsage}", $"{process.MemoryUsage}", process.HostComputer.Name
                ], LogType.RunProgramInstance);
            }

            //3. Remove processes
            List<Process> removedProcesses = originalProcesses
                .Where(x => computers.SelectMany(y => y.processes).All(z => z.FileName != x.FileName)).ToList();
            foreach (Process process in removedProcesses)
            {
                process.Shutdown();
            }

            return activeChangeProcesses.Count + newProcesses.Count + removedProcesses.Count;
        }
    }
}