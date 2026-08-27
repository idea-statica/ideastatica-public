using NorsokChecker.Services.Norsok64;

namespace UT_NorsokChecker
{
	/// <summary>
	/// What may be believed from a cross-section NAME, and what may not.
	///
	/// The rule these pin: a name that does not spell out its dimensions yields NOTHING. A
	/// permissive parse is worse than no parse, because a wrong D is indistinguishable downstream
	/// from a right one — every "are the dimensions known" test passes and the check runs on it.
	///
	/// Measured on CON1 of test_cs, which is why this exists: a fallback regex that accepted any
	/// "&lt;number&gt;&lt;sep&gt;&lt;number&gt;" in the name of a tubular section read the FRACTION in
	/// 'PIPE(Imp)3-1/2XS' and returned D = 1 mm, T = 2 mm for a Ø102/8.3 tube.
	/// </summary>
	[TestFixture]
	public class SectionNameParsingTests
	{
		/// <summary>
		/// The names from CON1 that carry no parseable dimensions. Every one is a real catalogue
		/// name, and every one must come back empty — including the two that contain digits and a
		/// separator and would tempt a looser parse.
		/// </summary>
		[TestCase("PIPE(Imp)3-1/2XS", TestName = "an imperial fraction is not a diameter")]
		[TestCase("PIPE127STD", TestName = "a nominal size is not a diameter (really 141.3)")]
		[TestCase("76.0x3.5", TestName = "dimensions without the CHS prefix are not claimed")]
		[TestCase("GB-SSP42X2.5", TestName = "a catalogue code is not a diameter")]
		[TestCase("RO323.9X12.5", TestName = "another prefix that is not CHS")]
		public void ANameWithoutCHSYieldsNothing(string name)
		{
			var (d, t) = JointSectionInfo.ParseChs(name);

			Assert.Multiple(() =>
			{
				Assert.That(d, Is.Null, $"D must not be inferred from '{name}'");
				Assert.That(t, Is.Null, $"T must not be inferred from '{name}'");
			});
		}

		/// <summary>
		/// The known-good positives. Without these the fixture would pass by rejecting everything,
		/// which is the failure mode a "must return null" suite invites.
		/// </summary>
		[TestCase("CHS168.3/8.0", 168.3, 8.0)]
		[TestCase("CHS457x16", 457.0, 16.0)]
		[TestCase("CHS168,3/8,0", 168.3, 8.0)]
		[TestCase("CHS457,16 - CHORD(CHS457,16)", 457.0, 16.0)]
		[TestCase("CHS30,3", 30.0, 3.0)]
		public void ANameThatSaysCHSIsParsed(string name, double expectedD, double expectedT)
		{
			var (d, t) = JointSectionInfo.ParseChs(name);

			Assert.Multiple(() =>
			{
				Assert.That(d, Is.EqualTo(expectedD).Within(1e-9), $"D of '{name}'");
				Assert.That(t, Is.EqualTo(expectedT).Within(1e-9), $"T of '{name}'");
			});
		}

		/// <summary>
		/// The fraction specifically: 'PIPE(Imp)3-1/2XS' held BOTH a plausible-looking pair (1/2)
		/// and the real answer nowhere in the name.
		/// </summary>
		[Test]
		public void TheImperialFractionIsNotReadAsOneMillimetre()
		{
			var (d, _) = JointSectionInfo.ParseChs("PIPE(Imp)3-1/2XS");

			Assert.That(d, Is.Not.EqualTo(1.0),
				"D = 1 mm came from matching the fraction '1/2'; the real tube is 102 mm");
		}

		/// <summary>
		/// Through the REAL map builder, which is where the dropped fallback lived — ParseChs never
		/// had it, so testing ParseChs alone could not catch its return. Proved by an oracle run:
		/// reinstating the fallback left every ParseChs test green.
		///
		/// A tubular section whose name says nothing must come out with NO dimensions, so that
		/// RejectReason can say "tubular but its D/T are unknown" and the model geometry is the only
		/// source. It must NOT come out with D = 1 mm and every downstream test satisfied.
		/// </summary>
		[TestCase("PIPE(Imp)3-1/2XS", TestName = "map: the fraction is not a diameter")]
		[TestCase("76.0x3.5", TestName = "map: digits without CHS are not a diameter")]
		[TestCase("GB-SSP42X2.5", TestName = "map: a catalogue code is not a diameter")]
		public void TheMapLeavesATubularSectionDimensionlessWhenTheNameSaysNothing(string name)
		{
			var cs = new IdeaRS.OpenModel.CrossSection.CrossSectionParameter
			{
				Id = 1,
				Name = name,
				CrossSectionType = IdeaRS.OpenModel.CrossSection.CrossSectionType.RolledCHS,
			};

			var map = JointSectionMap.FromCrossSections(new object[] { cs });

			Assert.That(map.ContainsKey(1), Is.True);
			var sec = map[1];
			Assert.Multiple(() =>
			{
				Assert.That(sec.IsCHS, Is.True, "the TYPE is tubular — that much is known");
				Assert.That(sec.D, Is.Null, $"no D may be invented from '{name}'");
				Assert.That(sec.T, Is.Null, $"no T may be invented from '{name}'");
				Assert.That(sec.HasDimensions, Is.False,
					"with no dimensions this must not pass as dimensioned data");
			});
		}

		/// <summary>
		/// The map's known-good positive: a name that DOES say CHS still yields its dimensions, so
		/// the fixture above cannot pass by making the map return nothing at all.
		/// </summary>
		[Test]
		public void TheMapStillReadsADimensionedCHSName()
		{
			var cs = new IdeaRS.OpenModel.CrossSection.CrossSectionParameter
			{
				Id = 2,
				Name = "CHS168.3/8.0",
				CrossSectionType = IdeaRS.OpenModel.CrossSection.CrossSectionType.RolledCHS,
			};

			var sec = JointSectionMap.FromCrossSections(new object[] { cs })[2];

			Assert.Multiple(() =>
			{
				Assert.That(sec.D, Is.EqualTo(168.3).Within(1e-9));
				Assert.That(sec.T, Is.EqualTo(8.0).Within(1e-9));
				Assert.That(sec.HasDimensions, Is.True);
			});
		}
	}

	/// <summary>
	/// A tube read from the IOM facet ring must be physically possible.
	///
	/// No library section comes near these bounds, so this never fires on real input. It is here
	/// because the alternative to rejecting an impossible tube is REPORTING one, and a caller
	/// cannot tell a bad D from a good one.
	/// </summary>
	[TestFixture]
	public class TubeFromIomPlausibilityTests
	{
		private static IdeaRS.OpenModel.Connection.BeamData Beam(int facets, double thicknessM,
			double radiusM)
		{
			var b = new IdeaRS.OpenModel.Connection.BeamData
			{
				Name = "M1",
				Plates = new List<IdeaRS.OpenModel.Connection.PlateData>(),
			};
			// facet origins on the mid-surface circle, as IDEA models a tube
			for (int i = 0; i < facets; i++)
			{
				double a = 2 * Math.PI * i / facets;
				b.Plates.Add(new IdeaRS.OpenModel.Connection.PlateData
				{
					Thickness = thicknessM,
					Origin = new IdeaRS.OpenModel.Geometry3D.Point3D
					{
						X = radiusM * Math.Cos(a), Y = radiusM * Math.Sin(a), Z = 0,
					},
				});
			}
			return b;
		}

		/// <summary>The known-good positive: a real Ø141/6.5 chord reads back correctly.</summary>
		[Test]
		public void ARealTubeIsRead()
		{
			// mid-surface radius = (D - T)/2 = (141 - 6.5)/2 = 67.25 mm
			var (d, t, why) = TubeFromIom.FromBeam(Beam(64, 0.0065, 0.06725));

			Assert.Multiple(() =>
			{
				Assert.That(why, Is.Null);
				Assert.That(d, Is.EqualTo(141.0).Within(0.5), "D in mm");
				Assert.That(t, Is.EqualTo(6.5).Within(1e-9), "T in mm");
			});
		}

		/// <summary>A wall at half the diameter is not a tube; beta and gamma would be meaningless.</summary>
		[Test]
		public void AWallThickerThanHalfTheDiameterIsRejected()
		{
			var (d, _, why) = TubeFromIom.FromBeam(Beam(16, 0.020, 0.008));

			Assert.Multiple(() =>
			{
				Assert.That(d, Is.Null);
				Assert.That(why, Does.Contain("implausible"));
			});
		}

		/// <summary>Below 10 mm no section exists; a number this small means the read went wrong.</summary>
		[Test]
		public void AnAbsurdlySmallTubeIsRejected()
		{
			var (d, _, why) = TubeFromIom.FromBeam(Beam(16, 0.0001, 0.001));

			Assert.Multiple(() =>
			{
				Assert.That(d, Is.Null);
				Assert.That(why, Does.Contain("implausible"));
			});
		}
	}

	/// <summary>
	/// A gap computed without one of the diameters it depends on is not evidence of an overlap.
	///
	/// Measured 2026-08-27: against service 26.1 the IOM export returns no model at all for CON1 of
	/// test_cs, so no D/T can be read. A missing D defaulted to 0, which shortens each foot to
	/// nothing and moves the landing point, and the toe-to-toe subtraction then came out negative —
	/// the app reported "feet overlap, out of 6.4 gap rules" for five braces whose real gaps are
	/// +1.5 to +47 mm. The joint still cannot be checked (the missing diameter is the true reason,
	/// and the section gates say so), but it is no longer accused of a geometry it does not have.
	/// </summary>
	[TestFixture]
	public class UnknownGapTests
	{
		private static BraceGap Gap(double gapM, bool known) => new()
		{
			A = "M4", B = "M5", GapM = gapM, Side = -1, Adjacent = true, Known = known,
		};

		/// <summary>
		/// Runs the REAL gate — JointTopologyBuilder.FinalizeVerdict — not a copy of its filter.
		///
		/// The first version of these tests re-implemented the `Adjacent &amp;&amp; Known &amp;&amp; GapM &lt; 0`
		/// condition here, and an oracle run proved that worthless: reverting the fix in the
		/// production code left all of them green, because they were testing the copy. A test that
		/// restates the code it guards cannot fail when that code changes.
		/// </summary>
		private static List<string> Errors(params BraceGap[] gaps)
		{
			var topo = new JointTopology();
			topo.Gaps.AddRange(gaps);
			JointTopologyBuilder.FinalizeVerdict(topo);
			return topo.Verdict.Errors;
		}

		[Test]
		public void AnUnknownNegativeGapIsNotReportedAsAnOverlap()
		{
			var errors = Errors(Gap(-0.032, known: false));

			Assert.Multiple(() =>
			{
				Assert.That(errors, Is.Empty, "an unmeasured gap is not evidence of an overlap");
				Assert.That(errors.Any(e => e.Contains("overlap")), Is.False);
			});
		}

		/// <summary>
		/// The known-good positive: a real overlap must still be caught, or this fix would have
		/// silenced the gate it was meant to make honest.
		/// </summary>
		[Test]
		public void AMeasuredOverlapIsStillReported()
		{
			var errors = Errors(Gap(-0.032, known: true));

			Assert.Multiple(() =>
			{
				Assert.That(errors, Has.Count.EqualTo(1));
				Assert.That(errors[0], Does.Contain("feet overlap"));
			});
		}

		[Test]
		public void AMeasuredClearGapIsNotAnOverlap()
		{
			Assert.That(Errors(Gap(0.0015, known: true)), Is.Empty);
		}

		/// <summary>
		/// The verdict as a whole, since that is what the caller acts on: an unknown gap must not
		/// push it to ERROR, because ERROR is what stops §6.4 from being evaluated at all.
		/// </summary>
		[Test]
		public void AnUnknownGapDoesNotMakeTheVerdictAnError()
		{
			var topo = new JointTopology();
			topo.Gaps.Add(Gap(-0.032, known: false));
			JointTopologyBuilder.FinalizeVerdict(topo);

			Assert.That(topo.Verdict.Status, Is.EqualTo("OK"));
		}

		/// <summary>Gaps default to known: the flag marks the exception, not the rule.</summary>
		[Test]
		public void AGapIsKnownUnlessSaidOtherwise()
		{
			Assert.That(new BraceGap().Known, Is.True);
		}
	}
}
