using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;
using Path = System.IO.Path;

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
            if (processes.Count > 0)
            {
                return "Shut down all the programs before deleting the computer!";
            }
            Directory.Delete($@"{MainWindow.ClusterPath}\{Name}", true);
            Log.WriteLog([Name, $"{ProcessorCore}", $"{RamCapacity}"], LogType.DeleteComputer);
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

        public bool OutSourcePrograms(string? path = null)
        {
            path = path ?? MainWindow.ClusterPath;
            List<Computer> computers = GetComputers(path)
                .Where(x => x.Name != Name).ToList();
            foreach (var process in processes)
            {
                Computer? capable = computers.FirstOrDefault(x => x.HasEnoughCore(process.ProcessorUsage) && x.HasEnoughRam(process.MemoryUsage));
                if (capable == null)
                {
                    return false;
                }

                try
                {
                    File.Move(Path.Combine(path, Name, process.FileName), Path.Combine(path, capable.Name, process.FileName));
                }
                catch (Exception ex)
                {
                    throw ex;
                    return false;
                }

                capable.processes.Add(process);
                computers = computers.Where(x => x.Name != capable.Name).ToList().Append(capable).ToList();
            }
            return true;
        }
    }
}
