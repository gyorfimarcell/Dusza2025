using LiveChartsCore;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cluster.ChartModels
{
    class ProgramsPageCharts
    {
        public ISeries[] RequestedSeries { get; set; }

        public Axis[] RequestedXAxis { get; set; }
        public Axis[] RequestedYAxis { get; set; }

        public ProgramsPageCharts(IEnumerable<ProgramType> programs) {
            var sortedPrograms = programs.OrderBy(x => x.ProgramName);

            RequestedSeries = sortedPrograms.Select(x => new StackedRowSeries<int> {
                Values = [x.ActivePrograms],
                Name = x.ProgramName,
                DataLabelsPaint = new SolidColorPaint(new SKColor(245, 245, 245)),
                DataLabelsPosition = DataLabelsPosition.Middle,
                IsHoverable = false,
                StackGroup = 0
            }).ToArray();

            RequestedXAxis = [new Axis { IsVisible = false, MinLimit = 0, MaxLimit = sortedPrograms.Sum(x => x.ActivePrograms) }];
            RequestedYAxis = [new Axis { IsVisible = false }];
        }
    }
}
