using System.Globalization;
using NorsokChecker.Models;

namespace UT_NorsokChecker
{
	/// <summary>
	/// The unit conversions, against values a reader could look up — not against the code that
	/// produces them.
	///
	/// A conversion test that computes its expectation the same way the production code does proves
	/// only that one expression equals itself. Every number below is either a defined constant
	/// (1 in = 25.4 mm exactly, 1 kip = 4448.2216152605 N by definition of the pound-force) or a
	/// figure an engineer would recognise: S355 is 51.5 ksi, a 141.3 mm tube is 5.563 in.
	/// </summary>
	[TestFixture]
	public class QuantityFormatTests
	{
		private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;

		[TestCase(ForceUnit.KiloNewton, 1.0)]
		[TestCase(ForceUnit.Newton, 1000.0)]
		[TestCase(ForceUnit.MegaNewton, 0.001)]
		[TestCase(ForceUnit.Kip, 0.2248089431)]      // 1 kN = 224.809 lbf = 0.2248 kip
		public void AForceConvertsToItsUnit(ForceUnit u, double expectedPerKiloNewton)
		{
			Assert.That(QuantityFormat.ToForce(1000.0, u),
				Is.EqualTo(expectedPerKiloNewton).Within(expectedPerKiloNewton * 1e-6));
		}

		[TestCase(StressUnit.MPa, 355.0)]
		[TestCase(StressUnit.NPerMm2, 355.0)]        // the same number, a different spelling
		[TestCase(StressUnit.KPa, 355_000.0)]
		[TestCase(StressUnit.Ksi, 51.4884)]          // S355 is 51.5 ksi in the tables
		public void AStressConvertsToItsUnit(StressUnit u, double expected)
		{
			Assert.That(QuantityFormat.ToStress(355e6, u),
				Is.EqualTo(expected).Within(expected * 1e-5));
		}

		[TestCase(LengthUnit.Millimetre, 141.3)]
		[TestCase(LengthUnit.Centimetre, 14.13)]
		[TestCase(LengthUnit.Metre, 0.1413)]
		[TestCase(LengthUnit.Inch, 5.5629921)]       // 141.3 / 25.4, the inch being exact
		public void ALengthConvertsToItsUnit(LengthUnit u, double expected)
		{
			Assert.That(QuantityFormat.ToLength(0.1413, u),
				Is.EqualTo(expected).Within(expected * 1e-6));
		}

		/// <summary>
		/// MPa and N/mm² are the same number. The setting is a spelling choice and must not move a
		/// digit — a "conversion" between them would be a factor of 1 dressed up as a feature.
		/// </summary>
		[Test]
		public void MPaAndNPerMm2AreTheSameNumber()
		{
			Assert.That(QuantityFormat.ToStress(355e6, StressUnit.NPerMm2),
				Is.EqualTo(QuantityFormat.ToStress(355e6, StressUnit.MPa)));
		}

		/// <summary>
		/// Area and inertia follow the LENGTH choice, squared and to the fourth. CHS 141.3/6.3 has
		/// A = 2672 mm² = 26.72 cm² and I = 6.10×10⁶ mm⁴ = 610 cm⁴ — the figures a section table
		/// prints, which is the point of offering cm at all.
		/// </summary>
		[Test]
		public void SectionPropertiesFollowTheLengthChoice()
		{
			const double aM2 = 2.6716e-3, iM4 = 6.1002e-6;

			Assert.Multiple(() =>
			{
				Assert.That(QuantityFormat.ToArea(aM2, LengthUnit.Millimetre),
					Is.EqualTo(2671.6).Within(0.1), "mm²");
				Assert.That(QuantityFormat.ToArea(aM2, LengthUnit.Centimetre),
					Is.EqualTo(26.716).Within(0.001), "cm² — the section table's own figure");

				Assert.That(QuantityFormat.ToInertia(iM4, LengthUnit.Millimetre),
					Is.EqualTo(6.1002e6).Within(1e3), "mm⁴");
				Assert.That(QuantityFormat.ToInertia(iM4, LengthUnit.Centimetre),
					Is.EqualTo(610.02).Within(0.1), "cm⁴");
			});
		}

		/// <summary>
		/// The moment is DERIVED from the force and length choice, and the metric units all pair
		/// with the metre. Nobody writes kN·mm, so selecting millimetres must not turn 1 kN·m into
		/// 1000 of anything.
		/// </summary>
		[TestCase(ForceUnit.KiloNewton, LengthUnit.Millimetre, "1.000")]
		[TestCase(ForceUnit.KiloNewton, LengthUnit.Metre, "1.000")]
		[TestCase(ForceUnit.Newton, LengthUnit.Millimetre, "1000.000")]
		public void AMomentFollowsItsForceAndLength(ForceUnit f, LengthUnit l, string expected)
		{
			var s = new DisplaySettings { Force = f, Length = l };
			Assert.That(QuantityFormat.Moment(1000.0, Inv, s), Is.EqualTo(expected));
		}

		/// <summary>
		/// The exponent comes from the value. This is the whole reason scientific notation is here:
		/// the fixed ×10⁶ it replaces printed a CHS 30×3's I as `0.0`.
		/// </summary>
		[TestCase(23475.0, "2.35×10⁴")]
		[TestCase(914277855.0, "9.14×10⁸")]
		[TestCase(0.0234, "2.34×10⁻²")]
		public void ScientificNotationTakesItsExponentFromTheValue(double v, string expected)
		{
			Assert.That(QuantityFormat.SciString(v, Inv), Is.EqualTo(expected));
		}

		/// <summary>
		/// Significant figures, floored at three decimals — the floor being what keeps a factor
		/// near unity from losing the digit that carries its meaning.
		/// </summary>
		[TestCase(18.444727, "18.445")]   // 4 sig figs would be 18.44; the 3-decimal floor wins
		[TestCase(1.003292, "1.003")]     // four significant figures alone would give this too, but
		[TestCase(1.000041, "1.000")]     // here the floor is what stops it becoming "1"
		[TestCase(0.0344, "0.03440")]
		public void SignificantFiguresKeepARelativeAccuracy(double v, string expected)
		{
			Assert.That(QuantityFormat.Significant(v, Inv, sig: 4), Is.EqualTo(expected));
		}

		/// <summary>
		/// The culture is a PARAMETER. The report must be invariant — one of its tests sweeps the
		/// whole body for comma decimals under cs-CZ — while the GUI follows the machine. A
		/// formatter that chose one would break the other.
		/// </summary>
		[Test]
		public void TheCultureIsTheCallersChoice()
		{
			var cz = new CultureInfo("cs-CZ");

			Assert.Multiple(() =>
			{
				Assert.That(QuantityFormat.Force(1500.0, Inv), Is.EqualTo("1.5"));
				Assert.That(QuantityFormat.Force(1500.0, cz), Is.EqualTo("1,5"),
					"the GUI shows a Czech user a comma");
				Assert.That(QuantityFormat.Report, Is.EqualTo(CultureInfo.InvariantCulture),
					"and the report is pinned to invariant whatever the machine is set to");
			});
		}

		/// <summary>NaN and infinity are display states, not numbers. They must not read as "NaN".</summary>
		[TestCase(double.NaN)]
		[TestCase(double.PositiveInfinity)]
		public void ANonFiniteValuePrintsAsADash(double v)
		{
			Assert.Multiple(() =>
			{
				Assert.That(QuantityFormat.Num(v, Inv, 2), Is.EqualTo("—"));
				Assert.That(QuantityFormat.Significant(v, Inv), Is.EqualTo("—"));
				Assert.That(QuantityFormat.Percent(v, Inv), Is.EqualTo("—"));
			});
		}
	}
}
