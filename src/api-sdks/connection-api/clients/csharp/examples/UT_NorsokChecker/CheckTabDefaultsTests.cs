using NorsokChecker.Models;

namespace UT_NorsokChecker
{
	/// <summary>
	/// The Check tab's start-up state, read from the REAL window rather than from a rebuilt copy of
	/// its controls. That distinction is the whole point: a copy would keep passing after someone
	/// edited MainWindow.xaml, which is exactly the change these guard.
	///
	/// Defaults are worth pinning because getting one wrong is silent. Both chapters ticked would
	/// start a CBFEM calculation — minutes of engine time — for a user who only pressed Run to see
	/// what happens, and an untick that flipped back would be noticed only by the clock.
	///
	/// STA is required: this constructs WPF controls.
	/// </summary>
	[TestFixture, Apartment(System.Threading.ApartmentState.STA)]
	public class CheckTabDefaultsTests
	{
		/// <summary>
		/// MainWindow's XAML resolves styles from the APPLICATION's resources (MaterialDesignWindow
		/// and the rest), so without an Application it throws XamlParseException before a single
		/// control exists. Building the real App is what makes this a test of the shipped window
		/// rather than of a stripped-down copy — the resources it loads are the ones the app runs
		/// with. One instance for the whole run: WPF allows only one Application per AppDomain.
		/// </summary>
		[OneTimeSetUp]
		public void EnsureApplication()
		{
			if (System.Windows.Application.Current == null)
			{
				var app = new NorsokChecker.App();
				app.InitializeComponent();   // merges the resource dictionaries App.xaml declares
			}
		}

		private static NorsokChecker.MainWindow NewWindow() => new();

		/// <summary>
		/// Every registered chapter has a toggle, and none of them starts ticked.
		///
		/// Two properties in one test because they fail together: the toggles are built from
		/// ChapterRegistry (see BuildChapterToggles), so a chapter with no toggle could never be run
		/// and a stray toggle would promise a chapter that does not exist.
		///
		/// Paired with <see cref="ActiveLoadEffectsOnlyStartsTicked"/>, which is deliberately ON — a
		/// rig that read "unticked" for every box would pass this one and fail that one.
		/// </summary>
		[Test]
		public void EveryChapterHasAToggleAndNoneStartsTicked()
		{
			var w = NewWindow();
			var boxes = w.ChapterToggles.Children
				.OfType<System.Windows.Controls.CheckBox>()
				.ToList();

			Assert.Multiple(() =>
			{
				Assert.That(boxes, Has.Count.EqualTo(NorsokChecker.Services.Chapters.ChapterRegistry.All.Count),
					"one toggle per registered chapter");
				foreach (var cb in boxes)
					Assert.That(cb.IsChecked, Is.False, $"'{cb.Content}' must start off");
			});
		}

		/// <summary>
		/// The known-good positive of the pair above: this checkbox is deliberately ON, so a test
		/// rig that reported "unticked" for every box would fail here. Without this row, three
		/// identical False assertions could not tell a real default from a broken read.
		/// </summary>
		[Test]
		public void ActiveLoadEffectsOnlyStartsTicked()
		{
			var w = NewWindow();

			Assert.That(w.ChkActiveLoadEffectsOnly.IsChecked, Is.True);
		}

		/// <summary>
		/// The log is diagnostics; expanded it takes height from the tables that carry the result.
		/// </summary>
		[Test]
		public void TheLogStartsCollapsed()
		{
			var w = NewWindow();

			Assert.That(w.LogExpander.IsExpanded, Is.False);
		}

		/// <summary>
		/// API Configuration starts OPEN — it carries the service and the project file, so nothing can
		/// happen until the user has been through it. LoadProject_Click collapses it on success; the
		/// pair of assertions here is what keeps that collapse REVERSIBLE, because the tempting way to
		/// write it (disable or hide the expander once loaded) would strand a user who wants a
		/// different file or service next, and would not be visible in any other test.
		/// </summary>
		[Test]
		public void ApiConfigurationStartsExpandedAndStaysCollapsible()
		{
			var w = NewWindow();

			Assert.Multiple(() =>
			{
				Assert.That(w.ApiConfigExpander.IsExpanded, Is.True,
					"nothing can be loaded before the user has seen these settings");
				Assert.That(w.ApiConfigExpander.IsEnabled, Is.True,
					"collapsing after a load must stay reversible by hand");
				Assert.That(w.ApiConfigExpander.Visibility, Is.EqualTo(System.Windows.Visibility.Visible));
			});
		}

		/// <summary>
		/// Cancel is hidden until a run is in progress — an always-visible disabled button reads as
		/// a broken feature rather than an inapplicable one.
		/// </summary>
		[Test]
		public void CancelIsHiddenAndDisabledBeforeAnyRun()
		{
			var w = NewWindow();

			Assert.Multiple(() =>
			{
				Assert.That(w.BtnCancelCheck.Visibility, Is.EqualTo(System.Windows.Visibility.Collapsed));
				Assert.That(w.BtnCancelCheck.IsEnabled, Is.False);
			});
		}

		/// <summary>
		/// The connections table: the assess-me checkbox first, then "Name", and the active/total
		/// load effect count somewhere after. Read off the real grid so a renamed or dropped column
		/// fails here.
		/// </summary>
		[Test]
		public void TheConnectionsGridHasTheSelectionNameAndLoadEffectColumns()
		{
			var w = NewWindow();

			var cols = w.ConnectionsGrid.Columns;
			var headers = cols.Select(c => c.Header?.ToString()).ToList();

			Assert.Multiple(() =>
			{
				Assert.That(cols[0], Is.TypeOf<System.Windows.Controls.DataGridTemplateColumn>(),
					"the assess-me checkbox comes first");
				Assert.That(headers[1], Is.EqualTo("Name"), "the connection's name follows it");
				Assert.That(headers, Has.Member("Active LC / Total"));
				Assert.That(headers, Has.No.Member("Connection"), "the old header must be gone");
			});
		}

		/// <summary>
		/// The §6.4 table's column GROUPS are named by a spanning banner above the grid, and their
		/// boundaries are drawn more strongly than the ordinary column dividers.
		///
		/// The python table gets both from a real colspan header row over a fully bordered table.
		/// WPF's DataGrid has no colspan, so the banner is a separate Canvas kept aligned to the
		/// columns in code, and the group boundary is a rule on the first column of each group —
		/// on its CELLS as well as its header, or the division stops at the header row.
		///
		/// Two things have to hold together, which is why they are one test: a banner over columns
		/// with no boundary rules is a heading nothing supports, and rules with no banner divide the
		/// table into groups it never names.
		/// </summary>
		[Test]
		public void TheColumnGroupsAreNamedAndVisiblyDivided()
		{
			var w = NewWindow();
			var byHeader = w.Grid64.Columns
				.Where(c => c.Header is string)
				.ToDictionary(c => (string)c.Header, c => c);

			// the first column of each group — the one that carries the boundary rule
			string[] groupStarts = { "K", "N_Rd", "axial", "utilisation" };

			Assert.Multiple(() =>
			{
				Assert.That(w.Group64Band, Is.Not.Null, "the group banner exists");

				foreach (string header in groupStarts)
				{
					Assert.That(byHeader.ContainsKey(header), Is.True, $"column '{header}' is present");
					if (!byHeader.TryGetValue(header, out var col)) continue;
					Assert.That(col.HeaderStyle, Is.Not.Null, $"'{header}' header carries a style");
					Assert.That(col.CellStyle, Is.Not.Null,
						$"'{header}' must style its CELLS too, or the rule stops at the header");
				}

				// a column INSIDE a group carries no boundary rule of its own — the ordinary vertical
				// grid line divides it from its neighbour, and a second rule would make every column
				// look like a group start
				Assert.That(byHeader["X"].CellStyle, Is.Null,
					"a column inside a group draws no group rule of its own");
			});
		}

		/// <summary>
		/// The members table's first column is "Name" too — the two tables are peers, so they name
		/// the same thing the same way.
		/// </summary>
		[Test]
		public void TheMembersGridNamesItsFirstColumnName()
		{
			var w = NewWindow();

			Assert.That(w.MembersGrid.Columns[0].Header?.ToString(), Is.EqualTo("Name"));
		}

		/// <summary>
		/// None of the three §6.4 result tables may be sortable. Two independent reasons, and the
		/// first is a correctness one: the classification table interleaves "K via &lt;partner&gt;"
		/// sub-rows that belong to the brace row directly above, so ANY reorder re-parents them onto
		/// a different brace while still reading as a valid table. The second is that it never worked
		/// — each ItemsSource is a plain List, so a header click drew a sort arrow and moved nothing,
		/// which is worse than no affordance at all.
		///
		/// Asserted on the real window, per grid by name, so re-enabling one fails here.
		/// </summary>
		[Test]
		public void The64ResultTablesAreNotSortable()
		{
			var w = NewWindow();

			Assert.Multiple(() =>
			{
				Assert.That(w.Grid64.CanUserSortColumns, Is.False,
					"sorting re-parents the K sub-rows onto the wrong brace");
				Assert.That(w.Grid64BraceForces.CanUserSortColumns, Is.False,
					"row order is the joint's member order, read against the 3D view");
				Assert.That(w.Grid64Equilibrium.CanUserSortColumns, Is.False,
					"two fixed rows, nothing to sort");
			});
		}

		/// <summary>
		/// All three §6.4 tables draw vertical column dividers. They are dense numeric tables read
		/// across a row — eleven columns of forces, or eleven of resistances and shares — and with
		/// horizontal lines only, a value cannot be tied back to its column heading by eye.
		/// </summary>
		[Test]
		public void The64TablesDrawVerticalColumnDividers()
		{
			var w = NewWindow();

			Assert.Multiple(() =>
			{
				foreach (var (name, grid) in new[]
				{
					("classification", w.Grid64),
					("brace forces", w.Grid64BraceForces),
					("node equilibrium", w.Grid64Equilibrium),
				})
				{
					Assert.That(grid.GridLinesVisibility,
						Is.EqualTo(System.Windows.Controls.DataGridGridLinesVisibility.All),
						$"the {name} table must divide its columns");
					Assert.That(grid.VerticalGridLinesBrush, Is.Not.Null,
						$"the {name} table's dividers need a colour");
				}
			});
		}

		/// <summary>
		/// The group banner must span exactly the columns it names. The spans are column INDICES, so
		/// this is the test that keeps them honest: inserting or reordering a column silently shifts
		/// every group one place, and a banner over the wrong columns is a table that lies about what
		/// its numbers are.
		///
		/// Checked by asserting the header at each end of every span, which is the fact the indices
		/// are supposed to encode. Reading the indices off the XAML by eye is not enough — the source
		/// nests a template column inside the Brace column, so the file order is one out from the
		/// grid's real DisplayIndex order.
		/// </summary>
		[Test]
		public void TheGroupBandSpansTheRightColumns()
		{
			var w = NewWindow();
			var headers = w.Grid64.Columns
				.OrderBy(c => c.DisplayIndex)
				.Select(c => c.Header?.ToString() ?? "")
				.ToList();

			Assert.Multiple(() =>
			{
				Assert.That(headers[2], Is.EqualTo("K"), "Classification starts at K");
				Assert.That(headers[4], Is.EqualTo("Y"), "Classification ends at Y");
				Assert.That(headers[5], Is.EqualTo("N_Rd"), "Resistance starts at N_Rd");
				Assert.That(headers[7], Is.EqualTo("M_z,Rd"), "Resistance ends at M_z,Rd");
				Assert.That(headers[8], Is.EqualTo("axial"), "Utilisation breakdown starts at axial");
				Assert.That(headers[10], Is.EqualTo("out-of-plane"), "…and ends at out-of-plane");
				Assert.That(headers[11], Is.EqualTo("utilisation"), "Check starts at utilisation");
				Assert.That(headers[13], Is.EqualTo("Verdict"), "Check ends at Verdict");

				// the group name must NOT also be in the column header — that was the old two-line
				// form, which read as a label for the group's first column rather than for the group
				foreach (var h in headers)
					Assert.That(h, Does.Not.Contain("Classification").And.Not.Contain("Resistance"),
						$"'{h}' still carries a group name");
			});
		}

		/// <summary>
		/// The group banner stays aligned when a column is HIDDEN.
		///
		/// A real defect, seen in per-LC mode: "Governing LC" is collapsed there, and a hidden
		/// DataGridColumn keeps the ActualWidth it had while visible — so summing widths blindly
		/// shifted every group cell 90 px right, putting each heading over the next group's numbers.
		/// A banner that names the wrong columns is worse than no banner.
		///
		/// Driven through the real window with the column collapsed, and the expected positions
		/// computed the same way the eye checks them: from the VISIBLE columns only.
		///
		/// STA: constructs WPF controls.
		/// </summary>
		[Test]
		public void TheGroupBannerFollowsAHiddenColumn()
		{
			var w = NewWindow();
			// Shown off-screen: a DataGrid's columns have no ActualWidth until the window is rendered,
			// and the banner is computed from those widths — without this the band comes back empty
			// and the test measures nothing.
			w.Left = -10000;
			w.Top = -10000;
			w.Show();

			w.Tab64.IsEnabled = true;
			w.MainTabs.SelectedItem = w.Tab64;
			w.Col64Gov.Visibility = System.Windows.Visibility.Collapsed;   // per-LC mode
			w.UpdateLayout();
			Pump();
			w.UpdateLayout();          // the banner is drawn from a LayoutUpdated handler
			Pump();

			var cols = w.Grid64.Columns.OrderBy(c => c.DisplayIndex).ToList();
			var edges = new double[cols.Count + 1];
			for (int i = 0; i < cols.Count; i++)
				edges[i + 1] = edges[i]
					+ (cols[i].Visibility == System.Windows.Visibility.Visible ? cols[i].ActualWidth : 0.0);

			var cells = w.Group64Band.Children
				.OfType<System.Windows.Controls.Border>()
				.ToList();

			// (label, first column, last column) — the spans the banner draws
			var spans = new[] { ("Classification", 2, 4), ("Resistance", 5, 7),
				("Utilisation breakdown", 8, 10), ("Check", 11, 13) };

			Assert.That(cells, Has.Count.EqualTo(spans.Length), "one cell per group");
			Assert.Multiple(() =>
			{
				for (int i = 0; i < spans.Length; i++)
				{
					var (label, first, last) = spans[i];
					double x = System.Windows.Controls.Canvas.GetLeft(cells[i]);
					Assert.That(x, Is.EqualTo(edges[first]).Within(0.6),
						$"'{label}' must start at its first column");
					Assert.That(cells[i].Width, Is.EqualTo(edges[last + 1] - edges[first]).Within(0.6),
						$"'{label}' must span exactly its columns");
				}
			});
		}

		/// <summary>
		/// Everything in the rejection banner starts at the same left edge.
		///
		/// The explanatory paragraph has a MaxWidth so long prose does not run the full width of a
		/// wide window — but a StackPanel child defaults to Stretch, and a stretched child that cannot
		/// fill its space is CENTRED. The paragraph therefore sat indented while the title above it
		/// and the condition list below it began at the left edge, which reads as a different kind of
		/// content rather than as one message.
		///
		/// Asserted on the alignment rather than on a measured x, so it holds at any window width.
		/// </summary>
		[Test]
		public void TheRejectionBannerIsLeftAligned()
		{
			var w = NewWindow();

			Assert.Multiple(() =>
			{
				Assert.That(w.Lbl64VerdictBody.HorizontalAlignment,
					Is.EqualTo(System.Windows.HorizontalAlignment.Left),
					"a MaxWidth block must be pinned left, or WPF centres it");
				// the control: these two have no MaxWidth, so Stretch already puts them at the left
				Assert.That(w.Lbl64VerdictTitle.MaxWidth, Is.EqualTo(double.PositiveInfinity),
					"the title fills the panel, so it needs no alignment of its own");
				Assert.That(w.Lst64Conditions.MaxWidth, Is.EqualTo(double.PositiveInfinity),
					"and so does the condition list");
			});
		}

		/// <summary>Let WPF finish the layout pass the banner is drawn from.</summary>
		private static void Pump()
		{
			var frame = new System.Windows.Threading.DispatcherFrame();
			System.Windows.Threading.Dispatcher.CurrentDispatcher.BeginInvoke(
				System.Windows.Threading.DispatcherPriority.Background,
				new Action(() => frame.Continue = false));
			System.Windows.Threading.Dispatcher.PushFrame(frame);
		}

		/// <summary>
		/// The legend has one swatch per band, in the scale's own colours — it IS the scale, drawn.
		/// Read off the real window, so a legend built from a hardcoded count (as the four hand-written
		/// swatches were) fails here instead of quietly describing a ramp the app no longer uses.
		/// </summary>
		[Test]
		public void TheLegendHasOneSwatchPerBand()
		{
			var w = NewWindow();
			var swatches = w.Legend64Swatches.Children
				.OfType<System.Windows.Controls.Border>()
				.ToList();

			Assert.Multiple(() =>
			{
				Assert.That(swatches.Count, Is.EqualTo(NorsokChecker.Models.UtilisationScale.BandCount),
					"one swatch per band, over-capacity included");

				for (int i = 0; i < swatches.Count; i++)
				{
					var expected = NorsokChecker.Models.UtilisationScale.Parse(
						NorsokChecker.Models.UtilisationScale.HexOfBand(i));
					var actual = (swatches[i].Background as System.Windows.Media.SolidColorBrush)?.Color;
					Assert.That(actual, Is.EqualTo(expected), $"swatch {i} must be its band's colour");
				}
			});
		}

		/// <summary>
		/// The brace-force table's shear columns are bound the way the frame requires: V_z to the
		/// IN-plane component, V_y to the out-of-plane one.
		///
		/// The engine's property names are ip/op while the headers are the frame's axes, and the two
		/// vocabularies CROSS OVER for the shears: a moment's index is its axis of ROTATION, a
		/// force's its direction of ACTION, so the in-plane force (Vip) sits on the z axis. The
		/// moments do not cross over — M_y is in-plane bending, i.e. Mip.
		///
		/// Nothing else in the suite noticed when that pairing was wrong: measured, swapping the two
		/// bindings left every other test green. BraceFrameTests covers the frame itself.
		/// </summary>
		[Test]
		public void TheBraceForceShearColumnsAreBoundToTheMatchingComponents()
		{
			var w = NewWindow();

			string? PathOf(string header) => w.Grid64BraceForces.Columns
				.OfType<System.Windows.Controls.DataGridTextColumn>()
				.Where(c => (c.Header?.ToString() ?? "").StartsWith(header, StringComparison.Ordinal))
				.Select(c => (c.Binding as System.Windows.Data.Binding)?.Path?.Path)
				.FirstOrDefault();

			Assert.Multiple(() =>
			{
				Assert.That(PathOf("V_z"), Is.EqualTo("Vip"),
					"V_z is the force IN the joint plane, which the engine calls Vip");
				Assert.That(PathOf("V_y"), Is.EqualTo("Vop"),
					"V_y is the force OUT of the plane, which the engine calls Vop");

				Assert.That(PathOf("M_y ["), Is.EqualTo("Mip"), "M_y is in-plane bending");
				Assert.That(PathOf("M_z ["), Is.EqualTo("Mop"), "M_z is out-of-plane bending");
			});
		}

		/// <summary>
		/// Every §6.4 header with a subscript in it gets the typesetting style.
		///
		/// Without it WPF eats the underscore (it reads as an access-key marker), which is how the
		/// brace-force table came to show "NSd" and "My" while the classification table showed
		/// "N_Rd" and "M_y,Rd" — two notations on one screen. SubscriptHeaderTests covers what the
		/// converter produces; this covers that the columns actually USE it, which is the half a
		/// correct-but-unwired converter would pass.
		/// </summary>
		[Test]
		public void EveryHeaderWithASubscriptIsTypeset()
		{
			var w = NewWindow();

			var untyped = new List<string>();
			foreach (var grid in new[] { w.Grid64BraceForces, w.Grid64, w.MembersGrid })
				foreach (var c in grid.Columns)
				{
					string header = c.Header?.ToString() ?? "";
					// A subscript is what needs typesetting; "brace", "face", "N [kN]" do not.
					if (!header.Contains('_')) continue;
					if (c.HeaderStyle == null) untyped.Add($"{header} (in {grid.Name})");
				}

			Assert.That(untyped, Is.Empty,
				"these headers carry a subscript but no HeaderStyle, so WPF will swallow the "
				+ "underscore and print e.g. 'MyRd':\n  " + string.Join("\n  ", untyped));
		}
	}

	/// <summary>
	/// The per-connection "assess this one" checkbox.
	///
	/// Four of the five tests here were removed: they restated `RunCheck_Click`'s own
	/// `Where(c => c.Selected)` inside the test and then asserted the restatement, so the mutation
	/// that matters — iterating `_connections` instead of the ticked ones — left every one of them
	/// green. A fifth pinned the pairing of `calcResults` to rows by Id, for a call that no longer
	/// exists; that rule now lives in `Services/Cbfem_Mothballed/README.md`, where whoever revives
	/// the calculate path will meet it.
	///
	/// What remains is the one fact about the MODEL rather than about the run.
	/// </summary>
	[TestFixture]
	public class ConnectionSelectionTests
	{
		/// <summary>
		/// Opening a project and pressing Run means "check this project": a connection is selected
		/// unless someone unticks it. The default lives on the model, so the grid, the run and the
		/// report all inherit it — a false default here would silently skip connections.
		/// </summary>
		[Test]
		public void EveryConnectionStartsSelected()
		{
			var project = new List<ConnectionCheckResult>
			{
				new() { Id = 1, Name = "CON1" },
				new() { Id = 2, Name = "CON2" },
			};

			Assert.That(project.All(c => c.Selected), Is.True);
		}
	}

	/// <summary>
	/// The active/total load-effect cell. Its one real decision is what to show before the counts
	/// have been read: an unread count is not the same as a connection with no load effects, and
	/// "0 / 0" would state the latter.
	/// </summary>
	[TestFixture]
	public class LoadEffectCountDisplayTests
	{
		[Test]
		public void CountsAreShownAsActiveOverTotal()
		{
			var con = new ConnectionCheckResult { TotalLoadEffects = 15, ActiveLoadEffects = 4 };

			Assert.That(con.LoadEffectsDisplay, Is.EqualTo("4 / 15"));
		}

		/// <summary>Nothing read yet — an em dash, never "0 / 0".</summary>
		[Test]
		public void UnreadCountsShowAnEmDash()
		{
			var con = new ConnectionCheckResult();

			Assert.That(con.LoadEffectsDisplay, Is.EqualTo("—"));
		}

		/// <summary>
		/// A connection that genuinely has none says so — this is what "0 / 0" is reserved for, and
		/// it is the row that proves the em dash above is about UNREAD counts, not about zero.
		/// </summary>
		[Test]
		public void AConnectionWithNoLoadEffectsShowsZeroOverZero()
		{
			var con = new ConnectionCheckResult { TotalLoadEffects = 0, ActiveLoadEffects = 0 };

			Assert.That(con.LoadEffectsDisplay, Is.EqualTo("0 / 0"));
		}

		/// <summary>
		/// A half-read pair still counts as unread. This is reachable: the counts are two separate
		/// properties, so a failure between them leaves one set.
		/// </summary>
		[Test]
		public void AHalfReadPairIsStillUnread()
		{
			var con = new ConnectionCheckResult { TotalLoadEffects = 15 };

			Assert.That(con.LoadEffectsDisplay, Is.EqualTo("—"));
		}

		/// <summary>The cell refreshes when either count arrives — they are set one after the other.</summary>
		[Test]
		public void SettingEitherCountRaisesTheDisplayChange()
		{
			var con = new ConnectionCheckResult();
			var raised = new List<string>();
			con.PropertyChanged += (_, e) => raised.Add(e.PropertyName ?? "");

			con.TotalLoadEffects = 15;
			con.ActiveLoadEffects = 4;

			Assert.That(raised.Count(p => p == nameof(ConnectionCheckResult.LoadEffectsDisplay)), Is.EqualTo(2));
		}

	}
}
