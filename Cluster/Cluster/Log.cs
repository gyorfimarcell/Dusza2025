using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Cluster
{
    public enum LogType
    {
        OpenProgram,
        CloseProgram,
        LoadCluster,
        AddComputer,
        DeleteComputer,
        ExportCSV,
        AddProgram,
        RunProgramInstance,
        ShutdownProgramInstance,
        ModifyProgram,
        ShutdownProgram,
        ClearProgramInstances,
        MoveProgramInstance,
        OptimizeProgramInstances,
        ModifyComputer,
        SpreadProgramInstances,
        FixIssues,
        ActivateProgramInstance,
        DeactivateProgramInstance,
        GenerateCluster,
    }

    public class Log
    {

        /// <summary>
        /// Fields of the logs
        /// </summary>
        public static Dictionary<LogType, List<string>> LogDataTypes = new()
        {
            { LogType.OpenProgram, new(){} },
            { LogType.CloseProgram, new(){} },
            { LogType.LoadCluster, new(){"Path"} },
            { LogType.AddComputer, new(){"Name", "CPU", "Memory"} },
            { LogType.DeleteComputer, new(){"Name", "CPU", "Memory"} },
            { LogType.ExportCSV, new(){"Type", "Path"} },
            { LogType.AddProgram, new(){"Name", "CPU", "Memory", "Active programs"} },
            { LogType.RunProgramInstance, new(){ "Instance", "Start", "Active", "CPU", "Memory", "Computer" } },
            { LogType.ShutdownProgramInstance, new(){"Instance", "Start", "Active", "CPU", "Memory", "Computer"} },
            { LogType.ModifyProgram, new(){"Name", "CPU", "Memory", "Active programs"} },
            { LogType.ShutdownProgram, new(){"Name", "CPU", "Memory", "Instances"}},
            { LogType.ClearProgramInstances, new(){"Computer", "Process count"} },
            { LogType.MoveProgramInstance, new(){ "Instance", "Source computer", "Destination computer"} },
            { LogType.OptimizeProgramInstances, new(){ "Minimum Percent", "Maximum Percent", "Computers"} },
            { LogType.ModifyComputer, new(){"Name", "CPU Capacity", "Memory Capacity"} },
            { LogType.SpreadProgramInstances, new(){ "CPU Spread Percent", "RAM Spread Percent", "Computers", "Processes" } },
            { LogType.FixIssues, new(){ "Fixed Processes" } },
            { LogType.ActivateProgramInstance, new(){"Instance", "Start", "Active", "Processor Usage", "Memory Usage"} },
            { LogType.DeactivateProgramInstance, new(){ "Instance", "Host Computer", "Active", "Processor Usage", "Memory Usage"} },
            { LogType.GenerateCluster, new(){ "Path", "Computers", "Programtypes", "Processes" } }
        };

        /// <summary>
        /// Returns the path of the log directory
        /// </summary>
        /// <returns>The path of the directory</returns>
        public static string GetLogDirectoryPath()
        {
            string logDirectoryPath = Path.Combine(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory)?.Parent?.Parent?.Parent?.FullName, "Logs");
            if (!Directory.Exists(logDirectoryPath))
            {
                Directory.CreateDirectory(logDirectoryPath);
            }
            return logDirectoryPath;
        }

        /// <summary>
        /// Writes a log entry to the log file
        /// </summary>
        /// <param name="logData">The data which should be written into the log fiel</param>
        /// <param name="type">The type of the log</param>
        public static void WriteLog(List<string> logData, LogType type)
        {
            if (logData == null) return;

            string date = DateTime.Now.ToString("yyyy.MM.dd. HH:mm:ss");
            string logFilePath = Path.Combine(GetLogDirectoryPath(), $"{date[..10].Replace(".", "")}.log");
            logData.InsertRange(0, [$"{type}", date]);
            string logEntry = string.Join(" - ", logData);

            try
            {
                //MessageBox.Show(Path.GetDirectoryName(GetLogDirectoryPath()));
                using (StreamWriter sw = new(logFilePath, File.Exists(logFilePath)))
                {
                    sw.WriteLine(logEntry);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error writing to log file: {ex.Message}");
            }
        }
    }
}
