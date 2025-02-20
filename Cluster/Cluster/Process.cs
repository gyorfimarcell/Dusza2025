using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Xml.Linq;
using Path = System.IO.Path;

namespace Cluster
{
    public class Process
    {
        public string ProgramName { get; set; }
        public string ProcessId { get; set; }
        public DateTime StartTime { get; set; }
        public bool Active { get; set; }
        public int ProcessorUsage { get; set; }
        public int MemoryUsage { get; set; }
        
        Computer? _hostComputer = null;

        public Computer HostComputer
        {
            get =>
                _hostComputer ?? (_hostComputer = Computer.GetComputers(MainWindow.ClusterPath)
                    .Find(x => x.processes.Any(x => x.FileName == this.FileName)));
            set => _hostComputer = value;
        }

        public Process(string path)
        {
            string filename = Path.GetFileName(path);
            this.ProgramName = filename.Split('-')[0];
            this.ProcessId = filename.Split("-")[1];

            string[] lines = File.ReadAllLines(path);
            this.StartTime = DateTime.Parse(lines[0]);
            this.Active = lines[1] == "AKTÍV";
            this.ProcessorUsage = Convert.ToInt32(lines[2]);
            this.MemoryUsage = Convert.ToInt32(lines[3]);
        }

        public Process(string programName, int processorUsage, int MemoryUsage, bool active) {
            this.ProgramName=programName;
            this.ProcessId = GenerateId();
            this.StartTime = DateTime.Now;
            this.Active = active;
            this.ProcessorUsage = processorUsage;
            this.MemoryUsage = MemoryUsage;
        }


        public string FileName => $"{ProgramName}-{ProcessId}";
        public string FileContent
        {
            get
            {
                List<string> lines = new();
                lines.Add($"{StartTime.Year}-{StartTime.Month.ToString().PadLeft(2, '0')}-{StartTime.Day.ToString().PadLeft(2, '0')} {StartTime.Hour.ToString().PadLeft(2, '0')}:{StartTime.Minute.ToString().PadLeft(2, '0')}:{StartTime.Second.ToString().PadLeft(2, '0')}");
                lines.Add(Active ? "AKTÍV" : "INAKTÍV");
                lines.Add(ProcessorUsage.ToString());
                lines.Add(MemoryUsage.ToString());
                return String.Join("\n", lines);
            }
        }

        public string GetCSVRow() {
            string status = Active ? "Active" : "Inactive";
            return $"{FileName};{HostComputer.Name};{status};{ProcessorUsage};{MemoryUsage}";
    }

        public void Write(string folder) {
            File.WriteAllText(Path.Combine(folder, FileName), FileContent);
        }

        public static string GenerateId()
        {
            Random r = new();

            string id = "";

            for (int i = 0; i < 6; i++)
            {
                int num = r.Next(0, 36);
                if (num < 10)
                {
                    id += Convert.ToChar(48 + num);
                }
                else
                {
                    id += Convert.ToChar(97 - 10 + num);
                }
            }

            return id;
        }

        public void Shutdown() {
            File.Delete($@"{MainWindow.ClusterPath}\{HostComputer.Name}\{FileName}");
            Log.WriteLog([$"{FileName}", $"{StartTime:yyyy.MM.dd. HH:mm:ss}", $"{Active}", $"{ProcessorUsage}", $"{MemoryUsage}", HostComputer.Name], LogType.ShutdownProgramInstance);
        }

        public bool ToggleActive() {
            if (Active == false)
            {
                Computer host = HostComputer;

                if (host.ProcessorUsage + ProcessorUsage > host.ProcessorCore ||
                    host.MemoryUsage + MemoryUsage > host.RamCapacity)
                {
                    return false;
                }
            }
            Active = !Active;
            Write($@"{MainWindow.ClusterPath}\{HostComputer.Name}");
            Log.WriteLog([$"{FileName}", HostComputer.Name, $"{Active}", $"{ProcessorUsage}", $"{MemoryUsage}"], Active ? LogType.ActivateProgramInstance : LogType.DeactivateProgramInstance);
            return true;
        }
    }
}
