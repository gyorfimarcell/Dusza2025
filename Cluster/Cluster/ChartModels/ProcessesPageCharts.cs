using LiveChartsCore.SkiaSharpView.Extensions;
using LiveChartsCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiveChartsCore.SkiaSharpView;
using System.Collections;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace Cluster.ChartModels
{
    class ProcessesPageCharts
    {
        //public IEnumerable<ISeries> ProgramsSeries { get; set; } = processes.GroupBy(x => x.ProgramName).Select(x => new PieSeries<int> { Values = [x.Count()], Name = x.Key }).ToList();

        public ISeries ProgramsActiveSeries { get; set; }
        public ISeries ProgramInactiveSeries { get; set; }
        public Axis[] ProgramsAxes { get; set; }
        public Axis[] ProgramsYAxes { get; set; }

        public IEnumerable<ISeries> ComputersSeries { get; set; }

        public ProcessesPageCharts(IEnumerable<Process> processes) {
            var programGroups = processes.GroupBy(x => x.ProgramName).OrderBy(x => x.Key);


            ProgramsActiveSeries = new ColumnSeries<int> {
                Name = "Active",
                Values = programGroups.Select(x => x.Count(y => y.Active)).ToList(),
                Fill = new SolidColorPaint(SKColors.LimeGreen),
                Rx = 2,
                Ry = 2
            };
            ProgramInactiveSeries = new ColumnSeries<int>
            {
                Name = "Inactive",
                Values = programGroups.Select(x => x.Count(y => !y.Active)).ToList(),
                Fill = new SolidColorPaint(SKColors.Red),
                Rx = 2,
                Ry = 2
            };
            ProgramsAxes = [new Axis {
                Labels = programGroups.Select(x => x.Key).ToList(),
            }];
            ProgramsYAxes = [new Axis {
                MinStep = 1,
                MinLimit = 0
            }];

            ComputersSeries = processes.GroupBy(x => x.HostComputer.Name).Select(x => new PieSeries<int> {
                Values = [x.Count()],
                Name = x.Key,
                InnerRadius = 30
            }).ToList();
        }
    }
}
