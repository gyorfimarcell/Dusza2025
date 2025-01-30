using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cluster
{
    public class ClusterHealth
    {
        public List<string> Errors { get; set; }
        public bool Ok => Errors.Count == 0;

        public ClusterHealth(List<Computer> computers, List<ProgramType> programs)
        {
            Errors = new();
            List<Process> processes = computers.Aggregate(new List<Process>(), (list, computer) => list.Concat(computer.processes).ToList());

            foreach (ProgramType p in programs)
            {
                int active = processes.Count(x => x.ProgramName == p.ProgramName && x.Active);
                int inactive = processes.Count(x => x.ProgramName == p.ProgramName && !x.Active);

                // 1. 
                if (active < p.ActivePrograms) {
                    Errors.Add($"{p.ProgramName} doesn't have enough processes ({p.ActivePrograms} wanted, {active} active, {inactive} inactive)");
                }

                //2. 
                if ((active + inactive) > p.ActivePrograms) {
                    Errors.Add($"{p.ProgramName} has too many processes ({p.ActivePrograms} wanted, {active} active, {inactive} inactive)");
                }
            }

            // 3.
            foreach (Computer c in computers)
            {
                int processorUsage = c.processes.Where(x => x.Active).Sum(x => x.ProcessorUsage);
                int memoryUsage = c.processes.Where(x => x.Active).Sum(x => x.MemoryUsage);

                if (processorUsage > c.ProcessorCore) {
                    Errors.Add($"{c.Name} doesn't have enough processor capacity ({processorUsage}/{c.ProcessorCore})");
                }
                if (memoryUsage > c.RamCapacity)
                {
                    Errors.Add($"{c.Name} doesn't have enough memory capacity ({memoryUsage}/{c.RamCapacity})");
                }
            }
        }
    }
}
