using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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
        public int RamCapacity { get; set; }
        public List<Process> processes { get; set; }


        public bool HasEnoughRam(int ram)
        {
            return ram <= RamCapacity;
        }

        public bool HasEnoughCore(int cores)
        {
            return cores <= ProcessorCore;
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

        public static bool AddComputer(string Path, string name, int cores, int ram, List<string>? computerNames = null)
        {
            if (computerNames == null) computerNames = GetComputers(Path).Select(x => x.Name).ToList();
            if (computerNames!.Contains(name))
            {
                MessageBox.Show("A computer already uses this name");
                return false;
            }
            if (cores < 1 || ram < 1)
            {
                MessageBox.Show("The amount of cpu cores and memory must be positive.");
                return false;
            }

            string dir = Directory.CreateDirectory($@"{Path}\{name}").FullName;
            File.WriteAllLines($@"{dir}\.szamitogep_konfig", [cores.ToString(), ram.ToString()]);

            return true;
        }

        public bool Delete(string Path)
        {
            if (processes.Count > 0)
            {
                MessageBox.Show("Shut down all the programs before deleting the computer!");
                return false;
            }
            Directory.Delete($@"{Path}\{Name}", true);
            return true;
        }
    }
}
