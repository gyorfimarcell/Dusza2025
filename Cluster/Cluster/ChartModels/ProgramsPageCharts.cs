using LiveChartsCore;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace Cluster.ChartModels
{
    internal class ProgramsPageCharts
    {
        public ISeries[] RequestedSeries { get; }

        public Axis[] RequestedXAxis { get; }
        public Axis[] RequestedYAxis { get; }

        public ProgramsPageCharts(IEnumerable<ProgramType> programs)
        {
            List<ProgramType> sortedPrograms = programs.OrderBy(x => x.ProgramName).ToList();

            RequestedSeries = sortedPrograms.Select(x => new StackedRowSeries<int>
            {
                Values = [x.ActivePrograms],
                Name = x.ProgramName,
                DataLabelsPaint = new SolidColorPaint(new SKColor(245, 245, 245)),
                DataLabelsPosition = DataLabelsPosition.Middle,
                IsHoverable = false,
                StackGroup = 0
            }).ToArray<ISeries>();

            RequestedXAxis =
                [new Axis { IsVisible = false, MinLimit = 0, MaxLimit = sortedPrograms.Sum(x => x.ActivePrograms) }];
            RequestedYAxis = [new Axis { IsVisible = false }];
        }
    }
}