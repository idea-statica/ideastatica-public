using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using NorsokChecker.Models;

namespace UT_NorsokChecker
{
	/// <summary>
	/// A MEASUREMENT rig, not a guard: it prints what the real window's tables need so the start-up
	/// sizes can be set from measurement instead of arithmetic. Row height comes from the Material
	/// Design row style and column widths from SizeToHeader — neither is knowable by adding up the
	/// MinWidths in the XAML, which is what made the first guesses at these numbers wrong.
	///
	/// Explicit so it never runs in the normal suite: it asserts almost nothing, its output is for a
	/// human, and it shows a window. Run with --filter TestCategory=Probe.
	///
	/// The category alone did NOT achieve that — a plain `dotnet test` ran it for months while this
	/// remark claimed otherwise. A category selects; only Explicit deselects.
	/// </summary>
	[TestFixture, Category("Probe"), Explicit("Shows a real window; measures rather than asserts")]
	[Apartment(System.Threading.ApartmentState.STA)]
	public class LayoutMeasurementProbe
	{
		[OneTimeSetUp]
		public void EnsureApplication()
		{
			if (Application.Current == null)
			{
				var app = new NorsokChecker.App();
				app.InitializeComponent();
			}
		}

		/// <summary>
		/// Lays the window out off-screen at its real start-up size, then reports the height the
		/// members table needs for 7 rows and the width the brace-force table needs for its columns.
		/// </summary>
		[Test]
		public void ReportRequiredSizes()
		{
			var w = new NorsokChecker.MainWindow();

			// 7 members, per the sizing target
			var members = new List<MemberDisplayInfo>();
			for (int i = 0; i < 7; i++)
				members.Add(new MemberDisplayInfo
				{
					Name = $"Member {i + 1}",
					Role = i == 0 ? "chord" : "brace",
					Profile = "CHS 219.1/10.0",
					Shape = "rolledCHS",
					Diameter = 219.1,
					WallThickness = 10.0,
					Fy = 355,
					MaterialName = "S 355",
				});
			w.MembersGrid.ItemsSource = members;

			// Show off-screen: a DataGrid virtualises, so nothing has a size until it is rendered.
			w.Left = -10000;
			w.Top = -10000;
			w.Show();
			w.UpdateLayout();
			DoEvents();

			var grid = w.MembersGrid;
			double header = 0, rowH = 0;
			int counted = 0;
			for (int i = 0; i < grid.Items.Count; i++)
			{
				if (grid.ItemContainerGenerator.ContainerFromIndex(i) is DataGridRow row && row.ActualHeight > 0)
				{
					rowH = Math.Max(rowH, row.ActualHeight);
					counted++;
				}
			}
			var presenter = FindChild<DataGridColumnHeadersPresenter>(grid);
			if (presenter != null) header = presenter.ActualHeight;

			TestContext.Out.WriteLine("=== MEMBERS TABLE (Check tab) ===");
			TestContext.Out.WriteLine($"window            : {w.ActualWidth} x {w.ActualHeight}");
			TestContext.Out.WriteLine($"rows realised     : {counted} of {grid.Items.Count}");
			TestContext.Out.WriteLine($"row height        : {rowH}");
			TestContext.Out.WriteLine($"col header height : {header}");
			TestContext.Out.WriteLine($"grid ActualHeight : {grid.ActualHeight}");
			TestContext.Out.WriteLine($"7 rows + header   : {header + 7 * rowH}");

			// The members grid sits inside an Expander inside a Grid row; report the chrome between
			// the grid and that row so the row height can be set from it.
			double chrome = ChromeAbove(grid, w);
			TestContext.Out.WriteLine($"expander+margin chrome: {chrome}");
			TestContext.Out.WriteLine($"=> members ROW needs  : {header + 7 * rowH + chrome}");

			// The derived chrome above is a sum over ancestors and easy to get wrong. Measure it
			// DIRECTLY instead: the Grid row the members expander lives in, minus the grid itself.
			var membersExpander = AncestorOfType<Expander>(grid);
			if (membersExpander != null)
			{
				TestContext.Out.WriteLine($"members expander H: {membersExpander.ActualHeight}");
				TestContext.Out.WriteLine($"  -> measured chrome (expander - grid): {membersExpander.ActualHeight - grid.ActualHeight}");
				TestContext.Out.WriteLine($"  -> row needed for 7 rows: {header + 7 * rowH + (membersExpander.ActualHeight - grid.ActualHeight)}");
			}

			// The band above (connections + 3D) and the whole tab, so the split can be reasoned about.
			TestContext.Out.WriteLine($"TopBand H         : {w.TopBand.ActualHeight}");
			TestContext.Out.WriteLine($"ConnectionsGrid H : {w.ConnectionsGrid.ActualHeight}");

			// ---- brace forces table width ----
			// The 6.4 panels start Collapsed and the tab is not selected, so nothing on it has a size
			// yet. Select the tab and show the panels, exactly as the per-LC mode does.
			w.Tab64.IsEnabled = true;              // disabled until a check has run
			w.MainTabs.SelectedItem = w.Tab64;
			w.Pnl64BraceForces.Visibility = Visibility.Visible;
			w.Pnl64Equilibrium.Visibility = Visibility.Visible;
			w.Pnl64EnvNote.Visibility = Visibility.Visible;
			w.UpdateLayout();
			DoEvents();

			// the classification grid's real column order — what the group banner spans index into
			TestContext.Out.WriteLine();
			TestContext.Out.WriteLine("=== Grid64 COLUMNS (DisplayIndex: header) ===");
			foreach (var c in w.Grid64.Columns.OrderBy(c => c.DisplayIndex))
				TestContext.Out.WriteLine($"  {c.DisplayIndex}: '{c.Header}'");

			// ---- does the group banner actually line up with those columns? ----
			// The banner is positioned in code, so this is the only place the alignment is checked
			// against the real widths rather than against the intent.
			w.Grid64.ItemsSource = Enumerable.Range(0, 4).Select(i => new NorsokChecker.Models.Joint64RowView
			{
				Brace = $"M{i + 1}", Actions = "N_Sd=-10.0 kN", FrK = "0 %", FrX = "100 %", FrY = "0 %",
				NRd = "149 kN", MRdIp = "6.9 kNm", MRdOp = "4.4 kNm",
				UtilAxial = "6.7 %", UtilIpb = "2.1 %", UtilOpb = "0.0 %",
				Util = "8.8 %", UtilValue = 0.088, Verdict = "PASS",
			}).ToList();
			w.UpdateLayout();
			DoEvents();
			w.UpdateLayout();          // the banner is drawn from a LayoutUpdated handler
			DoEvents();

			// BOTH modes: per-LC collapses the "Governing LC" column, and a hidden column keeps the
			// ActualWidth it had while visible — which is what put the banner one column off.
			foreach (bool envelope in new[] { true, false })
			{
				w.Rb64Envelope.IsChecked = envelope;
				w.Rb64PerLe.IsChecked = !envelope;
				w.Col64Gov.Visibility = envelope ? Visibility.Visible : Visibility.Collapsed;
				w.UpdateLayout();
				DoEvents();
				w.UpdateLayout();      // the banner is drawn from a LayoutUpdated handler
				DoEvents();

				var cols = w.Grid64.Columns.OrderBy(c => c.DisplayIndex).ToList();
				var edges = new double[cols.Count + 1];
				for (int i = 0; i < cols.Count; i++)
					edges[i + 1] = edges[i]
						+ (cols[i].Visibility == Visibility.Visible ? cols[i].ActualWidth : 0.0);

				TestContext.Out.WriteLine();
				TestContext.Out.WriteLine($"=== GROUP BANNER alignment — {(envelope ? "ENVELOPE" : "per LC")} ===");
				var cells = w.Group64Band.Children.OfType<System.Windows.Controls.Border>().ToList();
				TestContext.Out.WriteLine($"banner cells      : {cells.Count}");
				TestContext.Out.WriteLine($"Governing LC      : {w.Col64Gov.Visibility}, "
					+ $"ActualWidth={w.Col64Gov.ActualWidth:F1}");
				var spans = new[] { ("Classification", 2, 4), ("Resistance", 5, 7),
					("Utilisation breakdown", 8, 10), ("Check", 11, 13) };
				for (int i = 0; i < cells.Count && i < spans.Length; i++)
				{
					var (label, first, last) = spans[i];
					double wantX = edges[first], wantW = edges[last + 1] - edges[first];
					double gotX = System.Windows.Controls.Canvas.GetLeft(cells[i]);
					double gotW = cells[i].Width;
					string text = (cells[i].Child as TextBlock)?.Text ?? "?";
					bool ok = Math.Abs(gotX - wantX) < 0.6 && Math.Abs(gotW - wantW) < 0.6;
					TestContext.Out.WriteLine(
						$"  '{text}': x={gotX:F1} (want {wantX:F1}) w={gotW:F1} (want {wantW:F1})  "
						+ (ok ? "ALIGNED" : "*** MISALIGNED ***")
						+ (text == label ? "" : $"  *** LABEL MISMATCH, expected '{label}' ***"));
				}
			}

			var bf = w.Grid64BraceForces;
			TestContext.Out.WriteLine();
			TestContext.Out.WriteLine("=== BRACE FORCES (6.4) ===");
			double sumMin = 0;
			foreach (var c in bf.Columns)
			{
				TestContext.Out.WriteLine($"  {c.Header,-20} MinWidth={c.MinWidth,6} Actual={c.ActualWidth,7}");
				sumMin += c.MinWidth;
			}
			TestContext.Out.WriteLine($"columns           : {bf.Columns.Count}");
			TestContext.Out.WriteLine($"sum of MinWidth   : {sumMin}");

			// What the table's own container reports it wants — the number that decides whether a
			// horizontal scrollbar appears. Measured on the REAL grid with rows in it, because
			// SizeToHeader columns size to their content and the sum of MinWidths is only a floor.
			bf.ItemsSource = Enumerable.Range(0, 6).Select(i => new
			{
				Brace = $"B{i + 1}", NSd = "-1234.5", Mip = "-123.4", Mop = "-123.4",
				Vip = "-99.9", Vop = "-99.9", Mtor = "-12.3", Face = "tension",
				NChord = "-2345.6", MipChord = "-234.5", MopChord = "-234.5",
			}).ToList();
			w.UpdateLayout();
			DoEvents();
			bf.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
			TestContext.Out.WriteLine($"desired width (6 rows): {bf.DesiredSize.Width}");
			double actualSum = bf.Columns.Sum(c => c.ActualWidth);
			TestContext.Out.WriteLine($"sum of ActualWidth    : {actualSum}");

			// the panel that has to hold it, and the joint view it competes with for width
			TestContext.Out.WriteLine($"right column width    : {w.Pnl64BraceForces.ActualWidth}");
			TestContext.Out.WriteLine($"joint view width      : {w.Joint3D64.ActualWidth}");
			TestContext.Out.WriteLine($"h-scrollbar needed?   : {(bf.DesiredSize.Width > w.Pnl64BraceForces.ActualWidth - 24 ? "YES - TOO NARROW" : "no")}");

			// ---- 6.4 top-band height: does the whole brace-force panel fit for 6 braces? ----
			TestContext.Out.WriteLine();
			TestContext.Out.WriteLine("=== 6.4 HEIGHT (per-LC mode, 6 braces) ===");
			w.Pnl64EnvNote.Visibility = Visibility.Collapsed;   // per-LC hides the envelope note
			w.Grid64Equilibrium.ItemsSource = Enumerable.Range(0, 2).Select(i => new
			{
				Quantity = i == 0 ? "force [kN]" : "moment [kNm]",
				X = "0.0", Y = "0.0", Z = "0.0", State = "ok",
			}).ToList();
			w.UpdateLayout();
			DoEvents();

			var sheetRow = w.Pnl64Sheet;
			TestContext.Out.WriteLine($"sheet total H         : {sheetRow.ActualHeight}");
			TestContext.Out.WriteLine($"brace panel H         : {w.Pnl64BraceForces.ActualHeight}");
			TestContext.Out.WriteLine($"equilibrium panel H   : {w.Pnl64Equilibrium.ActualHeight}");
			var sv = AncestorOfType<ScrollViewer>(w.Pnl64BraceForces);
			if (sv != null)
			{
				TestContext.Out.WriteLine($"right ScrollViewer H  : {sv.ActualHeight}");
				TestContext.Out.WriteLine($"  content needs       : {sv.ExtentHeight}");
				TestContext.Out.WriteLine($"  v-scrollbar?        : {(sv.ExtentHeight > sv.ViewportHeight + 1 ? "YES - TOO SHORT" : "no")}");
				TestContext.Out.WriteLine($"  shortfall           : {Math.Max(0, sv.ExtentHeight - sv.ViewportHeight)}");
			}

			// ---- envelope mode: only the note is shown, and its text must WRAP, not scroll ----
			TestContext.Out.WriteLine();
			TestContext.Out.WriteLine("=== 6.4 ENVELOPE mode ===");
			w.Pnl64EnvNote.Visibility = Visibility.Visible;
			w.Pnl64BraceForces.Visibility = Visibility.Collapsed;
			w.Pnl64Equilibrium.Visibility = Visibility.Collapsed;
			w.UpdateLayout();
			DoEvents();
			if (sv != null)
			{
				TestContext.Out.WriteLine($"note width            : {w.Pnl64EnvNote.ActualWidth}");
				TestContext.Out.WriteLine($"viewport width        : {sv.ViewportWidth}");
				TestContext.Out.WriteLine($"content width needed  : {sv.ExtentWidth}");
				TestContext.Out.WriteLine($"  h-scrollbar?        : {(sv.ExtentWidth > sv.ViewportWidth + 1 ? "YES - TEXT NOT WRAPPING" : "no")}");
				TestContext.Out.WriteLine($"  v-scrollbar?        : {(sv.ExtentHeight > sv.ViewportHeight + 1 ? "YES" : "no")}");
			}

			// and the members table, now that the row ratio changed
			TestContext.Out.WriteLine();
			TestContext.Out.WriteLine("=== MEMBERS re-check after ratio change ===");
			w.MainTabs.SelectedIndex = 0;
			w.UpdateLayout();
			DoEvents();
			TestContext.Out.WriteLine($"members grid H        : {grid.ActualHeight}");
			TestContext.Out.WriteLine($"needed for 7 rows     : {header + 7 * rowH}");
			TestContext.Out.WriteLine($"  v-scrollbar?        : {(header + 7 * rowH > grid.ActualHeight + 1 ? "YES - TOO SHORT" : "no")}");

			w.Close();
			DoEvents();
			Assert.Pass("measurement only");
		}

		/// <summary>Total top+bottom padding/margin/header between a control and the window's layout root.</summary>
		private static double ChromeAbove(FrameworkElement inner, Window w)
		{
			double extra = 0;
			DependencyObject? d = inner;
			while (d != null && d != w)
			{
				if (d is FrameworkElement fe && fe != inner)
					extra += fe.Margin.Top + fe.Margin.Bottom;
				if (d is Expander ex)
				{
					var hdr = FindChild<ToggleButton>(ex);
					if (hdr != null) extra += hdr.ActualHeight;
				}
				if (d is Border b) extra += b.Padding.Top + b.Padding.Bottom + b.BorderThickness.Top + b.BorderThickness.Bottom;
				d = System.Windows.Media.VisualTreeHelper.GetParent(d);
			}
			return extra;
		}

		private static T? AncestorOfType<T>(DependencyObject start) where T : DependencyObject
		{
			var d = System.Windows.Media.VisualTreeHelper.GetParent(start);
			while (d != null && d is not T)
				d = System.Windows.Media.VisualTreeHelper.GetParent(d);
			return d as T;
		}

		private static T? FindChild<T>(DependencyObject parent) where T : DependencyObject
		{
			int n = System.Windows.Media.VisualTreeHelper.GetChildrenCount(parent);
			for (int i = 0; i < n; i++)
			{
				var c = System.Windows.Media.VisualTreeHelper.GetChild(parent, i);
				if (c is T hit) return hit;
				var deeper = FindChild<T>(c);
				if (deeper != null) return deeper;
			}
			return null;
		}

		private static void DoEvents()
		{
			var frame = new System.Windows.Threading.DispatcherFrame();
			System.Windows.Threading.Dispatcher.CurrentDispatcher.BeginInvoke(
				System.Windows.Threading.DispatcherPriority.Background,
				new Action(() => frame.Continue = false));
			System.Windows.Threading.Dispatcher.PushFrame(frame);
		}
	}
}
