using System.IO;

namespace Cluster
{
    public class ProgramType(string programName, int activePrograms, int cpuMilliCore, int memory)
    {
        public string ProgramName { get; } = programName;
        public int ActivePrograms { get; private set; } = activePrograms;
        public int CpuMilliCore { get; private set; } = cpuMilliCore;
        public int Memory { get; private set; } = memory;

        /// <summary>
        /// Reads the .klaszter file in the given path and returns the programs in a list
        /// </summary>
        /// <param name="path">The path of the chosen cluster</param>
        /// <returns>List of program types</returns>
        public static List<ProgramType> ReadClusterFile(string path)
        {
            try
            {
                string[] files = Directory.GetFiles(path);
                List<ProgramType> programs = [];
                if (!files.Select(Path.GetFileName).Contains(".klaszter"))
                {
                    return null!;
                }

                StreamReader sr = new(path + "\\.klaszter");
                List<string> currentValues = [];
                while (!sr.EndOfStream)
                {
                    currentValues.Add(sr.ReadLine()!);
                    if (currentValues.Count == 4)
                    {
                        ProgramType newProgram = new(currentValues[0], int.Parse(currentValues[1]),
                            int.Parse(currentValues[2]), int.Parse(currentValues[3]));
                        programs.Add(newProgram);
                        currentValues.Clear();
                    }
                }

                sr.Close();

                return programs;
            }
            catch (Exception)
            {
                return null!;
            }
        }

        /// <summary>
        /// Adds a new program to the cluster
        /// </summary>
        /// <param name="path">The path of the cluster</param>
        public void AddNewProgramToCluster(string path)
        {
            string fileContent = ClusterFileLines(this);
            File.AppendAllText(path + "/.klaszter", fileContent);
        }

        /// <summary>
        /// Returns the program in a string format
        /// </summary>
        /// <param name="program">Chosen program</param>
        /// <returns>The program in a string format</returns>
        private static string ClusterFileLines(ProgramType program)
        {
            return $"{program.ProgramName}\n{program.ActivePrograms}\n{program.CpuMilliCore}\n{program.Memory}\n";
        }

        /// <summary>
        /// Shuts down a program
        /// </summary>
        /// <param name="program">Chosen program</param>
        /// <returns>Success or not</returns>
        public static bool ShutdownProgram(ProgramType program)
        {
            List<ProgramType> programs = ReadClusterFile(MainWindow.ClusterPath);
            int index = programs.FindIndex(x => x.ProgramName == program.ProgramName);
            if (index == -1)
            {
                return false;
            }

            programs.RemoveAt(index);
            string fileContent = programs.Aggregate("", (current, p) => current + ClusterFileLines(p));
            File.WriteAllText(MainWindow.ClusterPath + "/.klaszter", fileContent);
            foreach (string directory in Directory.GetDirectories(MainWindow.ClusterPath))
            {
                string[] directoryFiles = Directory.GetFiles(directory);
                foreach (string file in directoryFiles)
                {
                    if (Path.GetFileName(file).Contains(program.ProgramName))
                    {
                        File.Delete(file);
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Modifies a program
        /// </summary>
        /// <param name="program">Chosen program</param>
        /// <param name="activePrograms">Number of active programs</param>
        /// <param name="processor">Processor capacity</param>
        /// <param name="memory">RAM capacity</param>
        /// <returns>Success or not</returns>
        public static bool ModifyProgram(ProgramType program, int activePrograms, int processor, int memory)
        {
            List<ProgramType> programs = ReadClusterFile(MainWindow.ClusterPath);

            int index = programs.FindIndex(x => x.ProgramName == program.ProgramName);
            if (index == -1)
            {
                return false;
            }

            programs[index].ActivePrograms = activePrograms;
            programs[index].CpuMilliCore = processor;
            programs[index].Memory = memory;
            string fileContent = programs.Aggregate("", (current, p) => current + ClusterFileLines(p));
            File.WriteAllText(MainWindow.ClusterPath + "/.klaszter", fileContent);
            return true;
        }
    }
}