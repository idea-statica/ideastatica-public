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
		/// The connections table names its first column "Name", and carries the active/total load
		/// effect count. Read off the real grid so a renamed or dropped column fails here.
		/// </summary>
		[Test]
		public void TheConnectionsGridHasNameAndLoadEffectColumns()
		{
			var w = NewWindow();

			var headers = w.ConnectionsGrid.Columns.Select(c => c.Header?.ToString()).ToList();

			Assert.Multiple(() =>
			{
				Assert.That(headers[0], Is.EqualTo("Name"), "first column is the connection's name");
				Assert.That(headers, Has.Member("Active LC / Total"));
				Assert.That(headers, Has.No.Member("Connection"), "the old header must be gone");
			});
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
