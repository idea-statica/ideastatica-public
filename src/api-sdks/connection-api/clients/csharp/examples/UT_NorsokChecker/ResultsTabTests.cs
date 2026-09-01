using NorsokChecker.Models;

namespace UT_NorsokChecker
{
	/// <summary>
	/// The Results table — the overview of every check, across chapters and connections.
	///
	/// What it shows was wrong in three ways at once, all of them the same mistake: printing a value
	/// where there is none. A row for a joint that was never assessed carried a load case
	/// ("envelope"), a demand and a capacity ("0 / 0"), and a rejected joint filled one row per unmet
	/// condition — seven for CON6, one for CON2 — which reads as seven checks against one rather than
	/// as the same answer twice.
	///
	/// Driven through the real window: the rows are anonymous types built inside PopulateResultsTab,
	/// so there is nothing else to test them through.
	///
	/// STA: constructs WPF controls.
	/// </summary>
	[TestFixture, Apartment(System.Threading.ApartmentState.STA)]
	public class ResultsTabTests
	{
		[OneTimeSetUp]
		public void EnsureApplication()
		{
			if (System.Windows.Application.Current == null)
			{
				var app = new NorsokChecker.App();
				app.InitializeComponent();
			}
		}

		private static NorsokFormulaResult Assessed(double util) => new()
		{
			Section = "6.4.3.6", Equation = "6.57", Title = "Tubular Joint — M1",
			LoadCaseName = "LE8", Utilization = util, Demand = util, Capacity = 1.0, Passed = true,
		};

		private static NorsokFormulaResult Rejected(string condition) => new()
		{
			Section = "6.4", Equation = "6.4.3", Title = "Outside the scope of §6.4",
			CheckExpression = condition, NotAssessed = true,
		};

		/// <summary>Read the grid back as (column → value) dictionaries.</summary>
		private static List<Dictionary<string, string>> RowsOf(
			params (int ConId, string Name, NorsokFormulaResult[] Results)[] connections)
		{
			var w = new NorsokChecker.MainWindow();
			foreach (var (id, name, results) in connections)
				w.SetResultsForTest(id, name, results.ToList());

			var rows = new List<Dictionary<string, string>>();
			foreach (var item in (System.Collections.IEnumerable)w.ResultsGrid.ItemsSource)
			{
				var d = new Dictionary<string, string>();
				foreach (var p in item.GetType().GetProperties())
					d[p.Name] = p.GetValue(item)?.ToString() ?? "";
				rows.Add(d);
			}
			return rows;
		}

		/// <summary>
		/// A joint outside the scope of the chapter is ONE row, however many conditions it failed.
		///
		/// The count belongs in the detail, not in the row count: six rows reading "(1 of 6)" …
		/// "(6 of 6)" put a joint that cannot be checked six times over a joint that passed once.
		/// </summary>
		[Test]
		public void ARejectedJointIsOneRowWhateverItFailed()
		{
			var rows = RowsOf(
				(6, "CON6", new[]
				{
					Rejected("M4-M6: feet overlap"), Rejected("M7-M3: feet overlap"),
					Rejected("M1: 20° off plane"), Rejected("M6: out-of-plane ecc. 10 mm"),
					Rejected("M7: θ=0.0°"), Rejected("chord not tubular"), Rejected("no brace"),
				}),
				(2, "CON2", new[] { Rejected("the joint produced no §6.4 check") }));

			var con6 = rows.Where(r => r["Connection"] == "CON6").ToList();
			var con2 = rows.Where(r => r["Connection"] == "CON2").ToList();

			Assert.Multiple(() =>
			{
				Assert.That(con6, Has.Count.EqualTo(1), "seven unmet conditions, one row");
				Assert.That(con2, Has.Count.EqualTo(1), "and one unmet condition is also one row");
				Assert.That(con6[0]["Title"], Does.Contain("7 conditions not met"),
					"the count survives as orientation — 'a lot' rather than 'one thing'");
			});
		}

		/// <summary>
		/// "envelope" appears only on a row that HAS one. An envelope is a set of load cases; naming
		/// one on a row where nothing was assessed states a calculation that never happened.
		/// </summary>
		[Test]
		public void AnUnassessedRowNamesNoLoadCase()
		{
			var rows = RowsOf(
				(1, "CON1", new[] { Assessed(0.735) }),
				(2, "CON2", new[] { Rejected("no brace") }));

			Assert.Multiple(() =>
			{
				Assert.That(rows.Single(r => r["Connection"] == "CON2")["LoadCase"], Is.EqualTo("—"),
					"nothing was assessed, so there is no state to name");
				Assert.That(rows.Single(r => r["Connection"] == "CON1")["LoadCase"], Is.EqualTo("LE8"),
					"and a row that has one still shows it");
			});
		}

		/// <summary>
		/// Demand and Capacity are not columns.
		///
		/// For a §6.4 row Joint64ReportAdapter sets Demand to the utilisation and Capacity to a
		/// constant 1, so the pair restated the Utilization column and added one that never varies.
		/// On an unassessed row they showed "0 / 0", which reads as a measured zero.
		/// </summary>
		[Test]
		public void ThereAreNoDemandOrCapacityColumns()
		{
			var rows = RowsOf((1, "CON1", new[] { Assessed(0.735) }));

			Assert.Multiple(() =>
			{
				Assert.That(rows[0].Keys, Does.Not.Contain("Demand"));
				Assert.That(rows[0].Keys, Does.Not.Contain("Capacity"));
				// the number, not its punctuation: the decimal separator follows the machine's locale
				Assert.That(rows[0]["Utilization"].Replace(',', '.'), Is.EqualTo("73.5%"),
					"the utilisation is what those two were restating");
			});
		}

		/// <summary>
		/// The overview carries no per-chapter detail: no Detail column, and a title stripped of the
		/// K/Y/X split and the validity-range note.
		///
		/// Those belong to the §6.4 tab, which has a column for the classification and states the
		/// validity range in its own right. Repeated down fifteen rows of an overview they made the
		/// widest column out of the part nobody reads.
		/// </summary>
		[Test]
		public void TheOverviewCarriesNoPerChapterDetail()
		{
			var rows = RowsOf((1, "CON1", new[]
			{
				new NorsokFormulaResult
				{
					Section = "6.4.3.6", Equation = "6.57",
					Title = "Tubular Joint — M1 (K 0% / Y 0% / X 100%) — outside validity range (6.4.3.1)",
					LoadCaseName = "LE9", Utilization = 0.296, Passed = true,
				},
			}));

			Assert.Multiple(() =>
			{
				Assert.That(rows[0].Keys, Does.Not.Contain("Detail"),
					"the overview has no detail column");
				Assert.That(rows[0]["Title"], Is.EqualTo("Tubular Joint — M1"),
					"and the title keeps only what identifies the row");
			});
		}

		/// <summary>
		/// LoadCase comes before Equation: it qualifies the Section beside it, and the equation number
		/// is reference rather than something read across a row.
		/// </summary>
		[Test]
		public void LoadCaseComesBeforeEquation()
		{
			var rows = RowsOf((1, "CON1", new[] { Assessed(0.735) }));
			var order = rows[0].Keys.ToList();

			Assert.That(order.IndexOf("LoadCase"), Is.LessThan(order.IndexOf("Equation")));
		}
	}
}
