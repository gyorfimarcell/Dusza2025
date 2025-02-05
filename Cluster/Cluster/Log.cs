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
        OptimizeProgramInstances
    }

    public class Log
    {

        public Dictionary<LogType, List<string>> LogDataTypes = new()
        {
            { LogType.OpenProgram, new(){} },
            { LogType.CloseProgram, new(){} },
            { LogType.LoadCluster, new(){"Path"} },
            { LogType.AddComputer, new(){"Name", "CPU", "Memory"} },
            { LogType.DeleteComputer, new(){"Name", "CPU", "Memory"} },
            { LogType.ExportCSV, new(){"Type"} },
            { LogType.AddProgram, new(){"Name", "CPU", "Memory", "Active programs"} },
            { LogType.RunProgramInstance, new(){"Filename", "Start", "Active", "CPU", "Memory"} },
            { LogType.ShutdownProgramInstance, new(){"Filename", "Start", "Active", "CPU", "Memory"} },
            { LogType.ModifyProgram, new(){"Name", "CPU", "Memory", "Active programs"} },
            { LogType.ShutdownProgram, new(){"Name", "CPU", "Memory"}},
            { LogType.ClearProgramInstances, new(){} },
            { LogType.OptimizeProgramInstances, new(){} },
        };

        public static string GetLogDirectoryPath()
        {
            string logDirectoryPath = Path.Combine(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory)?.Parent?.Parent?.Parent?.FullName, "Logs");
            if (!Directory.Exists(logDirectoryPath))
            {
                Directory.CreateDirectory(logDirectoryPath);
            }
            return logDirectoryPath;
        }
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
