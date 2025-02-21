using System.IO;
using Path = System.IO.Path;

namespace Cluster
{
    public class Process
    {
        public string ProgramName { get; }
        public string ProcessId { get; }
        public DateTime StartTime { get; }
        public bool Active { get; set; }
        public int ProcessorUsage { get; }
        public int MemoryUsage { get; }

        private Computer? _hostComputer;

        public Computer HostComputer
        {
            get =>
                (_hostComputer ??= Computer.GetComputers(MainWindow.ClusterPath)
                    .Find(x => x.processes.Any(y => y.FileName == this.FileName)))!;
            init => _hostComputer = value;
        }

        public Process(string path)
        {
            string filename = Path.GetFileName(path);
            ProgramName = filename.Split('-')[0];
            ProcessId = filename.Split("-")[1];

            string[] lines = File.ReadAllLines(path);
            StartTime = DateTime.Parse(lines[0]);
            Active = lines[1] == "AKTÍV";
            ProcessorUsage = Convert.ToInt32(lines[2]);
            MemoryUsage = Convert.ToInt32(lines[3]);
        }

        public Process(string programName, int processorUsage, int MemoryUsage, bool active)
        {
            ProgramName = programName;
            ProcessId = GenerateId();
            StartTime = DateTime.Now;
            Active = active;
            ProcessorUsage = processorUsage;
            this.MemoryUsage = MemoryUsage;
        }

        public string FileName => $"{ProgramName}-{ProcessId}";

        private string FileContent
        {
            get
            {
                List<string> lines =
                [
                    $"{StartTime.Year}-{StartTime.Month.ToString().PadLeft(2, '0')}-{StartTime.Day.ToString().PadLeft(2, '0')} {StartTime.Hour.ToString().PadLeft(2, '0')}:{StartTime.Minute.ToString().PadLeft(2, '0')}:{StartTime.Second.ToString().PadLeft(2, '0')}",
                    Active ? "AKTÍV" : "INAKTÍV", ProcessorUsage.ToString(), MemoryUsage.ToString()
                ];
                return string.Join("\n", lines);
            }
        }

        /// <summary>
        /// Returns a CSV row of the process
        /// </summary>
        /// <returns>CSV row</returns>
        public string GetCSVRow()
        {
            string status = Active ? "Active" : "Inactive";
            return $"{FileName};{HostComputer.Name};{status};{ProcessorUsage};{MemoryUsage}";
        }

        /// <summary>
        /// Writes the process to a file
        /// </summary>
        /// <param name="folder">The path of the computer folder</param>
        public void Write(string folder)
        {
            File.WriteAllText(Path.Combine(folder, FileName), FileContent);
        }

        /// <summary>
        /// Generates a random 6 character long id for the process
        /// </summary>
        /// <returns>The generated id</returns>
        public static string GenerateId()
        {
            Random r = new();

            var id = "";

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

        /// <summary>
        /// Shuts down the process
        /// </summary>
        public string? Shutdown() {
            try
            {
                File.Delete($@"{MainWindow.ClusterPath}\{HostComputer.Name}\{FileName}");
                Log.WriteLog([$"{FileName}", $"{StartTime:yyyy.MM.dd. HH:mm:ss}", $"{Active}", $"{ProcessorUsage}", $"{MemoryUsage}", HostComputer.Name], LogType.ShutdownProgramInstance);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return null;
        }

        /// <summary>
        /// Toggles the active state of the process
        /// </summary>
        public bool ToggleActive()
        {
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
            Log.WriteLog([$"{FileName}", HostComputer.Name, $"{Active}", $"{ProcessorUsage}", $"{MemoryUsage}"],
                Active ? LogType.ActivateProgramInstance : LogType.DeactivateProgramInstance);
            return true;
        }
    }
}