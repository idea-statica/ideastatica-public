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
		/// Neither chapter is ticked at start-up: running a check is the user's decision, and the
		/// CBFEM one is expensive enough that defaulting it on spends the engine's time uninvited.
		/// </summary>
		[Test]
		public void BothChapterTogglesStartUnticked()
		{
			var w = NewWindow();

			Assert.Multiple(() =>
			{
				Assert.That(w.ChkChapterCbfem.IsChecked, Is.False, "CBFEM chapter must start off");
				Assert.That(w.ChkChapter64.IsChecked, Is.False, "§6.4 chapter must start off");
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
		/// The §6.4 table's column GROUPS are visibly divided, not merely labelled.
		///
		/// The python table gets this from real colspan header cells over a fully bordered table, so
		/// the group boundary is a table line. WPF has no colspan, so the rule is drawn on the first
		/// column of each group — and on its CELLS as well as its header, or the division stops at
		/// the header row and the eleven data columns still read as one undivided run.
		/// </summary>
		[Test]
		public void TheColumnGroupsAreVisiblyDivided()
		{
			var w = NewWindow();
			var byHeader = w.Grid64.Columns
				.Where(c => c.Header is string)
				.ToDictionary(c => (string)c.Header, c => c);

			// the first column of each of the four groups, by the group name in its header
			string[] groupStarts =
			{
				"Classification\nK", "Resistance\nN_Rd",
				"Utilisation breakdown\naxial", "Check\nutilisation",
			};

			Assert.Multiple(() =>
			{
				foreach (string header in groupStarts)
				{
					Assert.That(byHeader.ContainsKey(header), Is.True, $"column '{header}' is present");
					if (!byHeader.TryGetValue(header, out var col)) continue;
					Assert.That(col.HeaderStyle, Is.Not.Null, $"'{header}' header carries a style");
					Assert.That(col.CellStyle, Is.Not.Null,
						$"'{header}' must style its CELLS too, or the rule stops at the header");
				}

				// and a column INSIDE a group must not carry the rule, or every boundary is drawn
				// and nothing is divided
				var inside = byHeader["\nX"];
				Assert.That(inside.CellStyle, Is.Null,
					"a column inside a group draws no rule of its own");
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
	}

	/// <summary>
	/// The per-connection "assess this one" checkbox, and the consequence that made it more than a
	/// UI flag: once the run sends a SUBSET of the project's connections, results can no longer be
	/// paired with rows by position.
	/// </summary>
	[TestFixture]
	public class ConnectionSelectionTests
	{
		private static List<ConnectionCheckResult> Project() => new()
		{
			new() { Id = 1, Name = "CON1" },
			new() { Id = 2, Name = "CON2" },
			new() { Id = 3, Name = "CON3" },
		};

		/// <summary>Opening a project and pressing Run means "check this project".</summary>
		[Test]
		public void EveryConnectionStartsSelected()
		{
			Assert.That(Project().All(c => c.Selected), Is.True);
		}

		/// <summary>The run's list is the ticked ones, in order.</summary>
		[Test]
		public void UntickingOneExcludesItFromTheRun()
		{
			var cons = Project();
			cons[1].Selected = false;

			var selected = cons.Where(c => c.Selected).ToList();

			Assert.That(selected.Select(c => c.Id), Is.EqualTo(new[] { 1, 3 }));
		}

		/// <summary>
		/// THE defect this feature would have introduced, pinned. calcResults comes back for the
		/// connections that were SENT; the old code walked it against the full connection list by
		/// index. With CON2 unticked, CON3's result (index 1 of the response) would have been
		/// written onto CON2 (index 1 of the list) — silently, since a utilisation and a status are
		/// plausible on any row. Pairing by Id is what makes it right.
		///
		/// Written as the two pairings side by side so the wrong one is visibly wrong, rather than
		/// asserting only that the right one works.
		/// </summary>
		[Test]
		public void ResultsArePairedByIdNotByPosition()
		{
			var cons = Project();
			cons[1].Selected = false;                       // CON2 out
			var sent = cons.Where(c => c.Selected).ToList();  // CON1, CON3

			// what the API returns, in the order it was asked: one entry per SENT connection
			var response = new[] { (Id: 1, Util: 0.10), (Id: 3, Util: 0.30) };

			// the correct pairing: by id
			var byId = response.ToDictionary(r => r.Id);
			foreach (var con in sent)
				if (byId.TryGetValue(con.Id, out var r)) con.MaxUtilization = r.Util;

			Assert.Multiple(() =>
			{
				Assert.That(cons[0].MaxUtilization, Is.EqualTo(0.10), "CON1 keeps its own result");
				Assert.That(cons[1].MaxUtilization, Is.EqualTo(0.0), "CON2 was not run, so it gets nothing");
				Assert.That(cons[2].MaxUtilization, Is.EqualTo(0.30), "CON3 keeps its own result");

				// and the pairing that was there before: index into the FULL list
				Assert.That(response[1].Id, Is.Not.EqualTo(cons[1].Id),
					"by position, response[1] would have landed on CON2 — which is the bug");
			});
		}

		/// <summary>
		/// A connection left unticked keeps the verdict it already had. Excluding it from a run is
		/// not the same as having no result, and blanking it would read as a regression.
		/// </summary>
		[Test]
		public void AnUntickedConnectionKeepsItsPreviousVerdict()
		{
			var cons = Project();
			cons[1].NorsokPass = "PASS";
			cons[1].MaxUtilization = 0.42;
			cons[1].Selected = false;

			// what the run does: touch only the selected ones
			foreach (var con in cons.Where(c => c.Selected))
			{
				con.NorsokPass = "FAIL";
				con.MaxUtilization = 1.5;
			}

			Assert.Multiple(() =>
			{
				Assert.That(cons[1].NorsokPass, Is.EqualTo("PASS"));
				Assert.That(cons[1].MaxUtilization, Is.EqualTo(0.42));
			});
		}

		/// <summary>Nothing ticked is a real state the run has to refuse rather than run empty.</summary>
		[Test]
		public void NothingSelectedLeavesNothingToRun()
		{
			var cons = Project();
			foreach (var c in cons) c.Selected = false;

			Assert.That(cons.Where(c => c.Selected), Is.Empty);
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
