using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cluster
{
    public class Validate
    {

        /// <summary>
        /// Validates the computer object
        /// </summary>
        /// <param name="computer">Chosen computer</param>
        /// <returns>Valid or not</returns>
        public static bool ValidateComputer(Computer computer)
        {
            if (computer == null)
                return false;
            if (string.IsNullOrEmpty(computer.Name))
                return false;
            if (computer.ProcessorCore < 1)
                return false;
            if (computer.RamCapacity < 1)
                return false;
            if (computer.processes == null)
                return false;
            if (computer.processes.Count == 0)
                return false;
            return true;
        }

        /// <summary>
        /// Validates the process object
        /// </summary>
        /// <param name="process">Chosen process</param>
        /// <returns>Valid or not</returns>
        public static bool ValidateProcess(Process process)
        {
            if (process == null)
                return false;
            if (string.IsNullOrEmpty(process.ProgramName))
                return false;
            if (string.IsNullOrEmpty(process.ProcessId))
                return false;
            if (process.StartTime == null)
                return false;
            if (process.ProcessorUsage < 0)
                return false;
            if (process.MemoryUsage < 0)
                return false;
            return true;
        }

        /// <summary>
        /// Validates the program type object
        /// </summary>
        /// <param name="programType">Chosen program type</param>
        /// <returns>Valid or not</returns>
        public static bool ValidateProgramType(ProgramType programType)
        {
            if (programType == null)
                return false;
            if (string.IsNullOrEmpty(programType.ProgramName))
                return false;
            if (programType.ActivePrograms < 0)
                return false;
            if (programType.CpuMilliCore < 0)
                return false;
            if (programType.Memory < 0)
                return false;
            return true;
        }

        /// <summary>
        /// Validates the filename
        /// </summary>
        /// <param name="fileName">Chosen filename</param>
        /// <returns>Valid or not</returns>
        public static bool ValidateFileName(string fileName)
        {
            fileName = fileName.Trim();
            Path.GetInvalidFileNameChars().Select(x => x.ToString()).ToList()
                .ForEach(x => fileName = fileName.Replace(x, ""));

            new List<string> { "CON", "PRN", "AUX", "NUL", "COM1–COM9", "LPT1–LPT9" }
            .ForEach(x =>
            {
                fileName = fileName == x ? "" : fileName;
            });

            if (string.IsNullOrEmpty(fileName))
                return false;
            if (fileName.EndsWith('.'))
                return false;
            if (fileName.Length < 1)
                return false;
            return true;
        }

    }
}
