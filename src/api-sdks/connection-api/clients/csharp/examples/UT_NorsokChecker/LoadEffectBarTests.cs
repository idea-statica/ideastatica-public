using System.Windows;
using System.Windows.Data;
using NorsokChecker.Controls;
using NorsokChecker.Models;

namespace UT_NorsokChecker
{
	/// <summary>
	/// The load-effect selector's utilisation bar — the python selector's device, which answers
	/// "which state should I open" without opening each one.
	///
	/// Worth testing rather than eyeballing because two parts of it fail SILENTLY at runtime: a
	/// string bound to a Brush property resolves through a type converter that a build cannot check,
	/// and a GridLength converter returning the wrong unit gives a bar of the wrong width with no
	/// error anywhere.
	/// </summary>
	[TestFixture]
	public class LoadEffectBarTests
	{
		private static Le64Option Le(double util, bool anyFail = false) =>
			new() { Id = 1, Name = "LE1", MaxUtil = util, AnyFail = anyFail };

		/// <summary>The bar is as wide as the utilisation, and never wider than the row.</summary>
		[Test]
		public void TheBarWidthFollowsTheUtilisation()
		{
			Assert.Multiple(() =>
			{
				Assert.That(Le(0.0).BarFraction, Is.EqualTo(0.0), "no result, no bar");
				Assert.That(Le(0.616).BarFraction, Is.EqualTo(0.616).Within(1e-9));
				Assert.That(Le(1.0).BarFraction, Is.EqualTo(1.0));
				Assert.That(Le(2.5).BarFraction, Is.EqualTo(1.0),
					"a state over capacity fills the row rather than overflowing it");
			});
		}

		/// <summary>
		/// One utilisation is one colour wherever it appears — the bar uses the same four bands as
		/// the joint view and its legend. Drifting palettes were what made the view unreadable
		/// before; two places showing one quantity in different colours is the same trap.
		/// </summary>
		[Test]
		public void TheBarColourUsesTheSameBandsAsTheJointView()
		{
			Assert.Multiple(() =>
			{
				Assert.That(Le(0.30).BarColour, Is.EqualTo("#66BB6A"), "green");
				Assert.That(Le(0.70).BarColour, Is.EqualTo("#E6C93A"), "yellow");
				Assert.That(Le(0.92).BarColour, Is.EqualTo("#F08A2E"), "orange");
				Assert.That(Le(1.20).BarColour, Is.EqualTo("#EF5350"), "red");
				Assert.That(Le(0.0).BarColour, Is.EqualTo("#D7DBE0"), "grey when there is no number");
			});
		}

		/// <summary>
		/// The colour string must actually resolve to a Brush. A `Background="{Binding BarColour}"`
		/// binding goes through WPF's type converter at RUNTIME — a build says nothing about it, and
		/// a malformed value would leave the bar invisible with a first-chance exception nobody sees.
		/// </summary>
		[Test]
		public void EveryBarColourParsesAsABrush()
		{
			foreach (double util in new[] { 0.0, 0.30, 0.70, 0.92, 1.20 })
			{
				string hex = Le(util).BarColour;
				object? brush = null;
				Assert.DoesNotThrow(
					() => brush = new System.Windows.Media.BrushConverter().ConvertFromString(hex),
					$"'{hex}' must be a value WPF can bind to Background");
				Assert.That(brush, Is.InstanceOf<System.Windows.Media.SolidColorBrush>(), hex);
			}
		}

		/// <summary>
		/// The number beside the bar, and the em dash that stands in for a missing one.
		///
		/// The expected number is formatted through the current culture rather than written out:
		/// this machine is cs-CZ, so the app renders "61,6 %" with a decimal comma, and a hardcoded
		/// "61.6 %" made the test fail on correct output.
		/// </summary>
		[Test]
		public void TheUtilisationTextIsAnEmDashWhenThereIsNoNumber()
		{
			string expected = $"{61.6:F1} %";

			Assert.Multiple(() =>
			{
				Assert.That(Le(0.616).UtilText, Is.EqualTo(expected));
				Assert.That(Le(0.0).UtilText, Is.EqualTo("—"), "never '0.0 %', which reads as a result");
			});
		}

		[Test]
		public void TheFailMarkAppearsOnlyWhenABraceFails()
		{
			Assert.Multiple(() =>
			{
				Assert.That(Le(0.6, anyFail: true).FailMark, Is.EqualTo("✗"));
				Assert.That(Le(0.6).FailMark, Is.Empty);
			});
		}
	}

	/// <summary>
	/// The two GridLength converters behind the bar. A star unit that came back as a pixel length
	/// would give a bar of a fixed few pixels on every row — visibly wrong but with nothing logged,
	/// so it is checked here rather than by looking at it.
	/// </summary>
	[TestFixture]
	public class FractionConverterTests
	{
		private static readonly IValueConverter Filled = new FractionToStarConverter();
		private static readonly IValueConverter Rest = new FractionToRestStarConverter();

		private static GridLength Conv(IValueConverter c, object? v) =>
			(GridLength)c.Convert(v!, typeof(GridLength), null!, System.Globalization.CultureInfo.InvariantCulture);

		[Test]
		public void AFractionBecomesAStarLengthAndItsComplement()
		{
			var filled = Conv(Filled, 0.4);
			var rest = Conv(Rest, 0.4);

			Assert.Multiple(() =>
			{
				Assert.That(filled.IsStar, Is.True, "a pixel length would be a fixed-width bar");
				Assert.That(filled.Value, Is.EqualTo(0.4).Within(1e-9));
				Assert.That(rest.IsStar, Is.True);
				Assert.That(rest.Value, Is.EqualTo(0.6).Within(1e-9));
			});
		}

		/// <summary>
		/// Zero must be an ABSOLUTE zero, not 0*: a 0* column keeps a minimum size in some layouts,
		/// so a state with no result would still show a sliver of colour.
		/// </summary>
		[Test]
		public void ZeroIsAnAbsoluteZeroNotAStar()
		{
			var filled = Conv(Filled, 0.0);

			Assert.Multiple(() =>
			{
				Assert.That(filled.IsStar, Is.False);
				Assert.That(filled.Value, Is.EqualTo(0.0));
			});
		}

		[Test]
		public void AFullBarLeavesNoRemainder()
		{
			var rest = Conv(Rest, 1.0);

			Assert.Multiple(() =>
			{
				Assert.That(rest.IsStar, Is.False);
				Assert.That(rest.Value, Is.EqualTo(0.0));
			});
		}

		/// <summary>
		/// Anything that is not a usable number is treated as zero rather than throwing — a binding
		/// converter that throws takes the whole item template down with it.
		/// </summary>
		[TestCase(null)]
		[TestCase("not a number")]
		[TestCase(double.NaN)]
		public void ARubbishValueIsZeroRatherThanAnException(object? value)
		{
			Assert.DoesNotThrow(() => Conv(Filled, value));
			Assert.That(Conv(Filled, value).Value, Is.EqualTo(0.0));
		}
	}
}
