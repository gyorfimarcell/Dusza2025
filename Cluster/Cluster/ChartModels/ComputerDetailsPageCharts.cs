using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cluster.ChartModels
{
    class ComputerDetailsPageCharts
    {
        public IEnumerable<ISeries> ProgramsSeries { get; set; }
        public IEnumerable<ISeries> ProcessorSeries { get; set; }
        public IEnumerable<ISeries> MemorySeries { get; set; }

        public ComputerDetailsPageCharts(Computer computer) {
            ProgramsSeries = computer.processes.GroupBy(x => x.ProgramName).OrderBy(x => x.Key).Select(x => new PieSeries<int>
            {
                Values = [x.Count()],
                Name = x.Key
            }).ToList();

            ProcessorSeries = computer.processes.OrderBy(x => x.FileName).Select(x => new PieSeries<int>
            {
                Values = [x.ProcessorUsage],
                Name = x.FileName,
                ToolTipLabelFormatter = point => $"{x.ProcessorUsage} (TODO)"
            }).ToList();

            MemorySeries = computer.processes.OrderBy(x => x.FileName).Select(x => new PieSeries<int>
            {
                Values = [x.MemoryUsage],
                Name = x.FileName
            }).ToList();
        }
    }
}
