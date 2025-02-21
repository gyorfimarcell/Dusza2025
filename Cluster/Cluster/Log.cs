using System.IO;
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

    public static class Log
    {
        /// <summary>
        /// Fields of the logs
        /// </summary>
        public static readonly Dictionary<LogType, List<string>> LogDataTypes = new()
        {
            { LogType.OpenProgram, [] },
            { LogType.CloseProgram, [] },
            { LogType.LoadCluster, ["Path"] },
            { LogType.AddComputer, ["Name", "CPU", "Memory"] },
            { LogType.DeleteComputer, ["Name", "CPU", "Memory"] },
            { LogType.ExportCSV, ["Type", "Path"] },
            { LogType.AddProgram, ["Name", "CPU", "Memory", "Active programs"] },
            { LogType.RunProgramInstance, ["Instance", "Start", "Active", "CPU", "Memory", "Computer"] },
            { LogType.ShutdownProgramInstance, ["Instance", "Start", "Active", "CPU", "Memory", "Computer"] },
            { LogType.ModifyProgram, ["Name", "CPU", "Memory", "Active programs"] },
            { LogType.ShutdownProgram, ["Name", "CPU", "Memory", "Instances"] },
            { LogType.ClearProgramInstances, ["Computer", "Process count"] },
            { LogType.MoveProgramInstance, ["Instance", "Source computer", "Destination computer"] },
            { LogType.OptimizeProgramInstances, ["Minimum Percent", "Maximum Percent", "Computers"] },
            { LogType.ModifyComputer, ["Name", "CPU Capacity", "Memory Capacity"] },
            { LogType.SpreadProgramInstances, ["CPU Spread Percent", "RAM Spread Percent", "Computers", "Processes"] },
            { LogType.FixIssues, ["Fixed Processes"] },
            { LogType.ActivateProgramInstance, ["Instance", "Start", "Active", "Processor Usage", "Memory Usage"] },
            {
                LogType.DeactivateProgramInstance,
                ["Instance", "Host Computer", "Active", "Processor Usage", "Memory Usage"]
            },
            { LogType.GenerateCluster, ["Path", "Computers", "Programtypes", "Processes"] }
        };

        /// <summary>
        /// Returns the path of the log directory
        /// </summary>
        /// <returns>The path of the directory</returns>
        public static string GetLogDirectoryPath()
        {
            string logDirectoryPath = "Logs";
            if (!Directory.Exists(logDirectoryPath))
            {
                Directory.CreateDirectory(logDirectoryPath);
            }

            return logDirectoryPath;
        }

        /// <summary>
        /// Writes a log entry to the log file
        /// </summary>
        /// <param name="logData">The data which should be written into the log field</param>
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
                using StreamWriter sw = new(logFilePath, File.Exists(logFilePath));
                sw.WriteLine(logEntry);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error writing to log file: {ex.Message}");
            }
        }
    }
}