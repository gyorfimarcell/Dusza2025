using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;
using Wpf.Ui.Controls;
using Path = System.IO.Path;
using MessageBox = Wpf.Ui.Controls.MessageBox;
using MessageBoxResult = Wpf.Ui.Controls.MessageBoxResult;
using System.Numerics;
using File = System.IO.File;
using System.Xml.Linq;
using System.Windows.Documents;
using Cluster.Controls;

namespace Cluster
{
    public class Computer
    {
        public Computer()
        {
        }

        /// <summary>
        ///    Creates a new computer with the given parameters.
        /// </summary>
        /// <param name="name">Name of the computer</param>
        /// <param name="processorCore">Amount of processor core</param>
        /// <param name="ramCapacity">Amount of RAM capacity</param>
        public Computer(string name, int processorCore, int ramCapacity)
        {
            Name = name;
            ProcessorCore = processorCore;
            RamCapacity = ramCapacity;
        }

        public string Name { get; set; }
        public int ProcessorCore { get; set; }
        public int ProcessorUsage => processes.Where(x => x.Active).Sum(x => x.ProcessorUsage);

        public int RamCapacity { get; set; }
        public int MemoryUsage => processes.Where(x => x.Active).Sum(x => x.MemoryUsage);

        public List<Process> processes { get; set; }

        public string CsvRow => $"{Name};{ProcessorCore};{ProcessorUsage};{RamCapacity};{MemoryUsage}";


        /// <summary>
        ///   Checks if the computer has enough RAM to run a program.
        /// </summary>
        /// <param name="ram">Amount of RAM</param>
        /// <returns>Has the computer enough ram to run a program</returns>
        public bool HasEnoughRam(int ram)
        {
            return ram <= RamCapacity - MemoryUsage;
        }

        /// <summary>
        ///  Checks if the computer has enough core to run a program.
        /// </summary>
        /// <param name="cores">Amount of processor cores</param>
        /// <returns>Has the computer enough cores to run a program</returns>
        public bool HasEnoughCore(int cores)
        {
            return cores <= ProcessorCore - ProcessorUsage;
        }

        /// <summary>
        ///   Gets the computers from the given cluster path.
        /// </summary>
        /// <param name="Path">Path of the cluster</param>
        /// <returns>List of computers in the cluster</returns>
        public static List<Computer> GetComputers(string Path)
        {
            List<Computer> computers = new List<Computer>();

            foreach (var item in Directory.GetDirectories(Path))
            {
                if (!Directory.GetFiles(item).Select(x => x.Split("\\").Last()).Contains(".szamitogep_konfig"))
                {
                    //MessageBox.Show($@"The '{item.Split("\\").Last()}' folder doesn't have the required '.szamitogep-konfig' file, so it doesn't count as a computer :(");
                    continue;
                }
                string data = string.Join(';', File.ReadAllLines(@$"{item}\.szamitogep_konfig"));
                Computer newComputer = new Computer
                {
                    Name = item.Split('\\').Last(),
                    ProcessorCore = Convert.ToInt32(data.Split(';')[0]),
                    RamCapacity = Convert.ToInt32(data.Split(";")[1]),
                };
                newComputer.processes = Directory.GetFiles(item)
                    .Where(x => !x.EndsWith(".szamitogep_konfig"))
                    .Select(x => new Process(x) { HostComputer = newComputer })
                    .ToList();
                computers.Add(newComputer);
            }
            return computers;
        }

        /// <summary>
        ///   Adds a new computer to the cluster.
        /// </summary>
        /// <param name="Path">The path of the cluster</param>
        /// <param name="name">The name of the computer</param>
        /// <param name="cores">Processor cores of the computer</param>
        /// <param name="ram">RAM of the computer</param>
        /// <param name="computerNames">Already given computer names to check if the name is already exists</param>
        /// <returns>If there is an error message it returns it otherwise returns null.</returns>
        public static string? AddComputer(string Path, string name, int cores, int ram, List<string>? computerNames = null)
        {
            if (computerNames == null)
                computerNames = GetComputers(Path).Select(x => x.Name).ToList();
            if (computerNames!.Contains(name))
            {
                return TranslationSource.T("Errors.ComputerNameTaken");
            }
            if (cores < 1 || ram < 1)
            {
                return TranslationSource.T("Errors.ComputerPositive");
            }

            string dir = Directory.CreateDirectory($@"{Path}\{name}").FullName;
            File.WriteAllLines($@"{dir}\.szamitogep_konfig", [cores.ToString(), ram.ToString()]);

            return null;
        }

        /// <summary>
        ///   Deletes the computer from the cluster.
        /// </summary>
        /// <returns>If there is an error message it returns it otherwise returns null.</returns>
        public string? Delete()
        {
            try
            {
                Directory.Delete($@"{MainWindow.ClusterPath}\{Name}", true);
                Log.WriteLog([Name, $"{ProcessorCore}", $"{RamCapacity}"], LogType.DeleteComputer);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return null;
        }

        public string? Modify(int processor, int memory)
        {
            if (processor < ProcessorUsage)
            {
                return TranslationSource.T("Errors.ComputerMoreProcessor");
            }
            if (memory < MemoryUsage)
            {
                return TranslationSource.T("Errors.ComputerMoreMemory");
            }

            File.WriteAllLines($@"{MainWindow.ClusterPath}\{Name}\.szamitogep_konfig", [processor.ToString(), memory.ToString()]);
            return null;
        }

        /// <summary>
        ///  Checks if the programs can be outsourced to other computers.
        /// </summary>
        /// <param name="path">Path of the clusters</param>
        /// <returns>Is it possible to outsource the programs or not.</returns>
        public bool CanOutSourcePrograms(string? path = null)
        {
            List<Computer> computers = GetComputers(path ?? MainWindow.ClusterPath)
                .Where(x => x.Name != Name).ToList();
            foreach (var program in processes)
            {
                Computer? capable = computers.FirstOrDefault(x => x.HasEnoughCore(program.ProcessorUsage) && x.HasEnoughRam(program.MemoryUsage));
                if (capable == null)
                {
                    return false;
                }
                capable.processes.Add(program);
                computers = computers.Where(x => x.Name != capable.Name).ToList().Append(capable).ToList();
            }
            return true;
        }

        /// <summary>
        ///   Giving messagebox to the user to decide whether outsource the programs to other computers.
        /// </summary>
        /// <returns>If there is an error message it returns it otherwise returns null.</returns>
        public List<string>? OutSourcePrograms()
        {
            MessageBox mgbox = new()
            {
                Title = TranslationSource.T("Errors.Error"),
                Content = TranslationSource.T("Computer.Delete.Text"),
                IsPrimaryButtonEnabled = true,
                IsSecondaryButtonEnabled = true,
                //Background = new SolidColorBrush(Color.FromRgb(244, 66, 54)),
                PrimaryButtonText = TranslationSource.T("Computer.Delete"),
                SecondaryButtonText = TranslationSource.T("Computer.Delete.Outsource"),
                CloseButtonText = TranslationSource.T("Cancel")

            };

            MessageBoxResult result = mgbox.ShowDialogAsync().GetAwaiter().GetResult();

            if (result != MessageBoxResult.Primary && result != MessageBoxResult.Secondary)
                return null;

            bool deleteAsWell = result == MessageBoxResult.Primary;

            bool canOutsource = CanOutSourcePrograms();
            if (!canOutsource)
            {
                //return "Outsourcing isn't possible. Shut down all the programs before deleting the computer!";
                deleteAsWell = false;
                mgbox = new()
                {
                    Title = TranslationSource.T("Errors.Error"),
                    Content = TranslationSource.T("Errors.OutsourceNotEnoughSpace"),
                    IsPrimaryButtonEnabled = true,
                    IsSecondaryButtonEnabled = false,
                    PrimaryButtonText = TranslationSource.T("Continue"),
                    CloseButtonText = TranslationSource.T("Cancel")

                };

                result = mgbox.ShowDialogAsync().GetAwaiter().GetResult();
                if (result != MessageBoxResult.Primary)
                {
                    return [TranslationSource.T("Errors.Outsource.Cancel"), "Info"];
                }
            }

            int processesCount = processes.Count;
            bool isSuccess = OutSource(forceOutSource: !canOutsource) == null;
            if (!isSuccess)
            {
                return [TranslationSource.T("Errors.Outsource.Fail"), "Danger"];
            }

            List<Process> remainingProcesses = GetComputers(MainWindow.ClusterPath).Find(x => x.Name == Name).processes;
            Log.WriteLog([Name, $"{processesCount - remainingProcesses.Count}"], LogType.ClearProgramInstances);

            if (deleteAsWell)
            {
                string? res = Delete();
                return res == null ? [TranslationSource.T("Outsourcing.DeleteSuccess"), "Success"] : [res, "Danger"];
            }

            if (!canOutsource)
            {
                return [TranslationSource.Instance.WithParam("Outsourcing.PartialSuccess", remainingProcesses.Count.ToString(), Name), "Info"];
            }

            return [TranslationSource.Instance.WithParam("Outsourcing.Success", Name), "Success"];
        }

        /// <summary>
        ///   Outsourcing the programs to other computers.
        /// </summary>
        /// <param name="path">Path of the cluster</param>
        /// <returns>If there is an error message it returns it otherwise returns null.</returns>
        private string? OutSource(string? path = null, bool forceOutSource = false)
        {
            path = path ?? MainWindow.ClusterPath;
            List<Computer> computers = GetComputers(path).Where(x => x.Name != Name)
                .OrderByDescending(x => (x.ProcessorCore - x.ProcessorUsage + (x.RamCapacity - x.MemoryUsage)) / 2.0).ToList();
            List<Process> orderedProcesses = processes.OrderBy(x => (x.ProcessorUsage + x.MemoryUsage) / 2.0).ToList();

            foreach (Process process in orderedProcesses)
            {
                Computer? capable = computers.FirstOrDefault(x => x.HasEnoughCore(process.ProcessorUsage) && x.HasEnoughRam(process.MemoryUsage));
                if (capable == null)
                {
                    if (forceOutSource)
                        continue;
                    return TranslationSource.T("Errors.OutsourcingOther");
                }

                try
                {
                    MoveProcess(process.FileName, this, capable);
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }

                capable.processes.Add(process);
                computers = computers.Where(x => x.Name != capable.Name).ToList().Append(capable).ToList();
            }
            return null;
        }

        /// <summary>
        ///    Moves a process from one computer to another.
        /// </summary>
        /// <param name="processFilename">The filename of the current process</param>
        /// <param name="sourceComputer">Source computer</param>
        /// <param name="destinationComputer">Destination computer</param>
        /// <param name="path">Path of the cluster</param>
        private static void MoveProcess(string processFilename, Computer sourceComputer, Computer destinationComputer, string? path = null)
        {
            path = path ?? MainWindow.ClusterPath;
            try
            {
                File.Move(Path.Combine(path, sourceComputer.Name, processFilename), Path.Combine(path, destinationComputer.Name, processFilename));
                Log.WriteLog([processFilename, sourceComputer.Name, destinationComputer.Name], LogType.MoveProgramInstance);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>
        ///    Checks if the computers can be optimized with the given values.
        /// </summary>
        /// <param name="min">Minimum resource usage in percentage</param>
        /// <param name="max">Maximum resource usage in percentage</param>
        /// <returns>If the computers can be optimized, then yes, otherwise false</returns>
        public static bool CanOptimizeComputers(int min, int max)
        {
            List<Computer> computers = GetComputers(MainWindow.ClusterPath);

            List<Process> allActiveProcesses = computers
                .SelectMany(x => x.processes).Where(x => x.Active)
                .ToList();

            computers.ForEach(x => x.processes.RemoveAll(x => x.Active));

            double sumProcesses = allActiveProcesses.Sum(x => x.ProcessorUsage + x.MemoryUsage);
            double sumComputers = computers.Sum(x => x.ProcessorCore + x.RamCapacity);

            bool canOptimize = sumProcesses * 100.0 / sumComputers >= min &&
                               sumProcesses * 100.0 / sumComputers <= max;

            //System.Windows.MessageBox.Show($"{canOptimize}\n" +
            //    $"Avg: {Math.Round(sumProcesses * 100.0 / sumComputers, 2)} %\n" +
            //    $"Proc: {sumProcesses} - Comp: {sumComputers}\n" +
            //    $"Min: {min} % - Max: {max} %");

            return canOptimize;
        }

        /// <summary>
        ///   Optimizes the computers in the cluster folder.
        /// </summary>
        /// <param name="min">Minimum running percent</param>
        /// <param name="max">Maximum running percent</param>
        /// <returns>Error mesaage if any error occures, otherwise false</returns>
        public static string? OptimizeComputers(int min, int max)
        {
            //Save the computers and their active processes in lists
            List<Computer> computers = GetComputers(MainWindow.ClusterPath);

            List<Process> allActiveProcesses = computers
                .SelectMany(x => x.processes).Where(x => x.Active).
                OrderByDescending(x => x.ProcessorUsage + x.MemoryUsage).ToList();

            //Check if there are active processes and computers
            if (allActiveProcesses.Count == 0 || computers.Count == 0)
            {
                return TranslationSource.T("Errors.OutsourcingNone");
            }

            //Separate the active processes into two lists based on their resource usage
            //One list for the ones that use more memory than cpu and the other for the opposite
            //Those, who use equal amount of resources remain in the original list
            List<Process> ramBasedActiveProcesses = allActiveProcesses
                .Where(x => x.MemoryUsage > x.ProcessorUsage)
                .OrderBy(x => x.MemoryUsage).ToList();

            List<Process> cpuBasedActiveProcesses = allActiveProcesses
                .Where(x => x.MemoryUsage < x.ProcessorUsage)
                .OrderByDescending(x => x.ProcessorUsage).ToList();

            //Create a third list for the processes that use equal amount of resources
            List<Process> equallyBasedActiveProcesses = allActiveProcesses
                .Where(x => x.MemoryUsage == x.ProcessorUsage)
                .OrderByDescending(x => x.MemoryUsage).ToList();

            //Remove the active processes from the computers
            computers.ForEach(x => x.processes.RemoveAll(x => x.Active));

            //Calculate the equal spread percentage both for memory and cpu
            int spreadMinPercent = min;
            int spreadMaxPercent = max;

            //When the while loop iterates, the computers that are full will be added to the blacklist
            List<string> blackListComputers = new List<string>();

            //Second round: Fill up the computers to their minimum with the active processes
            while (blackListComputers.Count < computers.Count)
            {
                List<Computer> capableComputers = computers.Where(x => !blackListComputers.Contains(x.Name)).ToList();
                foreach (Computer pc in capableComputers)
                {
                    int consumableRam = Convert.ToInt32(Math.Round(pc.RamCapacity * spreadMinPercent / 100.0));
                    int consumableCpu = Convert.ToInt32(Math.Round(pc.ProcessorCore * spreadMinPercent / 100.0));

                    if (pc.MemoryUsage > consumableRam || pc.ProcessorUsage > consumableCpu)
                    {
                        blackListComputers.Add(pc.Name);
                        continue;
                    }

                    bool isRamMoreUsed = consumableRam - pc.MemoryUsage < consumableCpu - pc.ProcessorUsage;

                    // ------------------------------- RAM/CPU BASED -------------------------------
                    Process? fitProcess = fitProcess = (isRamMoreUsed ? cpuBasedActiveProcesses : ramBasedActiveProcesses)
                        .Where(x => (pc.MemoryUsage + x.MemoryUsage) >= consumableRam &&
                        (pc.ProcessorUsage + x.ProcessorUsage) >= consumableCpu)?.MinBy(x => isRamMoreUsed ? x.ProcessorUsage : x.MemoryUsage)

                        ??

                        (isRamMoreUsed ? cpuBasedActiveProcesses : ramBasedActiveProcesses)
                        .MaxBy(x => isRamMoreUsed ? x.ProcessorUsage : x.MemoryUsage);

                    if (fitProcess != null)
                    {
                        computers.Find(x => x.Name == pc.Name).processes.Add(fitProcess);
                        (isRamMoreUsed ? cpuBasedActiveProcesses : ramBasedActiveProcesses).Remove(fitProcess);
                        continue;
                    }


                    // -------------------------------- BOTH BASED --------------------------------
                    fitProcess = equallyBasedActiveProcesses.Where(x =>
                    (pc.MemoryUsage + x.MemoryUsage) >= consumableRam &&
                    (pc.ProcessorUsage + x.ProcessorUsage) >= consumableCpu)?.MinBy(x => x.ProcessorUsage)

                    ??

                    equallyBasedActiveProcesses.MaxBy(x => x.ProcessorUsage);



                    if (fitProcess != null)
                    {
                        computers.Find(x => x.Name == pc.Name).processes.Add(fitProcess);
                        equallyBasedActiveProcesses.Remove(fitProcess);
                        continue;
                    }

                    // ------------------------------- CPU/RAM BASED -------------------------------
                    fitProcess = (isRamMoreUsed ? ramBasedActiveProcesses : cpuBasedActiveProcesses)
                    .Where(x => (pc.MemoryUsage + x.MemoryUsage) >= consumableRam &&
                    (pc.ProcessorUsage + x.ProcessorUsage) >= consumableCpu)?.MinBy(x => isRamMoreUsed ? x.ProcessorUsage : x.MemoryUsage)

                    ??

                    (isRamMoreUsed ? cpuBasedActiveProcesses : ramBasedActiveProcesses)
                    .MaxBy(x => isRamMoreUsed ? x.ProcessorUsage : x.MemoryUsage);

                    if (fitProcess != null)
                    {
                        computers.Find(x => x.Name == pc.Name).processes.Add(fitProcess);
                        (isRamMoreUsed ? ramBasedActiveProcesses : cpuBasedActiveProcesses).Remove(fitProcess);
                        continue;
                    }
                    else
                    {
                        blackListComputers.Add(pc.Name);
                    }
                }
            }

            List<Process> temp = [.. ramBasedActiveProcesses, .. cpuBasedActiveProcesses, .. equallyBasedActiveProcesses];
            //System.Windows.MessageBox.Show("Remaining Processes: " + temp.Count);

            //Third round: Fill up the computers to their maximum with the active processes
            for (int i = 0; i < computers.Count; i++)
            {
                Computer pc = computers[i];
                int consumableRam = Convert.ToInt32(Math.Round(pc.RamCapacity * spreadMaxPercent / 100.0));
                int consumableCpu = Convert.ToInt32(Math.Round(pc.ProcessorCore * spreadMaxPercent / 100.0));
                while (true)
                {
                    //System.Windows.MessageBox.Show("Before\n"+ $"{pc.Name}\n" + string.Join("\n", computers.Select(x => $"{x.Name}: {x.ProcessorCore - x.ProcessorUsage} - {x.RamCapacity - x.MemoryUsage}")));

                    bool isRamMoreUsed = consumableRam - pc.MemoryUsage < consumableCpu - pc.ProcessorUsage;

                    // ------------------------------- RAM/CPU BASED -------------------------------
                    Process? fitProcess = (isRamMoreUsed ? cpuBasedActiveProcesses : ramBasedActiveProcesses)
                        .Where(x => (pc.MemoryUsage + x.MemoryUsage) <= consumableRam &&
                        (pc.ProcessorUsage + x.ProcessorUsage) <= consumableCpu)?.MaxBy(x => isRamMoreUsed ? x.ProcessorUsage : x.MemoryUsage);

                    if (fitProcess != null)
                    {
                        computers[i].processes.Add(fitProcess);
                        (isRamMoreUsed ? cpuBasedActiveProcesses : ramBasedActiveProcesses).Remove(fitProcess);
                        continue;
                    }


                    // --------------------------------- BOTH BASED ---------------------------------
                    fitProcess = equallyBasedActiveProcesses.Where(x =>
                    (pc.MemoryUsage + x.MemoryUsage) <= consumableRam &&
                    (pc.ProcessorUsage + x.ProcessorUsage) <= consumableCpu)?.MaxBy(x => x.ProcessorUsage);


                    if (fitProcess != null)
                    {
                        computers[i].processes.Add(fitProcess);
                        equallyBasedActiveProcesses.Remove(fitProcess);
                        continue;
                    }

                    // ------------------------------- CPU/RAM BASED -------------------------------
                    fitProcess = (isRamMoreUsed ? ramBasedActiveProcesses : cpuBasedActiveProcesses).Where(x =>
                    (pc.MemoryUsage + x.MemoryUsage) <= consumableRam &&
                    (pc.ProcessorUsage + x.ProcessorUsage) <= consumableCpu)?.MaxBy(x => isRamMoreUsed ? x.ProcessorUsage : x.MemoryUsage);

                    if (fitProcess != null)
                    {
                        computers[i].processes.Add(fitProcess);
                        (isRamMoreUsed ? ramBasedActiveProcesses : cpuBasedActiveProcesses).Remove(fitProcess);
                        continue;
                    }
                    else
                    {
                        break;
                    }
                }
            }




            List<Process> remainingProcesses = [.. ramBasedActiveProcesses, .. cpuBasedActiveProcesses, .. equallyBasedActiveProcesses];

            if (remainingProcesses.Count > 0)
            {
                MessageBox mgbox = new()
                {
                    Title = TranslationSource.T("Errors.Error"),
                    Content = TranslationSource.T("Errors.OptimizeCannotFit"),
                    IsPrimaryButtonEnabled = true,
                    IsSecondaryButtonEnabled = false,
                    PrimaryButtonText = TranslationSource.T("Continue"),
                    CloseButtonText = TranslationSource.T("Cancel"),
                };
                MessageBoxResult result = mgbox.ShowDialogAsync().GetAwaiter().GetResult();
                if (result != MessageBoxResult.Primary)
                    return TranslationSource.T("Optimize.Cancel");
            }

            ArrangeFiles(computers);

            return null;
        }

        /// <summary>
        ///     Arranges the processes in the cluster folder according to the given computers' data.
        /// </summary>
        /// <param name="computers">The file structure will be based on the computerlist</param>
        /// <exception cref="InvalidDataException">Throws if the computers are not the same as in the existing file system</exception>

        private static void ArrangeFiles(List<Computer> computers)
        {
            List<Computer> oldComputers = GetComputers(MainWindow.ClusterPath);
            bool areSame = computers.Select(x => x.Name).OrderBy(x => x).SequenceEqual(oldComputers.Select(x => x.Name).OrderBy(x => x));
            if (!areSame)
                throw new InvalidDataException("The computers are not the same as the old ones!");

            foreach (Computer computer in computers)
            {
                foreach (Process process in computer.processes)
                {
                    MoveProcess(process.FileName, process.HostComputer, computer);
                }
            }
        }

        /// <summary>
        ///    Spreads the active processes equally among the computers.
        /// </summary>
        /// <param name="movingRangePercent">A range that specify the sensitivity range</param>
        /// <returns>Error message, otherwise null</returns>
        public static string? SpreadProcesses(int movingRangePercent = 5)
        {
            //Save the computers and their active processes in lists
            List<Computer> computers = GetComputers(MainWindow.ClusterPath);

            List<Process> allActiveProcesses = computers
                .SelectMany(x => x.processes).Where(x => x.Active).
                OrderByDescending(x => x.ProcessorUsage + x.MemoryUsage).ToList();

            //Check if there are active processes and computers
            if (allActiveProcesses.Count == 0 || computers.Count == 0)
            {
                return TranslationSource.T("Errors.SpreadNone");
            }

            //Separate the active processes into two lists based on their resource usage
            //One list for the ones that use more memory than cpu and the other for the opposite
            //Those, who use equal amount of resources remain in the original list
            List<Process> ramBasedActiveProcesses = allActiveProcesses
                .Where(x => x.MemoryUsage > x.ProcessorUsage)
                .OrderBy(x => x.MemoryUsage).ToList();

            List<Process> cpuBasedActiveProcesses = allActiveProcesses
                .Where(x => x.MemoryUsage < x.ProcessorUsage)
                .OrderByDescending(x => x.ProcessorUsage).ToList();

            //Create a third list for the processes that use equal amount of resources
            List<Process> equallyBasedActiveProcesses = allActiveProcesses
                .Where(x => x.MemoryUsage == x.ProcessorUsage)
                .OrderByDescending(x => x.MemoryUsage).ToList();

            //Remove the active processes from the computers
            computers.ForEach(x => x.processes.RemoveAll(x => x.Active));

            //Calculate the equal spread percentage both for memory and cpu
            int equalSpreadRamPercent = Convert.ToInt32(Math.Round(allActiveProcesses.Sum(x => x.MemoryUsage) * 100.0 / computers.Sum(x => x.RamCapacity)));
            int equalSpreadCpuPercent = Convert.ToInt32(Math.Round(allActiveProcesses.Sum(x => x.ProcessorUsage) * 100.0 / computers.Sum(x => x.ProcessorCore)));

            //When the while loop iterates, the computers that are full will be added to the blacklist
            List<string> blackListComputers = new List<string>();

            //Add the active processes to the computers until all of them are added
            while (blackListComputers.Count < computers.Count)
            {
                List<Computer> capableComputers = computers.Where(x => !blackListComputers.Contains(x.Name)).ToList();
                foreach (Computer pc in capableComputers)
                {
                    int consumableRam = Convert.ToInt32(Math.Round(pc.RamCapacity * equalSpreadRamPercent / 100.0));
                    int consumableCpu = Convert.ToInt32(Math.Round(pc.ProcessorCore * equalSpreadCpuPercent / 100.0));
                    int movingRangeRam = Convert.ToInt32(Math.Round(pc.RamCapacity * movingRangePercent / 100.0));
                    int movingRangeCpu = Convert.ToInt32(Math.Round(pc.ProcessorCore * movingRangePercent / 100.0));

                    if (pc.MemoryUsage > consumableRam - movingRangeRam || pc.ProcessorUsage > consumableCpu - movingRangeCpu)
                    {
                        blackListComputers.Add(pc.Name);
                        continue;
                    }

                    bool isRamMoreUsed = consumableRam - pc.MemoryUsage < consumableCpu - pc.ProcessorUsage;

                    // ------------------------------- RAM/CPU BASED -------------------------------
                    Process? fitProcess = (isRamMoreUsed ? cpuBasedActiveProcesses : ramBasedActiveProcesses)
                        .Where(x => (pc.MemoryUsage + x.MemoryUsage) <= consumableRam + movingRangeRam &&
                        (pc.ProcessorUsage + x.ProcessorUsage) <= consumableCpu + movingRangeCpu)?.MaxBy(x => isRamMoreUsed ? x.ProcessorUsage : x.MemoryUsage);

                    if (fitProcess != null)
                    {
                        computers.Find(x => x.Name == pc.Name).processes.Add(fitProcess);
                        (isRamMoreUsed ? cpuBasedActiveProcesses : ramBasedActiveProcesses).Remove(fitProcess);
                        continue;
                    }


                    // --------------------------------- BOTH BASED ---------------------------------
                    fitProcess = equallyBasedActiveProcesses.Where(x =>
                    (pc.MemoryUsage + x.MemoryUsage) <= consumableRam + movingRangeRam &&
                    (pc.ProcessorUsage + x.ProcessorUsage) <= consumableCpu + movingRangeCpu)?.MaxBy(x => x.ProcessorUsage);


                    if (fitProcess != null)
                    {
                        computers.Find(x => x.Name == pc.Name).processes.Add(fitProcess);
                        equallyBasedActiveProcesses.Remove(fitProcess);
                        continue;
                    }

                    // ------------------------------- CPU/RAM BASED -------------------------------
                    fitProcess = (isRamMoreUsed ? ramBasedActiveProcesses : cpuBasedActiveProcesses).Where(x =>
                    (pc.MemoryUsage + x.MemoryUsage) <= consumableRam + movingRangeRam &&
                    (pc.ProcessorUsage + x.ProcessorUsage) <= consumableCpu + movingRangeCpu)?.MaxBy(x => isRamMoreUsed ? x.ProcessorUsage : x.MemoryUsage);

                    if (fitProcess != null)
                    {
                        computers.Find(x => x.Name == pc.Name).processes.Add(fitProcess);
                        (isRamMoreUsed ? ramBasedActiveProcesses : cpuBasedActiveProcesses).Remove(fitProcess);
                        continue;
                    }
                    else
                    {
                        blackListComputers.Add(pc.Name);
                    }
                }
            }

            List<Process> remainingProcesses = ramBasedActiveProcesses.Concat(cpuBasedActiveProcesses).Concat(equallyBasedActiveProcesses).ToList().OrderByDescending(x => x.MemoryUsage + x.ProcessorUsage).ToList();


            for (int i = remainingProcesses.Count - 1; i >= 0; i--)
            {
                Process process = remainingProcesses[i];

                //Find the computer, that if we add a new process to it, the percent of the usage moves the least
                Computer? leastLoaded = computers
                    .Where(x => x.HasEnoughCore(process.ProcessorUsage) && x.HasEnoughRam(process.MemoryUsage))
                    .MinBy(x =>
                    {
                        int ramPercent = Convert.ToInt32(Math.Round((x.MemoryUsage + process.MemoryUsage) * 100.0 / x.RamCapacity));
                        int cpuPercent = Convert.ToInt32(Math.Round((x.ProcessorUsage + process.ProcessorUsage) * 100.0 / x.ProcessorCore));
                        return (cpuPercent + ramPercent) / 2.0;
                    })!;

                leastLoaded ??= computers
                    .Where(x => x.HasEnoughCore(process.ProcessorUsage) && x.HasEnoughRam(process.MemoryUsage))
                    .MaxBy(x => (x.ProcessorCore - x.ProcessorUsage) + (x.RamCapacity - x.MemoryUsage))!;

                if (leastLoaded == null)
                {
                    continue;
                }

                computers.Find(x => x.Name == leastLoaded.Name).processes.Add(process);
                remainingProcesses.RemoveAt(i);
                //System.Windows.MessageBox.Show($"{leastLoaded.Name}\n" + string.Join("\n", computers.Select(x => $"{x.Name}: {x.ProcessorCore - x.ProcessorUsage} - {x.RamCapacity - x.MemoryUsage}")));
            }


            if (remainingProcesses.Count > 0)
            {
                MessageBox mgbox = new()
                {
                    Title = TranslationSource.T("Errors.Error"),
                    Content = TranslationSource.T("Errors.OptimizeCannotFit"),
                    IsPrimaryButtonEnabled = true,
                    IsSecondaryButtonEnabled = false,
                    PrimaryButtonText = TranslationSource.T("Continue"),
                    CloseButtonText = TranslationSource.T("Cancel"),
                };
                MessageBoxResult result = mgbox.ShowDialogAsync().GetAwaiter().GetResult();
                if (result != MessageBoxResult.Primary)
                    return TranslationSource.T("Optimize.Cancel");
            }

            ArrangeFiles(computers);

            string processCount = allActiveProcesses.Count.ToString();
            Log.WriteLog([$"{equalSpreadCpuPercent}", $"{equalSpreadRamPercent}", computers.Count.ToString(), processCount.ToString()], LogType.SpreadProgramInstances);

            return null;
        }
    }

}