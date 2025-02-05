using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cluster
{
    public class ProgramType
    {
        public ProgramType(string programName, int activePrograms, int cpuMilliCore, int memory)
        {
            ProgramName = programName;
            ActivePrograms = activePrograms;
            CpuMilliCore = cpuMilliCore;
            Memory = memory;
        }

        public ProgramType() { }

        public string ProgramName { get; set; }
        public int ActivePrograms { get; set; }
        public int CpuMilliCore { get; set; }
        public int Memory { get; set; }

        public static List<ProgramType> ReadClusterFile(string path)
        {
            string[] files = Directory.GetFiles(path);
            List<ProgramType> programs = new();
            if (!files.Select(x => Path.GetFileName(x)).Contains(".klaszter"))
            {
                return null;
            }
            StreamReader sr = new(path + "\\.klaszter");
            List<string> currentValues = new();
            while (!sr.EndOfStream)
            {
                currentValues.Add(sr.ReadLine());
                if (currentValues.Count() == 4)
                {
                    ProgramType newProgram = new(currentValues[0], int.Parse(currentValues[1]), int.Parse(currentValues[2]), int.Parse(currentValues[3]));
                    programs.Add(newProgram);
                    currentValues.Clear();
                }
            }
            sr.Close();

            return programs;
        }

        public void AddNewProgramToCluster(string path)
        {
            string fileContent = ClusterFileLines(this);
            File.AppendAllText(path + "/.klaszter", fileContent);
        }

        public static string ClusterFileLines(ProgramType program)
        {
            return $"{program.ProgramName}\n{program.ActivePrograms}\n{program.CpuMilliCore}\n{program.Memory}\n";
        }

        public static bool ShutdownProgram(string path, List<ProgramType> programs, string programName)
        {
            if(!programs.Select(x => x.ProgramName).Contains(programName)) { return false; }
            List<ProgramType> newPrograms = programs.Where(x => x.ProgramName != programName).ToList();
            string fileContent = "";
            foreach (var program in newPrograms)
            {
                fileContent += ClusterFileLines(program);
            }
            File.WriteAllText(path + "/.klaszter", fileContent);
            foreach (var directory in Directory.GetDirectories(path)) 
            {
                string[] directoryFiles = Directory.GetFiles(directory);
                foreach (var file in directoryFiles)
                {
                    if (Path.GetFileName(file).Contains(programName))
                    {
                        File.Delete(file);
                    }
                }
            }
            return true;
        }

        public static bool ModifyProgram(ProgramType program, int activePrograms, int processor, int memory)
        {
            List<ProgramType> programs = ReadClusterFile(MainWindow.ClusterPath);

            int index = programs.FindIndex(x => x.ProgramName == program.ProgramName);
            if (index == -1) { return false; }

            programs[index].ActivePrograms = activePrograms;
            programs[index].CpuMilliCore = processor;
            programs[index].Memory = memory;
            string fileContent = "";
            foreach (var p in programs)
            {
                fileContent += ClusterFileLines(p);
            }
            File.WriteAllText(MainWindow.ClusterPath + "/.klaszter", fileContent);
            return true;
        }
    }
}
