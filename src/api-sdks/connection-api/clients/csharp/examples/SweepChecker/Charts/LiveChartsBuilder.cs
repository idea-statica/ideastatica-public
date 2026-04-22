using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using SweepChecker.Models;

namespace SweepChecker.Charts
{
	/// <summary>
	/// LiveCharts2 variant of the same "critical per load case" chart that Panel 1 of
	/// <see cref="CanvasCharts"/> renders. Axis titles, chart title, and X-tick labels
	/// deliberately mirror the Canvas version so switching tabs is seamless.
	/// </summary>
	public static class LiveChartsBuilder
	{
		public static (ISeries[] Series, ICartesianAxis[] XAxes, ICartesianAxis[] YAxes) BuildCriticalByLoadCase(
			IReadOnlyList<LoadCaseRow> rows,
			Func<LoadCaseRow, double> value,
			string yLabel)
		{
			var byLc = rows
				.Where(r => r.LoadCaseId.HasValue)
				.GroupBy(r => r.LoadCaseId!.Value)
				.Select(g => g.OrderByDescending(value).First())
				.OrderBy(r => r.LoadCaseId)
				.ToList();

			// Use integer indices 0..N-1 on the x-axis; the tick labels come from xLabels below
			// so the labelling mode matches the Canvas chart ("LC 0", "LC 1", ...).
			var xLabels = byLc.Select(r => $"LC {r.LoadCaseId}").ToArray();
			var points = byLc.Select((r, i) => new ObservablePoint(i, value(r))).ToArray();

			var axisBrush = new SolidColorPaint(new SKColor(0x33, 0x33, 0x33));

			var line = new LineSeries<ObservablePoint>
			{
				Values = points,
				Name = yLabel,
				GeometrySize = 10,
				Stroke = new SolidColorPaint(new SKColor(0x1E, 0x88, 0xE5), 2),
				GeometryStroke = new SolidColorPaint(new SKColor(0x1E, 0x88, 0xE5), 2),
				GeometryFill = new SolidColorPaint(SKColors.White),
				Fill = null,
			};

			var series = new List<ISeries> { line };

			if (points.Length > 0)
			{
				var maxPt = points.OrderByDescending(p => p.Y).First();
				series.Add(new ScatterSeries<ObservablePoint>
				{
					Values = new[] { maxPt },
					Name = "Max",
					GeometrySize = 18,
					Fill = new SolidColorPaint(new SKColor(0xFB, 0x8C, 0x00)),
					Stroke = new SolidColorPaint(SKColors.Black, 1),
				});
			}

			var xAxes = new ICartesianAxis[]
			{
				new Axis
				{
					Name = CanvasCharts.XAxisTitle,
					NamePaint = axisBrush,
					LabelsPaint = axisBrush,
					Labels = xLabels,
					MinStep = 1,
					ForceStepToMin = true,
				}
			};

			var yAxes = new ICartesianAxis[]
			{
				new Axis
				{
					Name = yLabel,
					NamePaint = axisBrush,
					LabelsPaint = axisBrush,
				}
			};

			return (series.ToArray(), xAxes, yAxes);
		}
	}
}
