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
        AddComputer,
        DeleteComputer,
        ExportCSV,
        AddProgram,
        RunProgramInstance,
        ShutdownProgramInstance,
        ModifyProgram,
        ClearProramInstances,
        OptimizeProgramInstances
    }
    public static class Log
    {
        public static string GetLogDirectoryPath()
        {
            return Path.Combine(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory)?.Parent?.Parent?.Parent?.FullName, "Logs");
        }
        public static void WriteLog(List<string> logData, LogType type)
        {
            if (logData == null || logData.Count == 0) return;

            string date = DateTime.Now.ToString("yyyy.MM.dd. HH:mm:ss");
            string logFilePath = Path.Combine(GetLogDirectoryPath(), $"{date[..10].Replace(".", "")}.log");
            logData.InsertRange(0, [$"{type}", date]);
            string logEntry = string.Join(" - ", logData);

            try
            {
                MessageBox.Show(Path.GetDirectoryName(GetLogDirectoryPath()));
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
