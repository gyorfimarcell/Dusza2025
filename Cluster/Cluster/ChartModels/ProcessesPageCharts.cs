using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace Cluster.ChartModels
{
    internal class ProcessesPageCharts
    {
        public ISeries ProgramsActiveSeries { get; }
        public ISeries ProgramInactiveSeries { get; }
        public Axis[] ProgramsAxes { get; }
        public Axis[] ProgramsYAxes { get; }

        public IEnumerable<ISeries> ComputersSeries { get; }

        public ProcessesPageCharts(IEnumerable<Process> processes)
        {
            List<IGrouping<string, Process>> programGroups =
                processes.GroupBy(x => x.ProgramName).OrderBy(x => x.Key).ToList();

            ProgramsActiveSeries = new ColumnSeries<int>
            {
                Name = TranslationSource.T("Active"),
                Values = programGroups.Select(x => x.Count(y => y.Active)).ToList(),
                Fill = new SolidColorPaint(SKColors.LimeGreen),
                Rx = 2,
                Ry = 2
            };
            ProgramInactiveSeries = new ColumnSeries<int>
            {
                Name = TranslationSource.T("Inactive"),
                Values = programGroups.Select(x => x.Count(y => !y.Active)).ToList(),
                Fill = new SolidColorPaint(SKColors.Red),
                Rx = 2,
                Ry = 2
            };
            ProgramsAxes =
            [
                new Axis
                {
                    Labels = programGroups.Select(x => x.Key).ToList(),
                }
            ];
            ProgramsYAxes =
            [
                new Axis
                {
                    MinStep = 1,
                    MinLimit = 0
                }
            ];

            ComputersSeries = processes.GroupBy(x => x.HostComputer.Name).Select(x => new PieSeries<int>
            {
                Values = [x.Count()],
                Name = x.Key,
                InnerRadius = 30,
                ToolTipLabelFormatter = point =>
                    $"{point.Coordinate.PrimaryValue} {TranslationSource.T("ProcessesPage.ComputerTooltip")}"
            }).ToList();
        }
    }
}