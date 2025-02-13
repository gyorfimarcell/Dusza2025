using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Extensions;
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
    class ComputerDetailsPageCharts
    {
        public IEnumerable<ISeries> ProgramsSeries { get; set; }
        public IEnumerable<ISeries> ProcessorSeries { get; set; }
        public IEnumerable<ISeries> MemorySeries { get; set; }

        public ComputerDetailsPageCharts(Computer computer) {
            ProgramsSeries = computer.processes.GroupBy(x => x.ProgramName).OrderBy(x => x.Key)
            .Select(x => new PieSeries<int> {
                Values = [x.Count()],
                Name = x.Key
            }).ToList();

            IEnumerable<Process> activeProcesses = computer.processes.Where(x => x.Active);

            ProcessorSeries = GaugeGenerator.BuildAngularGaugeSections(activeProcesses.Select(x => new GaugeItem(
                x.ProcessorUsage, s => FormatGaugeItem(s, x.FileName)
            )).Append(GetGaugeBackground(computer.ProcessorUsage / (double)computer.ProcessorCore * 100)).ToArray());

            MemorySeries = GaugeGenerator.BuildAngularGaugeSections(activeProcesses.Select(x => new GaugeItem(
                x.MemoryUsage, s => FormatGaugeItem(s, x.FileName)
            )).Append(GetGaugeBackground(computer.MemoryUsage / (double)computer.RamCapacity * 100)).ToArray());
        }

        private void FormatGaugeItem(PieSeries<ObservableValue> s, string name)
        {
            s.Name = name;
            s.InnerRadius = 50;
            s.CornerRadius = 0;
            s.IsHoverable = true;
            s.AnimationsSpeed = TimeSpan.FromMilliseconds(800);
        }

        private GaugeItem GetGaugeBackground(double percent)
        {
            return new GaugeItem(GaugeItem.Background, s =>
            {
                s.InnerRadius = 50;
                s.DataLabelsSize = 24;
                s.DataLabelsPaint = (SolidColorPaint?)LiveCharts.DefaultSettings.LegendTextPaint;
                s.DataLabelsPosition = PolarLabelsPosition.ChartCenter;
                s.DataLabelsFormatter = _ => $"{percent:f0}%";
            });
        }
    }
}
