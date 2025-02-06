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

namespace Cluster
{
    public class Computer
    {
        public Computer()
        {
        }

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


        public bool HasEnoughRam(int ram)
        {
            return ram <= RamCapacity - MemoryUsage;
        }

        public bool HasEnoughCore(int cores)
        {
            return cores <= ProcessorCore - ProcessorUsage;
        }

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
                computers.Add(new Computer
                {
                    Name = item.Split('\\').Last(),
                    ProcessorCore = Convert.ToInt32(data.Split(';')[0]),
                    RamCapacity = Convert.ToInt32(data.Split(";")[1]),
                    processes = Directory.GetFiles(item).Where(x => !x.EndsWith(".szamitogep_konfig")).Select(x => new Process(x)).ToList()

                });
            }
            return computers;
        }

        public static string? AddComputer(string Path, string name, int cores, int ram, List<string>? computerNames = null)
        {
            if (computerNames == null)
                computerNames = GetComputers(Path).Select(x => x.Name).ToList();
            if (computerNames!.Contains(name))
            {
                return "A computer already uses this name";
            }
            if (cores < 1 || ram < 1)
            {
                return "The amount of cpu cores and memory must be positive.";
            }

            string dir = Directory.CreateDirectory($@"{Path}\{name}").FullName;
            File.WriteAllLines($@"{dir}\.szamitogep_konfig", [cores.ToString(), ram.ToString()]);

            return null;
        }

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

        public string? OutSourcePrograms()
        {
            if (CanOutSourcePrograms())
            {
                MessageBox mgbox = new()
                {
                    Title = "Error",
                    Content = "Deletion failed as this computer is running programs, but they can be outsourced to other machines. Would you like to proceed?",
                    IsPrimaryButtonEnabled = true,
                    IsSecondaryButtonEnabled = false,
                    //Background = new SolidColorBrush(Color.FromRgb(244, 66, 54)),
                    PrimaryButtonText = "Yes",
                    CloseButtonText = "Cancel"

                };
                MessageBoxResult result = mgbox.ShowDialogAsync().GetAwaiter().GetResult();
                if (result == MessageBoxResult.Primary)
                {
                    int processesCount = processes.Count;
                    bool isSuccess = OutSource() == null;
                    if (!isSuccess)
                    {
                        return "Outsourcing failed! Please try again later.";
                    }
                    Log.WriteLog([Name, $"{processesCount}"], LogType.ClearProgramInstances);
                    return null;

                }
                return string.Empty;
            }
            return "Outsourcing isn't possible. Shut down all the programs before deleting the computer!";
        }

        private string? OutSource(string? path = null)
        {
            path = path ?? MainWindow.ClusterPath;
            List<Computer> computers = GetComputers(path)
                .Where(x => x.Name != Name).ToList();
            foreach (var process in processes)
            {
                Computer? capable = computers.FirstOrDefault(x => x.HasEnoughCore(process.ProcessorUsage) && x.HasEnoughRam(process.MemoryUsage));
                if (capable == null)
                {
                    return "There's not enough resource on other computers to outsource.";
                }

                try
                {
                    //File.Move(Path.Combine(path, Name, process.FileName), Path.Combine(path, capable.Name, process.FileName));
                    MoveProcess(process.FileName, this, capable);
                }
                catch (Exception ex)
                {
                    throw ex;
                    return ex.Message;
                }

                capable.processes.Add(process);
                computers = computers.Where(x => x.Name != capable.Name).ToList().Append(capable).ToList();
            }
            return null;
        }

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

        public static string? OptimizeComputers(int min, int max)
        {
            List<Computer> sortedComputers = GetComputers(MainWindow.ClusterPath)
                .OrderBy(x => x.ProcessorCore + x.RamCapacity).ToList();

            List<Process> allActiveProcesses = sortedComputers
                .SelectMany(x => x.processes).Where(x => x.Active)
                .OrderBy(x => x.ProcessorUsage + x.MemoryUsage).ToList();

            sortedComputers.ForEach(x => x.processes.RemoveAll(x => x.Active));


            return null;
        }

        public static string? SpreadProcesses(int movingRangePercent = 5)
        {
            List<Computer> computers = GetComputers(MainWindow.ClusterPath);

            List<Process> allActiveProcesses = computers
                .SelectMany(x => x.processes).Where(x => x.Active)
                .OrderByDescending(x => x.ProcessorUsage + x.MemoryUsage).ToList();

            computers.ForEach(x => x.processes.RemoveAll(x => x.Active));

            double equalSpreadPercentValue = allActiveProcesses.Sum(x => x.ProcessorUsage + x.MemoryUsage) * 100.0 / computers.Sum(x => x.ProcessorCore + x.RamCapacity);

            int equalSpreadPercent = Convert.ToInt32(Math.Round(equalSpreadPercentValue));

            foreach (Computer pc in computers)
            {
                int consumableResources = Convert.ToInt32((pc.RamCapacity + pc.ProcessorCore) * equalSpreadPercent / 100.0);
                int movingRange = Convert.ToInt32((pc.RamCapacity + pc.ProcessorCore) * movingRangePercent / 100.0);

                while (pc.ProcessorUsage + pc.MemoryUsage < consumableResources)
                {
                    Process? processToAdd = allActiveProcesses.FirstOrDefault(x => 
                    ((pc.ProcessorUsage + pc.MemoryUsage + x.MemoryUsage + x.ProcessorUsage) > consumableResources - movingRange && 
                    (pc.ProcessorUsage + pc.MemoryUsage + x.MemoryUsage + x.ProcessorUsage) < consumableResources + movingRange) || ((pc.ProcessorUsage + pc.MemoryUsage + x.MemoryUsage + x.ProcessorUsage) < consumableResources - movingRange));
                    if (processToAdd == null) break;
                    pc.processes.Add(processToAdd);
                    allActiveProcesses.Remove(processToAdd);


                    //remainingResources =
                    //(pc.RamCapacity * Convert.ToInt32(equalSpreadPercent) - pc.MemoryUsage) +
                    //(pc.ProcessorCore * Convert.ToInt32(equalSpreadPercent) - pc.ProcessorUsage);
                }
                computers.Find(x => x.Name == pc.Name)!.processes.AddRange(pc.processes);
            }

            ArrangeFiles(computers);

            return null;
        }

        private static void ArrangeFiles(List<Computer> computers)
        {
            List<Computer> oldComputers = GetComputers(MainWindow.ClusterPath);
            bool areSame = computers.Select(x => x.Name).OrderBy(x => x).SequenceEqual(oldComputers.Select(x => x.Name).OrderBy(x => x));
            if (!areSame) throw new InvalidDataException("The computers are not the same as the old ones!");

            foreach (Computer computer in computers)
            {
                foreach (Process process in computer.processes)
                {
                    MoveProcess(process.FileName, process.HostComputer, computer);
                }
            }
        }

        public static string? SpreadProcesses2(int movingRangePercent = 5)
        {
            List<Computer> computers = GetComputers(MainWindow.ClusterPath);

            List<Process> ramBasedActiveProcesses = computers
                .SelectMany(x => x.processes).Where(x => x.Active && x.MemoryUsage > x.ProcessorUsage)
                .OrderByDescending(x => x.MemoryUsage).ToList();

            List<Process> cpuBasedActiveProcesses = computers
                .SelectMany(x => x.processes).Where(x => x.Active && x.MemoryUsage < x.ProcessorUsage)
                .OrderByDescending(x => x.ProcessorUsage).ToList();

            computers.ForEach(x => x.processes.RemoveAll(x => x.Active));

            double equalSpreadPercentValue = allActiveProcesses.Sum(x => x.ProcessorUsage + x.MemoryUsage) * 100.0 / computers.Sum(x => x.ProcessorCore + x.RamCapacity);

            int equalSpreadPercent = Convert.ToInt32(Math.Round(equalSpreadPercentValue));

            foreach (Computer pc in computers)
            {
                int consumableResources = Convert.ToInt32((pc.RamCapacity + pc.ProcessorCore) * equalSpreadPercent / 100.0);
                int movingRange = Convert.ToInt32((pc.RamCapacity + pc.ProcessorCore) * movingRangePercent / 100.0);

                while (pc.ProcessorUsage + pc.MemoryUsage < consumableResources)
                {
                    Process? processToAdd = allActiveProcesses.FirstOrDefault(x =>
                    ((pc.ProcessorUsage + pc.MemoryUsage + x.MemoryUsage + x.ProcessorUsage) > consumableResources - movingRange &&
                    (pc.ProcessorUsage + pc.MemoryUsage + x.MemoryUsage + x.ProcessorUsage) < consumableResources + movingRange) || ((pc.ProcessorUsage + pc.MemoryUsage + x.MemoryUsage + x.ProcessorUsage) < consumableResources - movingRange));
                    if (processToAdd == null)
                        break;
                    pc.processes.Add(processToAdd);
                    allActiveProcesses.Remove(processToAdd);


                    //remainingResources =
                    //(pc.RamCapacity * Convert.ToInt32(equalSpreadPercent) - pc.MemoryUsage) +
                    //(pc.ProcessorCore * Convert.ToInt32(equalSpreadPercent) - pc.ProcessorUsage);
                }
                computers.Find(x => x.Name == pc.Name)!.processes.AddRange(pc.processes);
            }

            ArrangeFiles(computers);

            return null;
        }
    }
}
