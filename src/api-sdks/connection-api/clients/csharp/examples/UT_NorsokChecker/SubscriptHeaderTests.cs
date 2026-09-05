using System.Windows.Controls;
using System.Windows.Documents;
using NorsokChecker.Controls;

namespace UT_NorsokChecker
{
	/// <summary>
	/// Column headers carry TYPESET subscripts, not literal underscores.
	///
	/// Found on a rendered §6.4 tab: the brace-force table showed "NSd", "My", "Mz" while the
	/// classification table below it showed "N_Rd", "M_y,Rd". Two different notations for the same
	/// kind of quantity, on one screen.
	///
	/// The cause is a WPF detail worth stating, because it is invisible in the XAML: a
	/// DataGridColumn header is content for a ContentPresenter, which reads "_" as an ACCESS-KEY
	/// marker and eats it. The classification table escaped only because it already had a
	/// HeaderStyle routing the string through a TextBlock — an accident of the group-banner work.
	/// So neither table was "right": one silently dropped the underscore, the other printed it.
	///
	/// STA: constructs WPF text elements.
	/// </summary>
	[TestFixture, Apartment(System.Threading.ApartmentState.STA)]
	public class SubscriptHeaderTests
	{
		private static readonly SubscriptHeaderConverter Conv = new();

		/// <summary>The runs a header converts to, as (text, isSubscript) pairs.</summary>
		private static List<(string Text, bool Sub)> Runs(string header)
		{
			var block = (TextBlock)Conv.Convert(header, typeof(object), null,
				System.Globalization.CultureInfo.InvariantCulture);

			// A TextBlock built from a plain Text has no Inlines, so report it as one baseline run.
			if (block.Inlines.Count == 0)
				return new List<(string, bool)> { (block.Text, false) };

			return block.Inlines.OfType<Run>()
				.Select(r => (r.Text, r.BaselineAlignment == System.Windows.BaselineAlignment.Subscript))
				.ToList();
		}

		/// <summary>
		/// The symbol stays on the baseline, the subscript goes below it, and the unit comes back.
		///
		/// Asserted on the RUNS rather than on the concatenated text: a converter that returned
		/// "M_y,chord [kNm]" as one string would satisfy any text-level check while typesetting
		/// nothing, which is the whole defect.
		/// </summary>
		[TestCase("N_Rd", "N", "Rd", "")]
		[TestCase("M_y [kNm]", "M", "y", " [kNm]")]
		[TestCase("M_y,chord [kNm]", "M", "y,chord", " [kNm]")]
		[TestCase("M_z,Rd", "M", "z,Rd", "")]
		[TestCase("f_y [MPa]", "f", "y", " [MPa]")]
		[TestCase("M_tor [kNm]", "M", "tor", " [kNm]")]
		public void ASubscriptIsTypesetBelowTheBaseline(string header, string sym, string sub, string tail)
		{
			var runs = Runs(header);

			Assert.Multiple(() =>
			{
				Assert.That(runs[0].Text, Is.EqualTo(sym), "the symbol");
				Assert.That(runs[0].Sub, Is.False, "which is NOT subscripted");

				Assert.That(runs[1].Text, Is.EqualTo(sub), "the subscript's text");
				Assert.That(runs[1].Sub, Is.True,
					$"'{sub}' must be a real subscript — an underscore is not notation, and WPF would "
					+ "eat it anyway");

				if (tail.Length > 0)
					Assert.That(runs[2].Text, Is.EqualTo(tail), "the unit follows on the baseline");
				else
					Assert.That(runs, Has.Count.EqualTo(2), "nothing after the subscript");
			});
		}

		/// <summary>
		/// A header with no subscript still goes through a TextBlock, and its text survives intact.
		///
		/// The point is the plain ones: returning the raw string for them would put it back in the
		/// ContentPresenter's hands, and any underscore this converter's pattern does not match would
		/// be swallowed again — a silent partial fix.
		/// </summary>
		[TestCase("brace")]
		[TestCase("face")]
		[TestCase("utilisation")]
		[TestCase("N [kN]")]
		public void AHeaderWithoutASubscriptIsUnchanged(string header)
		{
			var runs = Runs(header);

			Assert.Multiple(() =>
			{
				Assert.That(runs, Has.Count.EqualTo(1), "one run, no split");
				Assert.That(runs[0].Text, Is.EqualTo(header), "and the text is intact");
				Assert.That(runs[0].Sub, Is.False);
			});
		}

		/// <summary>
		/// The subscript is SMALLER than the symbol.
		///
		/// BaselineAlignment alone only moves the run down; at the same size it reads as a dropped
		/// character rather than as an index.
		/// </summary>
		[Test]
		public void TheSubscriptIsSetSmaller()
		{
			var block = (TextBlock)Conv.Convert("M_y,Rd", typeof(object), null,
				System.Globalization.CultureInfo.InvariantCulture);
			var runs = block.Inlines.OfType<Run>().ToList();

			// `.Or.LessThan(12.0)` was appended here and defeated the relation the test is named
			// for: a subscript set at the SAME size as its symbol still passes as long as both are
			// under 12. The relation between the two runs is the whole subject, so it is asserted
			// on its own.
			Assert.That(runs[1].FontSize, Is.LessThan(block.FontSize),
				"the subscript must be smaller than the symbol it qualifies");
		}
	}
}
