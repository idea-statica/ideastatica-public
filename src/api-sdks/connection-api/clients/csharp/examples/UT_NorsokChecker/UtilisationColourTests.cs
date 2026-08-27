using System.Windows.Media;
using System.Windows.Media.Media3D;
using NorsokChecker.Controls;

namespace UT_NorsokChecker
{
	/// <summary>
	/// The utilisation ramp has to stay readable ON A LIT 3D SURFACE, which is not the same as
	/// being readable as flat swatches.
	///
	/// The measured cause of the reported defect (2026-08-27) was DARKNESS, not the choice of hues:
	/// with the old directional-only rig the darkest band shaded to #205823 — perceived brightness
	/// 0.11 — and at that little light the eye cannot resolve a hue that is mathematically fine
	/// (the old green/amber pair was 27 degrees apart and still read as two olives). The fix was
	/// the AmbientLight; the lighter, hue-separated tones are the second guard.
	///
	/// So the test that matters is <see cref="NoBandGoesNearlyBlackWhenUnlit"/>, and its threshold
	/// comes from the measurement: the old ramp's floor was 0.11, the new one's is 0.28. An
	/// earlier version of this fixture asserted only a hue separation of 20 degrees and PASSED on
	/// the old colours — a test that would not have caught the very defect it was written for.
	///
	/// The arithmetic is WPF's own: colour × (ambient + Σ lights), clamped, evaluated on the worst
	/// case — a face turned away from both directional lights, lit by ambient alone.
	///
	/// STA: constructing Joint3DView builds WPF controls.
	/// </summary>
	[TestFixture, Apartment(System.Threading.ApartmentState.STA)]
	public class UtilisationColourTests
	{
		/// <summary>
		/// The ambient term read from the REAL view, not hardcoded.
		///
		/// It was a constant here at first, and an oracle run showed the cost: stripping the
		/// AmbientLight out of Joint3DView.xaml left every test green, because the test was shading
		/// with a rig the app no longer had. The ambient light IS the fix for the reported defect —
		/// a test that cannot see it removed does not guard it.
		/// </summary>
		private static (double R, double G, double B) AmbientOf(Joint3DView view)
		{
			var light = FindLight(view);
			Assert.That(light, Is.Not.Null,
				"Joint3DView has no AmbientLight — the utilisation colours will shade to near-black "
				+ "on faces turned away from the directional lights, which is the reported defect");
			return (light!.Color.R / 255.0, light.Color.G / 255.0, light.Color.B / 255.0);
		}

		private static AmbientLight? FindLight(Joint3DView view)
		{
			foreach (var child in view.Viewport.Children)
			{
				if (child is not ModelVisual3D { Content: Model3DGroup group }) continue;
				foreach (var model in group.Children)
					if (model is AmbientLight a) return a;
			}
			return null;
		}

		[OneTimeSetUp]
		public void EnsureApplication()
		{
			if (System.Windows.Application.Current == null)
			{
				var app = new NorsokChecker.App();
				app.InitializeComponent();
			}
		}

		/// <summary>
		/// WPF's DiffuseMaterial on a face lit by the ambient term ONLY — the worst case, and the
		/// one the reported ramp failed. Both the colour and the light come from the real view.
		/// </summary>
		private static (double R, double G, double B) Unlit(double util)
		{
			var view = new Joint3DView();
			var c = ((SolidColorBrush)view.UtilisationBrushForTest(util)).Color;
			var a = AmbientOf(view);
			return (c.R / 255.0 * a.R, c.G / 255.0 * a.G, c.B / 255.0 * a.B);
		}

		/// <summary>The flat colour the view paints for this utilisation.</summary>
		private static Color ColourFor(double util) =>
			((SolidColorBrush)new Joint3DView().UtilisationBrushForTest(util)).Color;

		private static double Brightness((double R, double G, double B) c)
			=> 0.2126 * c.R + 0.7152 * c.G + 0.0722 * c.B;

		private static double Hue((double R, double G, double B) c)
		{
			double mx = Math.Max(c.R, Math.Max(c.G, c.B));
			double mn = Math.Min(c.R, Math.Min(c.G, c.B));
			if (mx - mn < 1e-9) return 0;
			double d = mx - mn, h;
			if (mx == c.R) h = ((c.G - c.B) / d % 6 + 6) % 6;
			else if (mx == c.G) h = (c.B - c.R) / d + 2;
			else h = (c.R - c.G) / d + 4;
			return h * 60;
		}

		/// <summary>One representative utilisation per band, in order.</summary>
		private static readonly (string Name, double Util)[] Bands =
		{
			("green (<0.5)", 0.30),
			("yellow (0.5-0.85)", 0.70),
			("orange (0.85-1.0)", 0.92),
			("red (>=1.0)", 1.20),
		};

		/// <summary>
		/// Every neighbouring pair must differ enough in brightness OR in hue. Either alone is
		/// sufficient — requiring both would rule out ramps that read perfectly well.
		/// </summary>
		[Test]
		public void NeighbouringBandsStayApartOnAnUnlitFace()
		{
			Assert.Multiple(() =>
			{
				for (int i = 0; i < Bands.Length - 1; i++)
				{
					var a = Unlit(Bands[i].Util);
					var b = Unlit(Bands[i + 1].Util);

					double dBright = Math.Abs(Brightness(a) - Brightness(b));
					double dHue = Math.Abs(Hue(a) - Hue(b));
					dHue = Math.Min(dHue, 360 - dHue);

					Assert.That(dBright > 0.08 || dHue > 20.0, Is.True,
						$"{Bands[i].Name} vs {Bands[i + 1].Name}: Δbrightness={dBright:F3}, "
						+ $"Δhue={dHue:F1}° — indistinguishable on a face turned away from the lights");
				}
			});
		}

		/// <summary>
		/// THE test for the reported defect. No band may shade so dark that its hue stops
		/// registering — that, not the hues themselves, is what made the view unreadable.
		///
		/// The threshold is measured under THIS lighting rig, which matters: the same colours are
		/// darker under the old directional-only rig (the old ramp's floor was 0.11 there, and that
		/// is the number that describes what was reported). Measured with the ambient light in
		/// place, the old ramp still bottoms out at 0.204 (its #C62828 red) while the current one
		/// holds 0.321. 0.25 sits between them, so this fails on the old colours and passes on
		/// these — verified by reverting them.
		/// </summary>
		[Test]
		public void NoBandGoesNearlyBlackWhenUnlit()
		{
			Assert.Multiple(() =>
			{
				foreach (var (name, util) in Bands)
				{
					double b = Brightness(Unlit(util));
					Assert.That(b, Is.GreaterThan(0.25),
						$"{name} shades to brightness {b:F3} — too dark to read as a colour "
						+ "(the ramp that was reported unreadable measured 0.204 here, 0.11 under "
						+ "the directional-only rig it actually shipped with)");
				}
			});
		}

		/// <summary>
		/// And not so light that a fully lit face clips to white, which on a white panel makes the
		/// body vanish. The full rig is ambient + both directionals.
		/// </summary>
		[Test]
		public void NoBandWashesOutToWhiteWhenFullyLit()
		{
			// every light in the real view, summed as WPF sums them
			var view = new Joint3DView();
			double sumR = 0, sumG = 0, sumB = 0;
			foreach (var child in view.Viewport.Children)
			{
				if (child is not ModelVisual3D { Content: Model3DGroup group }) continue;
				foreach (var model in group.Children)
				{
					if (model is not Light light) continue;
					sumR += light.Color.R / 255.0;
					sumG += light.Color.G / 255.0;
					sumB += light.Color.B / 255.0;
				}
			}
			var lights = (R: sumR, G: sumG, B: sumB);

			Assert.Multiple(() =>
			{
				foreach (var (name, util) in Bands)
				{
					var c = ColourFor(util);
					int clipped = 0;
					if (c.R / 255.0 * lights.R >= 0.999) clipped++;
					if (c.G / 255.0 * lights.G >= 0.999) clipped++;
					if (c.B / 255.0 * lights.B >= 0.999) clipped++;
					Assert.That(clipped, Is.LessThan(3),
						$"{name} clips on every channel — a white body on a white panel is invisible");
				}
			});
		}

		/// <summary>The bands are ordered: higher utilisation is never a cooler colour.</summary>
		[Test]
		public void TheRampRunsGreenToRed()
		{
			var green = ColourFor(0.30);
			var red = ColourFor(1.20);

			Assert.Multiple(() =>
			{
				Assert.That(green.G, Is.GreaterThan(green.R), "the low band is green-dominant");
				Assert.That(red.R, Is.GreaterThan(red.G), "the top band is red-dominant");
			});
		}

		/// <summary>
		/// A brace with no result must be grey, never a utilisation colour: an unchecked member
		/// painted green reads as "safe", the same defect as showing 0.0 % for "not assessed".
		/// </summary>
		[Test]
		public void AMemberWithNoResultIsGreyNotGreen()
		{
			var view = new Joint3DView();
			var grey = ((SolidColorBrush)view.NoCheckBrushForTest).Color;

			Assert.Multiple(() =>
			{
				Assert.That(grey.R, Is.EqualTo(grey.G), "grey has no hue");
				Assert.That(grey.G, Is.EqualTo(grey.B));
			});
		}
	}
}
