using SweepChecker.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SweepChecker.Charts
{
	/// <summary>
	/// 3-panel chart layout rendered directly onto a WPF Canvas (no external chart library).
	///  Panel 1 — critical value per load case (line chart)
	///  Panel 2 — opening schematic at the globally critical parameters
	///  Panel 3 — max / min across load cases, with rotation on a secondary axis
	/// Mirrors make_three_panel_figure() in Results_reading_06.py.
	/// </summary>
	public static class CanvasCharts
	{
		public record ParamNames(string? Width, string? Depth, string? Rotation);

		/// <summary>Detect W/D/Rot-like keys so the schematic can still render when names differ.</summary>
		public static ParamNames DetectParamNames(IEnumerable<string> keys)
		{
			string? W = null, D = null, R = null;
			foreach (var k in keys)
			{
				var l = k.ToLowerInvariant();
				if (R == null && (l.Contains("rot") || l.Contains("angle") || l == "phi" || l.Contains("theta"))) R = k;
				else if (W == null && (l == "w" || l.Contains("width") || l.Contains("wid"))) W = k;
				else if (D == null && (l == "d" || l.Contains("depth") || l.Contains("height") || l == "h")) D = k;
			}
			// Fallback: just use first/second non-rotation keys in order
			var nonRot = keys.Where(k => k != R).ToList();
			W ??= nonRot.ElementAtOrDefault(0);
			D ??= nonRot.ElementAtOrDefault(1);
			return new ParamNames(W, D, R);
		}

		/// <summary>Common title text used by both the Canvas Panel 1 and LiveCharts2 — keeps them in sync.</summary>
		public static string CriticalByLcTitle(string section) => $"{section}: critical per load case";
		public const string XAxisTitle = "Load case";

		public static void Render(
			Canvas canvas,
			IReadOnlyList<LoadCaseRow> rows,
			string title,
			Func<LoadCaseRow, double> valueSelector,
			string valueLabel,
			ParamNames paramNames)
		{
			canvas.Children.Clear();
			double w = canvas.ActualWidth, h = canvas.ActualHeight;
			if (w < 80 || h < 80) return;
			if (rows.Count == 0) { AddLabel(canvas, "No data for current filters", w / 2 - 60, h / 2 - 10, 14, Brushes.Gray); return; }

			// Split canvas into three horizontal panels
			double panelW = w / 3.0;
			var p1 = new Rect(0, 0, panelW, h);
			var p2 = new Rect(panelW, 0, panelW, h);
			var p3 = new Rect(2 * panelW, 0, panelW, h);

			// Common: resolve the globally-critical row by the selected value column
			var critical = rows.OrderByDescending(valueSelector).FirstOrDefault();

			DrawCriticalByLoadCase(canvas, p1, rows, valueSelector, CriticalByLcTitle(title), valueLabel);
			DrawOpeningSchematic(canvas, p2, critical, paramNames, $"{title}: critical dimensions");
			DrawMinMaxByLoadCase(canvas, p3, rows, valueSelector, $"{title}: max / min per load case", valueLabel);
		}

		// ───────── Panel 1: max value across iterations, one point per LC ─────────

		private static void DrawCriticalByLoadCase(
			Canvas c, Rect area, IReadOnlyList<LoadCaseRow> rows,
			Func<LoadCaseRow, double> value, string title, string yLabel)
		{
			DrawPanelBackground(c, area, title);

			var byLc = rows
				.Where(r => r.LoadCaseId.HasValue)
				.GroupBy(r => r.LoadCaseId!.Value)
				.Select(g => g.OrderByDescending(value).First())
				.OrderBy(r => r.LoadCaseId)
				.ToList();

			if (byLc.Count == 0) { AddLabel(c, "No load-case data", area.X + 20, area.Y + area.Height / 2, 12, Brushes.Gray); return; }

			var plot = GetPlotArea(area);
			double maxV = Math.Max(1e-9, byLc.Max(value)) * 1.15;
			double minV = 0;

			DrawAxes(c, plot, byLc.Count, maxV, minV, yLabel, i => $"LC {byLc[i].LoadCaseId}");
			// x-axis title
			AddLabel(c, XAxisTitle, plot.X + plot.Width / 2 - 22, plot.Y + plot.Height + 18, 9, Brushes.Black, true);

			var points = byLc.Select((r, i) => new Point(
				plot.X + (byLc.Count <= 1 ? plot.Width / 2 : (i * plot.Width / (byLc.Count - 1))),
				plot.Y + plot.Height - (value(r) - minV) / (maxV - minV) * plot.Height
			)).ToList();

			// Line
			var poly = new Polyline { Stroke = Brushes.DodgerBlue, StrokeThickness = 2 };
			foreach (var p in points) poly.Points.Add(p);
			c.Children.Add(poly);

			// Dots + tooltips
			foreach (var (p, r) in points.Zip(byLc))
			{
				var dot = new Ellipse
				{
					Width = 7, Height = 7, Fill = Brushes.DodgerBlue,
					ToolTip = $"LC {r.LoadCaseId}\n{yLabel}={value(r):F3}\n{r.ParameterLabel}"
				};
				Canvas.SetLeft(dot, p.X - 3.5);
				Canvas.SetTop(dot, p.Y - 3.5);
				c.Children.Add(dot);
			}
		}

		// ───────── Full-canvas hole visualization (face view + edge view with cables) ─────────

		/// <summary>
		/// Large two-pane hole visualization. Left pane = face-on view of the member showing
		/// the rotated rectangular hole with dimension lines and rotation arc. Right pane =
		/// edge view showing the member thickness with cables threaded through the hole.
		/// </summary>
		public static void RenderHole(Canvas canvas, LoadCaseRow? row, ParamNames names, string subtitle = "")
		{
			canvas.Children.Clear();
			double w = canvas.ActualWidth, h = canvas.ActualHeight;
			if (w < 80 || h < 80) return;
			if (row == null || names.Width == null || names.Depth == null)
			{
				AddLabel(canvas, "No data — run a sweep and a critical case will appear here.",
					w / 2 - 170, h / 2 - 8, 13, Brushes.Gray);
				return;
			}

			double W = row.Parameters.TryGetValue(names.Width, out var vw) ? vw : 0.15;
			double D = row.Parameters.TryGetValue(names.Depth, out var vd) ? vd : 0.15;
			double rot = names.Rotation != null && row.Parameters.TryGetValue(names.Rotation, out var vr) ? vr : 0;
			double rotDeg = Math.Abs(rot) < Math.PI * 2 ? rot * 180.0 / Math.PI : rot;

			// Header
			string header = $"Critical hole — {names.Width}={W:F3}, {names.Depth}={D:F3}"
				+ (names.Rotation != null ? $", {names.Rotation}={rotDeg:F1}°" : "");
			if (!string.IsNullOrEmpty(subtitle)) header += "   ·   " + subtitle;
			if (row.LoadCaseId.HasValue) header += $"   ·   LC {row.LoadCaseId}";
			AddLabel(canvas, header, 16, 8, 13, Brushes.Black, true);

			double topPad = 36;
			double gap = 12;
			double leftW = w * 0.58 - gap / 2;
			double rightW = w * 0.42 - gap / 2;
			var leftRect = new Rect(8, topPad, leftW - 16, h - topPad - 12);
			var rightRect = new Rect(8 + leftW + gap, topPad, rightW - 16, h - topPad - 12);

			DrawFaceView(canvas, leftRect, W, D, rotDeg, names);
			DrawEdgeView(canvas, rightRect, W, D, rotDeg);
		}

		/// <summary>Face-on view: member outline + rotated hole with W/D dimensions and rotation arc.</summary>
		private static void DrawFaceView(Canvas c, Rect area, double W, double D, double rotDeg, ParamNames names)
		{
			// Member outline (simple plate / web face). Keep member wider than hole so annotations have room.
			double memberScale = Math.Min(area.Width, area.Height) * 0.82;
			double memberW = memberScale * 0.95;
			double memberH = memberScale * 1.0;
			double cx = area.X + area.Width / 2;
			double cy = area.Y + area.Height / 2 + 6;

			// Panel background
			c.Children.Add(new System.Windows.Shapes.Rectangle
			{
				Width = area.Width, Height = area.Height,
				Fill = new SolidColorBrush(Color.FromRgb(0xFA, 0xFB, 0xFD)),
				Stroke = Brushes.Gainsboro, StrokeThickness = 0.6,
			});
			var bg = (System.Windows.Shapes.Rectangle)c.Children[c.Children.Count - 1];
			Canvas.SetLeft(bg, area.X); Canvas.SetTop(bg, area.Y);

			AddLabel(c, "Face view", area.X + 10, area.Y + 8, 10, Brushes.DimGray, true);

			// Member plate
			var plate = new System.Windows.Shapes.Rectangle
			{
				Width = memberW, Height = memberH,
				Fill = new SolidColorBrush(Color.FromRgb(0xDD, 0xE4, 0xEE)),
				Stroke = new SolidColorBrush(Color.FromRgb(0x4B, 0x64, 0x7D)),
				StrokeThickness = 1.5,
			};
			Canvas.SetLeft(plate, cx - memberW / 2);
			Canvas.SetTop(plate, cy - memberH / 2);
			c.Children.Add(plate);

			// Scale so the hole is visible but still clearly inside the member.
			// Fit the larger of W,D to ~55% of the plate size.
			double fitScale = Math.Min(memberW, memberH) * 0.55 / Math.Max(1e-6, Math.Max(W, D));
			double ow = W * fitScale;
			double od = D * fitScale;

			// Rotated hole corners (local frame axis-aligned, then rotated by rotDeg about center)
			var rad = rotDeg * Math.PI / 180.0;
			double ca = Math.Cos(rad), sa = Math.Sin(rad);
			Point R(double lx, double ly) => new(cx + lx * ca - ly * sa, cy + lx * sa + ly * ca);

			var corners = new[]
			{
				R(-ow/2, -od/2), R( ow/2, -od/2), R( ow/2,  od/2), R(-ow/2,  od/2)
			};
			var hole = new System.Windows.Shapes.Polygon
			{
				Stroke = Brushes.Black, StrokeThickness = 2.2,
				Fill = new SolidColorBrush(Color.FromArgb(70, 0x93, 0xC5, 0xFD)),
			};
			foreach (var p in corners) hole.Points.Add(p);
			c.Children.Add(hole);

			// Cross-hatch inside the hole to indicate "void / cable pass-through"
			for (int i = -5; i <= 5; i++)
			{
				double t = i * Math.Max(ow, od) / 10.0;
				// line segments in local frame (parallel to W axis), clipped to hole rectangle
				var a = R(-ow / 2, t);
				var b = R(ow / 2, t);
				// Only draw if |t| <= od/2
				if (Math.Abs(t) <= od / 2)
					c.Children.Add(new System.Windows.Shapes.Line
					{
						X1 = a.X, Y1 = a.Y, X2 = b.X, Y2 = b.Y,
						Stroke = new SolidColorBrush(Color.FromArgb(40, 0x00, 0x5A, 0x8A)),
						StrokeThickness = 0.6,
					});
			}

			// Cable cross-sections (2 dots in local frame, one above one below center along D axis)
			var cable1 = R(0, -od * 0.2);
			var cable2 = R(0, od * 0.2);
			DrawCableCrossSection(c, cable1, 6);
			DrawCableCrossSection(c, cable2, 6);

			// W dimension line (parallel to local W axis, offset outside hole in -D direction)
			double dimOff = Math.Max(10, Math.Min(memberW, memberH) * 0.05);
			DrawDimensionLine(c,
				R(-ow / 2, -od / 2 - dimOff),
				R(ow / 2, -od / 2 - dimOff),
				$"{names.Width} = {W:F3}",
				Brushes.DarkSlateBlue,
				rotDeg);

			// D dimension line (parallel to local D axis, offset in -W direction)
			DrawDimensionLine(c,
				R(-ow / 2 - dimOff, -od / 2),
				R(-ow / 2 - dimOff, od / 2),
				$"{names.Depth} = {D:F3}",
				Brushes.DarkSlateBlue,
				rotDeg + 90);

			// Rotation arc + reference horizontal
			if (names.Rotation != null)
			{
				double arcR = Math.Max(ow, od) * 0.75;
				// Reference horizontal line (dashed) from center to the right
				c.Children.Add(new System.Windows.Shapes.Line
				{
					X1 = cx, Y1 = cy, X2 = cx + arcR, Y2 = cy,
					Stroke = Brushes.Gray, StrokeThickness = 0.8,
					StrokeDashArray = new DoubleCollection { 3, 3 },
				});
				// Rotated local-X axis (solid, red)
				var axisEnd = R(arcR, 0);
				c.Children.Add(new System.Windows.Shapes.Line
				{
					X1 = cx, Y1 = cy, X2 = axisEnd.X, Y2 = axisEnd.Y,
					Stroke = Brushes.Crimson, StrokeThickness = 1.4,
				});
				// Arc (approximated by polyline over 20 steps)
				DrawArc(c, cx, cy, arcR * 0.55, 0, rotDeg, Brushes.Crimson, 1.4);

				// Rotation label near the arc midpoint
				var mid = 0.5 * rotDeg;
				double mx = cx + arcR * 0.68 * Math.Cos(mid * Math.PI / 180);
				double my = cy + arcR * 0.68 * Math.Sin(mid * Math.PI / 180);
				AddLabel(c, $"{names.Rotation} = {rotDeg:F1}°", mx + 4, my - 6, 10, Brushes.Crimson, true);
			}
		}

		/// <summary>Edge view: member shown as a horizontal slab, with cables threaded through the hole.</summary>
		private static void DrawEdgeView(Canvas c, Rect area, double W, double D, double rotDeg)
		{
			// Panel background
			c.Children.Add(new System.Windows.Shapes.Rectangle
			{
				Width = area.Width, Height = area.Height,
				Fill = new SolidColorBrush(Color.FromRgb(0xFA, 0xFB, 0xFD)),
				Stroke = Brushes.Gainsboro, StrokeThickness = 0.6,
			});
			var bg = (System.Windows.Shapes.Rectangle)c.Children[c.Children.Count - 1];
			Canvas.SetLeft(bg, area.X); Canvas.SetTop(bg, area.Y);

			AddLabel(c, "Edge view · cables", area.X + 10, area.Y + 8, 10, Brushes.DimGray, true);

			// Layout — horizontal plate slab centered vertically
			double cx = area.X + area.Width / 2;
			double cy = area.Y + area.Height / 2 + 4;
			double plateLen = area.Width * 0.82;
			double plateThk = Math.Min(30, area.Height * 0.16);

			// Projected hole opening on the edge view is roughly D * sin(rot) + W * cos(rot) wide
			// on the member face. We just use the smaller of (W,D) as a visual "hole height".
			double holeH = Math.Min(area.Height * 0.55, Math.Min(W, D) * plateLen / Math.Max(1e-6, Math.Max(W, D)) * 1.6);
			holeH = Math.Max(20, holeH);

			// Plate
			var plate = new System.Windows.Shapes.Rectangle
			{
				Width = plateLen, Height = plateThk,
				Fill = new SolidColorBrush(Color.FromRgb(0xCF, 0xD8, 0xE3)),
				Stroke = new SolidColorBrush(Color.FromRgb(0x4B, 0x64, 0x7D)),
				StrokeThickness = 1.4,
			};
			Canvas.SetLeft(plate, cx - plateLen / 2);
			Canvas.SetTop(plate, cy - plateThk / 2);
			c.Children.Add(plate);

			// Hole silhouette — clear the plate stripe where the hole passes through
			double holeEdgeWidth = plateThk * 3;
			var holeRect = new System.Windows.Shapes.Rectangle
			{
				Width = holeEdgeWidth, Height = plateThk,
				Fill = Brushes.White,
				Stroke = new SolidColorBrush(Color.FromRgb(0x4B, 0x64, 0x7D)),
				StrokeThickness = 0.8,
				StrokeDashArray = new DoubleCollection { 4, 2 },
			};
			Canvas.SetLeft(holeRect, cx - holeEdgeWidth / 2);
			Canvas.SetTop(holeRect, cy - plateThk / 2);
			c.Children.Add(holeRect);

			// Cables: 2 horizontal strands threading through the hole
			var cableBrush = new SolidColorBrush(Color.FromRgb(0x57, 0x3A, 0x17));
			for (int i = 0; i < 2; i++)
			{
				double dy = (i == 0 ? -1 : 1) * plateThk * 0.2;
				// Cable line
				c.Children.Add(new System.Windows.Shapes.Line
				{
					X1 = area.X + 6, Y1 = cy + dy, X2 = area.X + area.Width - 6, Y2 = cy + dy,
					Stroke = cableBrush, StrokeThickness = 3,
				});
				// End caps (small bulbs)
				c.Children.Add(new System.Windows.Shapes.Ellipse
				{
					Width = 7, Height = 7, Fill = cableBrush,
				});
				var e1 = (System.Windows.Shapes.Ellipse)c.Children[c.Children.Count - 1];
				Canvas.SetLeft(e1, area.X + 2); Canvas.SetTop(e1, cy + dy - 3.5);

				c.Children.Add(new System.Windows.Shapes.Ellipse
				{
					Width = 7, Height = 7, Fill = cableBrush,
				});
				var e2 = (System.Windows.Shapes.Ellipse)c.Children[c.Children.Count - 1];
				Canvas.SetLeft(e2, area.X + area.Width - 9); Canvas.SetTop(e2, cy + dy - 3.5);
			}

			// Arrow showing cable direction
			double ay = cy + plateThk * 0.85;
			DrawArrow(c, area.X + 16, ay, area.X + area.Width - 16, ay, Brushes.SaddleBrown, 1.2);
			AddLabel(c, "cables", area.X + area.Width / 2 - 18, ay + 2, 9, Brushes.SaddleBrown);

			// Thickness label
			AddLabel(c, "member thickness", cx - 55, cy + plateThk / 2 + 6, 9, Brushes.DimGray);
		}

		private static void DrawCableCrossSection(Canvas c, Point p, double r)
		{
			var outer = new System.Windows.Shapes.Ellipse
			{
				Width = r * 2, Height = r * 2,
				Fill = new SolidColorBrush(Color.FromRgb(0x8D, 0x62, 0x3C)),
				Stroke = Brushes.Black, StrokeThickness = 1,
			};
			Canvas.SetLeft(outer, p.X - r); Canvas.SetTop(outer, p.Y - r);
			c.Children.Add(outer);
			var inner = new System.Windows.Shapes.Ellipse
			{
				Width = r, Height = r, Fill = new SolidColorBrush(Color.FromRgb(0xC7, 0x8E, 0x55)),
			};
			Canvas.SetLeft(inner, p.X - r / 2); Canvas.SetTop(inner, p.Y - r / 2);
			c.Children.Add(inner);
		}

		/// <summary>Draw a double-arrow dimension line with a rotated label near the midpoint.</summary>
		private static void DrawDimensionLine(Canvas c, Point a, Point b, string label, Brush color, double labelRotDeg)
		{
			DrawArrow(c, a.X, a.Y, b.X, b.Y, color, 1.2, doubleHead: true);
			var mid = new Point((a.X + b.X) / 2, (a.Y + b.Y) / 2);
			var tb = new TextBlock
			{
				Text = label,
				FontSize = 10, Foreground = color, FontWeight = FontWeights.SemiBold,
				FontFamily = new FontFamily("Segoe UI"),
				RenderTransform = new RotateTransform(labelRotDeg % 180 > 90 ? labelRotDeg - 180 : labelRotDeg),
				Background = new SolidColorBrush(Color.FromArgb(200, 255, 255, 255)),
				Padding = new Thickness(2, 0, 2, 0),
			};
			Canvas.SetLeft(tb, mid.X - 30);
			Canvas.SetTop(tb, mid.Y - 8);
			c.Children.Add(tb);
		}

		private static void DrawArrow(Canvas c, double x1, double y1, double x2, double y2, Brush color, double thickness, bool doubleHead = false)
		{
			c.Children.Add(new System.Windows.Shapes.Line { X1 = x1, Y1 = y1, X2 = x2, Y2 = y2, Stroke = color, StrokeThickness = thickness });
			AddArrowHead(c, x2, y2, x1, y1, color);
			if (doubleHead) AddArrowHead(c, x1, y1, x2, y2, color);
		}

		private static void AddArrowHead(Canvas c, double tipX, double tipY, double fromX, double fromY, Brush color)
		{
			double dx = tipX - fromX, dy = tipY - fromY;
			double len = Math.Sqrt(dx * dx + dy * dy);
			if (len < 0.1) return;
			double ux = dx / len, uy = dy / len;
			double size = 7;
			double baseX = tipX - ux * size;
			double baseY = tipY - uy * size;
			double perpX = -uy, perpY = ux;

			var head = new System.Windows.Shapes.Polygon { Fill = color };
			head.Points.Add(new Point(tipX, tipY));
			head.Points.Add(new Point(baseX + perpX * size * 0.4, baseY + perpY * size * 0.4));
			head.Points.Add(new Point(baseX - perpX * size * 0.4, baseY - perpY * size * 0.4));
			c.Children.Add(head);
		}

		private static void DrawArc(Canvas c, double cx, double cy, double r, double startDeg, double endDeg, Brush stroke, double thickness)
		{
			if (Math.Abs(endDeg - startDeg) < 0.5) return;
			int steps = 24;
			var poly = new System.Windows.Shapes.Polyline { Stroke = stroke, StrokeThickness = thickness };
			for (int i = 0; i <= steps; i++)
			{
				double t = i / (double)steps;
				double a = (startDeg + (endDeg - startDeg) * t) * Math.PI / 180.0;
				poly.Points.Add(new Point(cx + r * Math.Cos(a), cy + r * Math.Sin(a)));
			}
			c.Children.Add(poly);
		}

		// ───────── Panel 2: opening schematic ─────────

		private static void DrawOpeningSchematic(
			Canvas c, Rect area, LoadCaseRow? row, ParamNames names, string title)
		{
			DrawPanelBackground(c, area, title);
			if (row == null || names.Width == null || names.Depth == null)
			{ AddLabel(c, "No critical case", area.X + 20, area.Y + area.Height / 2, 12, Brushes.Gray); return; }

			double W = row.Parameters.TryGetValue(names.Width, out var vw) ? vw : 0.15;
			double D = row.Parameters.TryGetValue(names.Depth, out var vd) ? vd : 0.15;
			double rot = names.Rotation != null && row.Parameters.TryGetValue(names.Rotation, out var vr) ? vr : 0;

			// If rotation looks like radians convert to degrees for display
			double rotDeg = Math.Abs(rot) < Math.PI * 2 ? rot * 180.0 / Math.PI : rot;

			var cx = area.X + area.Width * 0.42;
			var cy = area.Y + area.Height * 0.55;
			double plateSize = Math.Min(area.Width, area.Height) * 0.65;

			// Outer plate
			c.Children.Add(new Rectangle
			{
				Width = plateSize, Height = plateSize, Stroke = Brushes.SlateGray, StrokeThickness = 1.5,
				Fill = new SolidColorBrush(Color.FromRgb(0xDB, 0xE7, 0xF3)),
				RenderTransformOrigin = new Point(0.5, 0.5),
			});
			var lastPlate = (Rectangle)c.Children[c.Children.Count - 1];
			Canvas.SetLeft(lastPlate, cx - plateSize / 2);
			Canvas.SetTop(lastPlate, cy - plateSize / 2);

			// Opening (rectangle, rotated)
			double scale = plateSize / Math.Max(0.3, Math.Max(W, D) * 1.8);
			double ow = Math.Max(10, W * scale);
			double od = Math.Max(10, D * scale);
			var opening = new Rectangle
			{
				Width = ow, Height = od, Stroke = Brushes.Black, StrokeThickness = 2,
				Fill = new SolidColorBrush(Color.FromArgb(90, 0x93, 0xC5, 0xFD)),
				RenderTransform = new RotateTransform(rotDeg),
				RenderTransformOrigin = new Point(0.5, 0.5),
			};
			Canvas.SetLeft(opening, cx - ow / 2);
			Canvas.SetTop(opening, cy - od / 2);
			c.Children.Add(opening);

			// Info card
			double ix = area.X + area.Width - 120;
			double iy = area.Y + 40;
			c.Children.Add(new Rectangle
			{
				Width = 110, Height = 92,
				Fill = Brushes.White, Stroke = Brushes.Gainsboro, StrokeThickness = 1,
			});
			var card = (Rectangle)c.Children[c.Children.Count - 1];
			Canvas.SetLeft(card, ix); Canvas.SetTop(card, iy);

			AddLabel(c, "Critical case", ix + 8, iy + 6, 11, Brushes.Black, true);
			AddLabel(c, $"{names.Width} = {W:F3}", ix + 8, iy + 26, 10, Brushes.DimGray);
			AddLabel(c, $"{names.Depth} = {D:F3}", ix + 8, iy + 44, 10, Brushes.DimGray);
			if (names.Rotation != null)
				AddLabel(c, $"{names.Rotation} = {rotDeg:F1}°", ix + 8, iy + 62, 10, Brushes.Crimson);
			if (row.LoadCaseId.HasValue)
				AddLabel(c, $"LC {row.LoadCaseId}", ix + 8, iy + 78, 10, Brushes.SteelBlue);
		}

		// ───────── Panel 3: max / min per LC ─────────

		private static void DrawMinMaxByLoadCase(
			Canvas c, Rect area, IReadOnlyList<LoadCaseRow> rows,
			Func<LoadCaseRow, double> value, string title, string yLabel)
		{
			DrawPanelBackground(c, area, title);

			var byLc = rows
				.Where(r => r.LoadCaseId.HasValue)
				.GroupBy(r => r.LoadCaseId!.Value)
				.Select(g => (Lc: g.Key, Max: g.Max(value), Min: g.Min(value)))
				.OrderBy(t => t.Lc)
				.ToList();

			if (byLc.Count == 0) { AddLabel(c, "No load-case data", area.X + 20, area.Y + area.Height / 2, 12, Brushes.Gray); return; }

			var plot = GetPlotArea(area);
			double maxV = Math.Max(1e-9, byLc.Max(t => t.Max)) * 1.15;

			DrawAxes(c, plot, byLc.Count, maxV, 0, yLabel, i => $"LC {byLc[i].Lc}");
			AddLabel(c, XAxisTitle, plot.X + plot.Width / 2 - 22, plot.Y + plot.Height + 18, 9, Brushes.Black, true);

			double barW = Math.Max(4, plot.Width / (byLc.Count * 3.0));
			for (int i = 0; i < byLc.Count; i++)
			{
				double cxb = plot.X + (byLc.Count <= 1
					? plot.Width / 2
					: (i * plot.Width / (byLc.Count - 1)));

				double yMax = plot.Y + plot.Height - byLc[i].Max / maxV * plot.Height;
				double yMin = plot.Y + plot.Height - byLc[i].Min / maxV * plot.Height;

				var barMax = new Rectangle { Width = barW, Height = plot.Y + plot.Height - yMax, Fill = Brushes.DarkGreen, ToolTip = $"Max={byLc[i].Max:F3}" };
				Canvas.SetLeft(barMax, cxb - barW - 1); Canvas.SetTop(barMax, yMax);
				c.Children.Add(barMax);

				var barMin = new Rectangle { Width = barW, Height = plot.Y + plot.Height - yMin, Fill = Brushes.IndianRed, ToolTip = $"Min={byLc[i].Min:F3}" };
				Canvas.SetLeft(barMin, cxb + 1); Canvas.SetTop(barMin, yMin);
				c.Children.Add(barMin);
			}

			// Legend
			AddLegendSwatch(c, plot.X + plot.Width - 90, plot.Y + 4, Brushes.DarkGreen, "Max");
			AddLegendSwatch(c, plot.X + plot.Width - 90, plot.Y + 20, Brushes.IndianRed, "Min");
		}

		// ───────── Helpers ─────────

		private static Rect GetPlotArea(Rect panel)
		{
			const double ml = 46, mt = 34, mr = 14, mb = 34;
			return new Rect(panel.X + ml, panel.Y + mt, Math.Max(10, panel.Width - ml - mr), Math.Max(10, panel.Height - mt - mb));
		}

		private static void DrawPanelBackground(Canvas c, Rect area, string title)
		{
			c.Children.Add(new Rectangle { Width = area.Width - 4, Height = area.Height - 4, Fill = Brushes.White, Stroke = Brushes.Gainsboro, StrokeThickness = 0.6 });
			var bg = (Rectangle)c.Children[c.Children.Count - 1];
			Canvas.SetLeft(bg, area.X + 2); Canvas.SetTop(bg, area.Y + 2);
			AddLabel(c, title, area.X + 12, area.Y + 8, 11, Brushes.Black, true);
		}

		private static void DrawAxes(Canvas c, Rect plot, int nTicks, double maxY, double minY, string yLabel, Func<int, string> xLabel)
		{
			// Axes
			c.Children.Add(new Line { X1 = plot.X, Y1 = plot.Y + plot.Height, X2 = plot.X + plot.Width, Y2 = plot.Y + plot.Height, Stroke = Brushes.Black, StrokeThickness = 1 });
			c.Children.Add(new Line { X1 = plot.X, Y1 = plot.Y, X2 = plot.X, Y2 = plot.Y + plot.Height, Stroke = Brushes.Black, StrokeThickness = 1 });

			// Y ticks
			double stepY = NiceStep(maxY - minY, 4);
			for (double v = minY; v <= maxY + 1e-9; v += stepY)
			{
				double y = plot.Y + plot.Height - (v - minY) / (maxY - minY) * plot.Height;
				c.Children.Add(new Line { X1 = plot.X - 3, Y1 = y, X2 = plot.X + plot.Width, Y2 = y, Stroke = Brushes.Gainsboro, StrokeThickness = 0.4 });
				AddLabel(c, v.ToString("G3"), plot.X - 36, y - 7, 9, Brushes.DimGray);
			}
			AddLabel(c, yLabel, plot.X - 38, plot.Y - 18, 9, Brushes.DimGray);

			// X ticks
			if (nTicks <= 0) return;
			int skip = Math.Max(1, nTicks / 10);
			for (int i = 0; i < nTicks; i++)
			{
				if (i % skip != 0) continue;
				double x = plot.X + (nTicks <= 1 ? plot.Width / 2 : (i * plot.Width / (nTicks - 1)));
				c.Children.Add(new Line { X1 = x, Y1 = plot.Y + plot.Height, X2 = x, Y2 = plot.Y + plot.Height + 3, Stroke = Brushes.Black, StrokeThickness = 0.5 });
				AddLabel(c, xLabel(i), x - 14, plot.Y + plot.Height + 5, 9, Brushes.DimGray);
			}
		}

		private static double NiceStep(double range, int target)
		{
			if (range <= 0) return 1;
			double raw = range / target;
			double pow10 = Math.Pow(10, Math.Floor(Math.Log10(raw)));
			double n = raw / pow10;
			return (n < 1.5 ? 1 : n < 3 ? 2 : n < 7 ? 5 : 10) * pow10;
		}

		private static void AddLegendSwatch(Canvas c, double x, double y, Brush color, string text)
		{
			c.Children.Add(new Rectangle { Width = 10, Height = 10, Fill = color });
			var sw = (Rectangle)c.Children[c.Children.Count - 1];
			Canvas.SetLeft(sw, x); Canvas.SetTop(sw, y);
			AddLabel(c, text, x + 14, y - 2, 9, Brushes.Black);
		}

		private static void AddLabel(Canvas c, string text, double x, double y, double fs, Brush fg, bool bold = false)
		{
			var tb = new TextBlock
			{
				Text = text, FontSize = fs, Foreground = fg,
				FontWeight = bold ? FontWeights.SemiBold : FontWeights.Normal,
				FontFamily = new FontFamily("Segoe UI"),
			};
			Canvas.SetLeft(tb, x); Canvas.SetTop(tb, y);
			c.Children.Add(tb);
		}
	}
}
