using NorsokChecker.Models;
using NorsokChecker.Services;
using NorsokChecker.Services.Norsok64;

namespace UT_NorsokChecker
{
	/// <summary>
	/// The "Joint plane and force transformation" section — the report's answer to the reviewer who
	/// cannot reconcile a single force in it against the model.
	///
	/// §6.4 is not evaluated on the load effects as IDEA StatiCa Connection shows them: the tool
	/// resolves a joint plane, identifies the through chord, and projects the member forces into it.
	/// The report used to print only the OUTPUT, headed "Applied forces (in the joint plane)", so
	/// there was no way in from the model side at all.
	/// </summary>
	[TestFixture]
	public class JointPlaneSectionTests
	{
		/// <summary>
		/// A topology with a known frame, a chord, two braces and one load effect.
		///
		/// The frame is deliberately OFF-AXIS — no member aligned to a global plane. A frame aligned
		/// to the axes makes "the local force was copied through" and "the force was projected"
		/// produce identical numbers, so the test could not tell them apart.
		/// </summary>
		private static JointTopology Topology()
		{
			var chord = new JointMemberData
			{
				Id = 1, Name = "M1",
				Section = new JointSectionInfo { Name = "CHS273.0/12.5", D = 0.273, T = 0.0125 },
			};

			return new JointTopology
			{
				Chord = chord,
				Ex = new Vec3(0.7071, 0.7071, 0),
				Ey = new Vec3(-0.4082, 0.4082, 0.8165),
				NPlane = new Vec3(0.5774, -0.5774, 0.5774),
				PlaneFitBasis = "least-squares fit over 2 braces",
				PlaneSpread = 0.0032,
				Coplanar = true,
				BracesMeta = new List<BraceMeta>
				{
					new() { Name = "M2", ThetaDeg = 47.3, Beta = 0.279, CoplanarDevDeg = 2.1,
						OopOffsetM = 0.004, Side = 1,
						Section = new JointSectionInfo { Name = "CHS76.1/3.6" } },
					new() { Name = "M3", ThetaDeg = 61.8, Beta = 0.418, CoplanarDevDeg = 0.4,
						OopOffsetM = -0.002, Side = -1,
						Section = new JointSectionInfo { Name = "CHS114.3/5.0" } },
				},
				// THREE states, with values that differ per state — so a table that shows one state
				// for every brace, or the wrong state for a brace, is detectable. LE1 is first in
				// the list on purpose: it is what the shipped version printed for everything.
				BraceForces = new List<PerLoadEffect<BraceForceRow>>
				{
					new()
					{
						Id = 1, Name = "LE1",
						Rows = new List<BraceForceRow>
						{
							new() { Name = "M2", LocalN = -11_000, LocalMy = 111, LocalMz = -11,
								NSd = -11_000, Mip = 101, Mop = -12 },
							new() { Name = "M3", LocalN = 22_000, LocalMy = 222, LocalMz = 22,
								NSd = 22_000, Mip = 202, Mop = 24 },
						},
					},
					new()
					{
						Id = 12, Name = "LE12",
						Rows = new List<BraceForceRow>
						{
							new() { Name = "M2",
								LocalN = -142_100, LocalVy = 3_200, LocalVz = -1_100,
								LocalMx = 210, LocalMy = 4_700, LocalMz = -980,
								NSd = -142_100, Vip = 2_900, Vop = -1_500,
								Mip = 4_310, Mop = -1_620 },
							new() { Name = "M3",
								LocalN = 33_000, LocalMy = 333, LocalMz = 33,
								NSd = 33_000, Mip = 303, Mop = 36 },
						},
					},
					new()
					{
						Id = 9, Name = "LE9",
						Rows = new List<BraceForceRow>
						{
							new() { Name = "M2", LocalN = -44_000, LocalMy = 444, LocalMz = -44,
								NSd = -44_000, Mip = 404, Mop = -48 },
							new() { Name = "M3",
								LocalN = 88_400, LocalVy = -2_100, LocalVz = 900,
								LocalMx = -140, LocalMy = -3_300, LocalMz = 720,
								NSd = 88_400, Vip = -1_800, Vop = 1_200,
								Mip = -3_050, Mop = 1_140 },
						},
					},
				},
			};
		}

		private static NorsokFormulaResult Assessed() => new()
		{
			Section = "6.4.3.6", Equation = "6.57", Title = "Tubular Joint — M2",
			LoadCaseName = "LE12", Utilization = 0.476, Passed = true,
		};

		/// <summary>
		/// A §6.4 check row for one brace, carrying the load effect that GOVERNS it.
		///
		/// The governing state is the point: the force table takes each brace's forces from ITS OWN
		/// governing load effect, so a row without a JointDetail contributes nothing — and two braces
		/// of one joint routinely govern on different states, which is what the fixture below
		/// exercises.
		/// </summary>
		private static NorsokFormulaResult Check(string brace, int govLeId, string govLeName) => new()
		{
			Section = "6.4.3.6", Equation = "6.57", Title = $"Tubular Joint — {brace}",
			LoadCaseName = govLeName, Utilization = 0.476, Passed = true,
			JointDetail = new JointCheckRow
			{
				Name = brace, Skipped = false, Util = 0.476, Passed = true,
				GovLeId = govLeId, GovLeName = govLeName,
			},
		};

		/// <summary>The rejection rows a topology emits when the chapter does not apply.</summary>
		private static NorsokFormulaResult Rejected(string why) => new()
		{
			Section = "6.4", Equation = "", Title = "Outside the scope of §6.4",
			CheckExpression = why, NotAssessed = true,
			Reason = NotAssessedReason.OutsideScope,
		};

		private static string Report(bool withTopology = true,
			IEnumerable<NorsokFormulaResult>? rows = null) =>
			NorsokHtmlReportGenerator.GenerateReport("test.ideaCon",
				new List<(string, List<NorsokFormulaResult>)>
				{
					("CON1", (rows ?? new[]
					{
						// M2 governs on LE12, M3 on LE9 — different states, which a single-state
						// table cannot represent and which the shipped version got wrong by
						// printing LE1's forces for both.
						Check("M2", 12, "LE12"), Check("M3", 9, "LE9"),
					}).ToList()),
				},
				expandAll: false,
				jointImages: null,
				topologies: withTopology
					? new Dictionary<string, JointTopology> { ["CON1"] = Topology() }
					: null);

		/// <summary>
		/// THE test: the model's own force and the projected force appear TOGETHER.
		///
		/// Both numbers, in one table. Printing only the projected value is the defect; printing
		/// only the local one would be a different defect. The pair is what makes the check
		/// traceable, so the pair is what is asserted.
		/// </summary>
		[Test]
		public void BothTheModelForceAndTheProjectedForceAreShown()
		{
			string html = Report();

			int at = html.IndexOf("Force transformation", StringComparison.Ordinal);
			Assert.That(at, Is.GreaterThan(0), "the transformation section is rendered");
			string table = html[at..(at + 3000)];

			Assert.Multiple(() =>
			{
				// M2's axial force at ITS governing state (LE12), as the model carries it.
				Assert.That(table, Does.Contain("-142.1 kN"), "N from the model");
				// Its in-plane moment differs between the two frames — 4.70 local, 4.31 projected —
				// which is the whole point: the same loading, two frames, two numbers.
				Assert.That(table, Does.Contain("4.70 kN&middot;m"), "M_y in the member's local axes");
				Assert.That(table, Does.Contain("4.31 kN&middot;m"), "and after projection");
				Assert.That(table, Does.Contain("from the model (local axes)"),
					"the columns say which frame each half is in");
				Assert.That(table, Does.Contain("resolved into the joint plane"));

				// EACH BRACE AT ITS OWN STATE. M2 governs on LE12 and M3 on LE9, so both names
				// appear and M3's numbers are LE9's — 88.4 kN, not LE12's 33.0 kN. The shipped
				// version printed LE1's forces for every brace, which is what the LE1 values in the
				// fixture are there to catch.
				Assert.That(table, Does.Contain("LE12").And.Contain("LE9"),
					"each row names the state that governs that brace");
				Assert.That(table, Does.Contain("88.4 kN"), "M3 is shown at LE9, its own governing state");
				Assert.That(table, Does.Not.Contain("11.0 kN"),
					"and nothing comes from LE1 merely because it is first in the list");
			});
		}

		/// <summary>
		/// The plane, the chord and the frame are stated in model coordinates.
		///
		/// Without the normal a reader cannot reproduce the projection at all, and without the chord
		/// they cannot tell which member the β and γ ratios are taken against.
		/// </summary>
		[Test]
		public void ThePlaneAndTheChordAreStated()
		{
			string html = Report();

			Assert.Multiple(() =>
			{
				Assert.That(html, Does.Contain("Joint plane and force transformation"), "the section");
				Assert.That(html, Does.Contain("chord (through member)"), "which member is the chord");
				Assert.That(html, Does.Contain("M1"), "and its name");
				Assert.That(html, Does.Contain("CHS273.0/12.5"), "with its section");
				Assert.That(html, Does.Contain("plane normal"), "the normal is given");
				// Off-axis by construction, so the components are all non-trivial.
				Assert.That(html, Does.Contain("+0.577"), "in model coordinates, as a vector");
				Assert.That(html, Does.Contain("least-squares fit"), "and how the plane was fixed");
			});
		}

		/// <summary>
		/// The per-member geometry is printed ONCE for the joint, not once per check.
		///
		/// The review measured the repetition: θ, β, γ, τ and the chord's section properties were
		/// printed identically inside every one of the 30 checks — pages 6 and 20 of the shipped
		/// report carry the same numbers. These are properties of the JOINT.
		/// </summary>
		[Test]
		public void TheMemberGeometryIsListedOncePerJoint()
		{
			string html = Report();

			int at = html.IndexOf("Members &mdash; geometry at the joint", StringComparison.Ordinal);
			Assert.That(at, Is.GreaterThan(0), "the geometry table");
			string table = html[at..html.IndexOf("</table>", at, StringComparison.Ordinal)];

			Assert.Multiple(() =>
			{
				Assert.That(table, Does.Contain("M2").And.Contain("M3"), "every brace has a row");
				Assert.That(table, Does.Contain("47.3&deg;"), "θ per brace");
				Assert.That(table, Does.Contain("0.279"), "β per brace");
				Assert.That(table, Does.Contain("+ey").And.Contain("&minus;ey"),
					"and which chord face each lands on — the two braces are on opposite faces");
			});
		}

		/// <summary>
		/// No topology, no section — and the rest of the report is unaffected.
		///
		/// The parameter is optional so a test about check cards need not build a topology. Worth
		/// pinning: a generator that threw or emitted an empty section here would make every
		/// existing report test carry a topology it does not care about.
		/// </summary>
		[Test]
		public void WithoutATopologyTheSectionIsSimplyAbsent()
		{
			string html = Report(withTopology: false);

			Assert.Multiple(() =>
			{
				Assert.That(html, Does.Not.Contain("Joint plane and force transformation"));
				Assert.That(html, Does.Contain("Tubular Joint"), "the check card is still there");
			});
		}

		/// <summary>
		/// A REJECTED joint shows how the verdict was reached, and no forces.
		///
		/// Two contradictions were shipped here in turn. First the section rendered unconditionally,
		/// so CON7 stated the joint plane, the chord and the transformed forces directly above a card
		/// saying those quantities are ones "the joint does not provide" and that no brace could be
		/// assessed. Then suppressing it entirely created the opposite problem: every assessed joint
		/// showed its workings while a rejected one gave a bare verdict — even though its conditions
		/// quote measured numbers ("gap -16 mm", "20.0° off plane (>15°)").
		///
		/// So: the geometry and the chord stay, because that is what the conditions are read FROM,
		/// and the force table goes, because no force was resolved.
		/// </summary>
		[Test]
		public void ARejectedJointShowsItsGeometryButNoForces()
		{
			string html = Report(rows: new[]
			{
				Rejected("M4-M6: feet overlap (gap -16 mm < 0)"),
				Rejected("M1: 20.0° off plane (>15°)"),
			});

			Assert.Multiple(() =>
			{
				Assert.That(html, Does.Contain("How the joint was read"),
					"the block says what it is for on a joint nothing was assessed on");
				Assert.That(html, Does.Contain("Members &mdash; geometry at the joint"),
					"the geometry the conditions are measured from is shown");
				Assert.That(html, Does.Contain("chord (through member)"), "and the chord it identified");

				// The TABLE, matched by its column markup. The prose "no forces were resolved into
				// the joint plane" legitimately contains the same words, so matching the phrase
				// failed on correct output — the header cell is what distinguishes them.
				Assert.That(html, Does.Not.Contain("Force transformation"),
					"but NO force table — none were resolved into the plane");
				Assert.That(html, Does.Not.Contain("<th colspan='3'>resolved into the joint plane</th>"),
					"nor its column heading");
				Assert.That(html, Does.Not.Contain("from the model (local axes)"),
					"nor the model-side half of it");
			});
		}

		/// <summary>
		/// An ambiguous chord is labelled as a CHOICE, not stated as a fact.
		///
		/// The builder tie-breaks on "the largest Ø of the continuous members"
		/// (JointTopologyBuilder.cs:63). Printed bare, the row read as certainty immediately above a
		/// condition saying the chord is ambiguous — the document contradicting itself again.
		/// </summary>
		[Test]
		public void AnAmbiguousChordSaysSo()
		{
			string ambiguous = Report(rows: new[]
			{
				Rejected("2 continuous members — the chord is ambiguous; §6.4 needs exactly one"),
			});
			string plain = Report(rows: new[] { Rejected("M7: θ=0.0° — parallel to chord") });

			Assert.Multiple(() =>
			{
				Assert.That(ambiguous, Does.Contain("ambiguous &mdash; taken as the largest"),
					"the tie-break is disclosed where the chord is ambiguous");
				Assert.That(plain, Does.Not.Contain("taken as the largest"),
					"and not where it is not — the caveat must not be boilerplate");
			});
		}

		/// <summary>
		/// The section sits INSIDE the §6.4 chapter group, not above it.
		///
		/// The joint plane, the chord and the K/Y/X frame are §6.4's own constructs — the user's
		/// point: *"doufám že takhle tabulka je řiřazena k 6.4 protože jestli je obecná tak tam
		/// nepatří"*. Rendered before the groups, as it was, it read as a general property of the
		/// connection, and a second chapter would have inherited a section that is not about it.
		/// </summary>
		[Test]
		public void TheSectionBelongsToTheSixFourGroup()
		{
			string html = Report();

			int group = html.IndexOf("class='chapter-group'", StringComparison.Ordinal);
			int header = html.IndexOf("Tubular joints", StringComparison.Ordinal);
			int plane = html.IndexOf("Joint plane and force transformation", StringComparison.Ordinal);

			Assert.Multiple(() =>
			{
				Assert.That(group, Is.GreaterThan(0), "the chapter group is rendered");
				Assert.That(plane, Is.GreaterThan(group),
					"the section is inside the group, not before it");
				Assert.That(plane, Is.GreaterThan(header),
					"and after the chapter heading that owns it");
			});
		}

		/// <summary>
		/// The section comes BEFORE the checks that use its forces.
		///
		/// A reader meeting the utilisation first and its inputs afterwards is reading a claim with
		/// an appendix — the same reason eq (6.57) closes the derivation rather than opening it.
		/// </summary>
		[Test]
		public void TheSectionPrecedesTheChecks()
		{
			string html = Report();

			int plane = html.IndexOf("Joint plane and force transformation", StringComparison.Ordinal);
			int card = html.IndexOf("<details class='check-card", StringComparison.Ordinal);

			Assert.Multiple(() =>
			{
				Assert.That(plane, Is.GreaterThan(0));
				Assert.That(card, Is.GreaterThan(plane),
					"the inputs are stated before the check that consumes them");
			});
		}

		/// <summary>
		/// The section says WHICH plane each brace is resolved in — its own chord–brace sub-plane,
		/// not the fitted joint plane.
		///
		/// Round-2 §6: the reviewer found two connections differing only in one brace's `off-plane`
		/// angle whose force tables were identical to the last digit, and could not tell from the
		/// document whether that was correct. It is (the frame is built from the brace's own axis, so
		/// its deviation from the FITTED plane is absent from its own projection by construction) —
		/// but nothing in the report said so, and the geometry table presented the angle alongside θ
		/// and β, which the resistance really does use.
		/// </summary>
		[Test]
		public void TheSectionNamesTheSubPlaneEachBraceIsResolvedIn()
		{
			string html = Report();

			// In the METHOD chapter, stated once — it used to be repeated under every assessed
			// connection, which is the repetition the review objected to. The per-connection section
			// still has to point at it, which the second half asserts.
			int method = html.IndexOf("How the checks are made", StringComparison.Ordinal);
			Assert.That(method, Is.GreaterThan(0), "the method chapter is rendered");
			string chapter = html[method..Math.Min(html.Length, method + 6000)];

			int at = html.IndexOf("Force transformation", StringComparison.Ordinal);
			Assert.That(at, Is.GreaterThan(0), "the transformation section is rendered");
			string section = html[at..Math.Min(html.Length, at + 3000)];

			Assert.Multiple(() =>
			{
				Assert.That(chapter, Does.Contain("its own chord&ndash;brace pair"),
					"the plane each brace is actually resolved in");
				Assert.That(chapter, Does.Contain("cannot appear in its own"),
					"and why its own off-plane deviation cannot show up there");
				// What the fitted plane IS for, so removing the angle from the inputs does not read as
				// the plane being decorative.
				Assert.That(chapter, Does.Contain("K/Y/X classification"),
					"the fitted plane's real jobs are named");

				// And the per-connection section says where to find it, rather than repeating it.
				Assert.That(section, Does.Contain("sub-plane"), "the section names the frame");
				Assert.That(section, Does.Contain("chapter 3"), "and points at the method");
			});
		}

		/// <summary>
		/// The geometry table separates the resistance INPUTS from the coplanarity CHECKS.
		///
		/// Same finding: with all seven columns presented alike, `off-plane` read as a quantity the
		/// projection consumes. The values must stay attached to their braces after the reorder —
		/// a column swap that shifted the numbers would be worse than the original defect.
		/// </summary>
		[Test]
		public void TheGeometryTableSeparatesInputsFromChecks()
		{
			string html = Report();

			int at = html.IndexOf("Members &mdash; geometry at the joint", StringComparison.Ordinal);
			Assert.That(at, Is.GreaterThan(0));
			string table = html[at..html.IndexOf("</table>", at, StringComparison.Ordinal)];

			Assert.Multiple(() =>
			{
				Assert.That(table, Does.Contain("used by the check"), "the input group is labelled");
				Assert.That(table, Does.Contain("coplanarity checks"), "and so is the check group");
				Assert.That(table, Does.Contain("tool tolerances"),
					"marked as ours, not as a §6.4 requirement");

				// The values still belong to their own braces. M2 is θ=47.3, β=0.279, dev=2.1;
				// M3 is θ=61.8, β=0.418, dev=0.4 — so a shifted column would cross them over.
				var m2 = System.Text.RegularExpressions.Regex.Match(table,
					@"<b>M2</b></td>(?<cells>.*?)</tr>", System.Text.RegularExpressions.RegexOptions.Singleline);
				Assert.That(m2.Success, Is.True, "M2 has a row");
				var cells = System.Text.RegularExpressions.Regex
					.Matches(m2.Groups["cells"].Value, @"<td>([^<]*)</td>")
					.Select(x => x.Groups[1].Value).ToList();
				// Assert on POSITION relative to the header groups, not on absolute indices: the
				// column count is what the reorder changes, so an index-based assertion would have
				// to be rewritten by the very change it is meant to catch.
				string all = string.Join(" | ", cells);
				int iTheta = cells.FindIndex(c => c.Contains("47.3"));
				int iBeta = cells.FindIndex(c => c.Contains("0.279"));
				int iFace = cells.FindIndex(c => c.Contains("ey"));
				int iDev = cells.FindIndex(c => c.Contains("2.1"));

				Assert.That(iTheta, Is.GreaterThanOrEqualTo(0), $"θ present — cells: {all}");
				Assert.That(iBeta, Is.GreaterThan(iTheta), $"β after θ — cells: {all}");
				Assert.That(iFace, Is.GreaterThan(iBeta), $"chord face closes the inputs — cells: {all}");
				Assert.That(iDev, Is.GreaterThan(iFace),
					$"the brace's own off-plane sits in the CHECKS group, after the inputs — cells: {all}");
			});
		}

		/// <summary>
		/// How many braces are actually in the fitted plane, so a reader does not take the
		/// side-by-side columns as evidence of arithmetic that did not occur.
		///
		/// The reviewer asked for this: in the sample, 57 of 65 braces sit at 0.0°, where the two
		/// halves differ by a relabelling and a sign convention. The fixture holds one brace at 0.0°
		/// and one at 2.1°, so the count is 1 of 2 — a fixture with neither case could not fail.
		/// </summary>
		[Test]
		public void TheSectionSaysHowManyBracesLieInThePlane()
		{
			var topo = Topology();
			topo.BracesMeta[1].CoplanarDevDeg = 0.0;      // M3 exactly in the plane

			string html = NorsokHtmlReportGenerator.GenerateReport("test.ideaCon",
				new List<(string, List<NorsokFormulaResult>)>
				{
					("CON1", new List<NorsokFormulaResult> { Check("M2", 12, "LE12"), Check("M3", 9, "LE9") }),
				},
				expandAll: false, jointImages: null,
				topologies: new Dictionary<string, JointTopology> { ["CON1"] = topo });

			int at = html.IndexOf("Force transformation", StringComparison.Ordinal);
			string section = html[at..Math.Min(html.Length, at + 6000)];

			Assert.That(section, Does.Contain("<b>1 of 2</b>"),
				"one of the two braces lies in the plane — counted, not asserted in prose");
		}

		/// <summary>
		/// The selection criterion and the runner-up margin are printed.
		///
		/// Two questions the document could not answer. The criterion: "governing" is not the
		/// largest force — N_Rd depends on Q_f, Q_f on the chord stresses, and those on the load
		/// effect, so each candidate has its own resistance. Without that stated, the selection is
		/// not reproducible even by a reader holding every number. The margin: whether the choice
		/// was close, which is the one thing a reviewer wants from an envelope.
		/// </summary>
		[Test]
		public void TheSectionStatesHowTheGoverningStateWasChosen()
		{
			var rowM2 = Check("M2", 12, "LE12");
			rowM2.JointDetail!.Util = 0.737;
			rowM2.JointDetail.RunnerUpLeName = "LE7";
			rowM2.JointDetail.RunnerUpUtil = 0.712;

			string html = NorsokHtmlReportGenerator.GenerateReport("test.ideaCon",
				new List<(string, List<NorsokFormulaResult>)>
				{
					("CON1", new List<NorsokFormulaResult> { rowM2 }),
				},
				expandAll: false, jointImages: null,
				topologies: new Dictionary<string, JointTopology> { ["CON1"] = Topology() });

			int at = html.IndexOf("Governing state, and by what margin", StringComparison.Ordinal);
			Assert.That(at, Is.GreaterThan(0), "the per-connection table is rendered");
			string section = html[at..Math.Min(html.Length, at + 3000)];

			int method = html.IndexOf("How the checks are made", StringComparison.Ordinal);
			string chapter = html[method..Math.Min(html.Length, method + 6000)];

			Assert.Multiple(() =>
			{
				// The CRITERION is in the method chapter, once: not the largest force, because each
				// candidate state has its own resistance.
				Assert.That(chapter, Does.Contain("own resistance"),
					"each state has its own resistance — the reason force cannot pick the winner");
				Assert.That(chapter, Does.Contain("Q<sub>f</sub>"), "and what it depends on");

				// The MARGIN is per connection, in percentage POINTS and said so — "2.3 %" beside
				// two percentages invites the reader to take it as a relative difference.
				Assert.That(section, Does.Contain("LE7"), "the runner-up state is named");
				Assert.That(section, Does.Contain("2.5 pp"),
					"73.7 % − 71.2 % = 2.5 percentage points, not 2.5 %");
				Assert.That(section, Does.Contain("percentage points"), "and the unit is named");
			});
		}

		/// <summary>
		/// With no runner-up, the cell says WHICH of the three reasons applies rather than printing
		/// a dash that stands for all of them — the CON10 mistake in miniature.
		/// </summary>
		[Test]
		public void TheSectionDistinguishesTheReasonsForNoRunnerUp()
		{
			var single = Check("M2", 12, "LE12");
			single.JointDetail!.Util = 0.5;
			single.JointDetail.RunnerUpAbsence = JointEnvelope.RunnerUpAbsence.SingleState;

			var skipped = Check("M3", 9, "LE9");
			skipped.JointDetail!.Util = 0.4;
			skipped.JointDetail.RunnerUpAbsence = JointEnvelope.RunnerUpAbsence.OthersSkipped;

			string html = NorsokHtmlReportGenerator.GenerateReport("test.ideaCon",
				new List<(string, List<NorsokFormulaResult>)>
				{
					("CON1", new List<NorsokFormulaResult> { single, skipped }),
				},
				expandAll: false, jointImages: null,
				topologies: new Dictionary<string, JointTopology> { ["CON1"] = Topology() });

			int at = html.IndexOf("Governing state, and by what margin", StringComparison.Ordinal);
			string section = html[at..Math.Min(html.Length, at + 3000)];

			Assert.Multiple(() =>
			{
				Assert.That(section, Does.Contain("only one load effect"),
					"one state existed, so nothing could come second");
				Assert.That(section, Does.Contain("no other state produced a check"),
					"which is a different fact, and the reader acts differently on it");
			});
		}

		/// <summary>
		/// The method is explained ONCE, not under every connection.
		///
		/// Measured on the export: six assessed connections carried six identical copies of six
		/// explanatory paragraphs — and three of those paragraphs had just been added to answer the
		/// reviewer's own questions, so answering §6 made §4's repetition worse. Reported from the
		/// running app.
		///
		/// The rule is per-PARAGRAPH rather than a page count: what belongs to a connection stays
		/// there (its plane, chord, geometry, forces, governing states and how many of ITS braces
		/// lie in the fitted plane), and what is the same for all of them is stated where a reader
		/// meets it first.
		/// </summary>
		[Test]
		public void TheMethodIsExplainedOnceNotPerConnection()
		{
			// THREE assessed connections, so a per-connection copy shows up as three occurrences.
			var topos = new Dictionary<string, JointTopology>
			{
				["CON1"] = Topology(), ["CON2"] = Topology(), ["CON3"] = Topology(),
			};
			var rows = () => new List<NorsokFormulaResult> { Check("M2", 12, "LE12") };

			string html = NorsokHtmlReportGenerator.GenerateReport("test.ideaCon",
				new List<(string, List<NorsokFormulaResult>)>
				{
					("CON1", rows()), ("CON2", rows()), ("CON3", rows()),
				},
				expandAll: false, jointImages: null, topologies: topos);

			int Count(string needle) =>
				System.Text.RegularExpressions.Regex.Matches(
					html, System.Text.RegularExpressions.Regex.Escape(needle)).Count;

			Assert.Multiple(() =>
			{
				// The control: three connections really are in the document, so "once" is a finding
				// about the prose and not about an empty report.
				Assert.That(Count("Joint plane and force transformation"), Is.EqualTo(3),
					"control — three connections, three sections");

				foreach (var paragraph in new[]
				{
					"its own chord&ndash;brace pair",       // the sub-plane frame
					"Shear and torsion do not enter",      // eq (6.57) has three terms
					"own resistance</b>",                  // why force cannot pick the governing state
					"positive in TENSION",                 // the sign convention
					"without an r&times;F transfer",       // forces at the node
				})
				{
					Assert.That(Count(paragraph), Is.EqualTo(1),
						$"'{paragraph}' must appear once, in the method chapter — not per connection");
				}
			});
		}

		/// <summary>
		/// Shear and torsion are stated as outside THIS check, not as irrelevant.
		///
		/// "Shear does not enter eq (6.57) and torsion is excluded by §6.4" reads as "the standard
		/// deems them unimportant", which is not what the clause says. Eq (6.57) has three terms;
		/// the actions still have to be verified, and the report has to say where. Their §9.4:
		/// the difference between "not calculated" and "not calculated here" is the whole point.
		/// </summary>
		[Test]
		public void ShearAndTorsionAreExcludedFromThisCheckNotDismissed()
		{
			string html = Report();

			// In the method chapter now — the wording is what matters, not where it sits, and it
			// used to sit under all six connections.
			int at = html.IndexOf("How the checks are made", StringComparison.Ordinal);
			Assert.That(at, Is.GreaterThan(0), "the method chapter is rendered");
			string chapter = html[at..Math.Min(html.Length, at + 6000)];

			Assert.Multiple(() =>
			{
				Assert.That(html, Does.Not.Contain("torsion is excluded by"),
					"the wording that read as a dismissal — nowhere in the document");
				Assert.That(chapter, Does.Contain("do not enter"), "they are outside THIS check");
				Assert.That(chapter, Does.Contain("verified elsewhere"), "and must be checked");
				Assert.That(chapter, Does.Contain("6.3"), "with a pointer to where");
			});
		}

		/// <summary>
		/// A displaced joint says so, and the method chapter says what the offset means.
		///
		/// The CON16 defect was that a joint whose members all carry the same eccentricity was
		/// rejected outright — the braces stayed coplanar and their common plane was merely offset,
		/// but the gate measured from the work point. Now the plane sits on the chord, which means
		/// the report has to disclose two things it did not before: WHERE the plane is, and that a
		/// brace's eccentricity is judged against it rather than against the origin.
		/// </summary>
		[Test]
		public void ADisplacedJointReportsWhereItsPlaneSits()
		{
			var topo = Topology();
			topo.PlaneOffsetM = 0.040;          // 40 mm, as CON17 carries

			string html = NorsokHtmlReportGenerator.GenerateReport("test.ideaCon",
				new List<(string, List<NorsokFormulaResult>)>
				{
					("CON1", new List<NorsokFormulaResult> { Check("M2", 12, "LE12") }),
				},
				expandAll: false, jointImages: null,
				topologies: new Dictionary<string, JointTopology> { ["CON1"] = topo });

			int at = html.IndexOf("plane offset from the work point", StringComparison.Ordinal);
			Assert.That(at, Is.GreaterThan(0), "the offset is disclosed");
			string row = html[at..(at + 400)];

			// And the method chapter explains what it is measured against, once.
			int method = html.IndexOf("How the checks are made", StringComparison.Ordinal);
			string chapter = html[method..Math.Min(html.Length, method + 6000)];

			Assert.Multiple(() =>
			{
				Assert.That(row, Does.Contain("40.0 mm"), "the distance");
				Assert.That(row, Does.Contain("whole joint is displaced"), "and what it means");
				Assert.That(chapter, Does.Contain("passes through the chord axis"),
					"the method states where the plane is");
				Assert.That(chapter, Does.Contain("not from the model&#39;s work point")
					.Or.Contain("not from the model's work point"),
					"and what it is NOT measured from");
			});
		}

		/// <summary>
		/// An ordinary joint prints no offset row. Every joint carrying a 0.0 mm line would be a row
		/// of noise on the great majority of them.
		/// </summary>
		[Test]
		public void AJointOnTheWorkPointPrintsNoOffsetRow()
		{
			string html = Report();      // Topology() leaves PlaneOffsetM at zero

			Assert.That(html, Does.Not.Contain("plane offset from the work point"),
				"nothing to disclose when the joint sits on the work point");
		}

		/// <summary>
		/// The coplanarity tolerance is typeset with a degree sign and labelled non-normative.
		///
		/// It was built in the ENGINE as "within 2deg" — ASCII, in a document that typesets ≤ and °,
		/// and printed beside genuine clause references with nothing marking it as a tool setting.
		/// </summary>
		[Test]
		public void ThePlaneFitToleranceIsTypesetAndDeclaredNonNormative()
		{
			var topo = Topology();
			topo.PlaneFitTolDeg = 2.0;

			string html = NorsokHtmlReportGenerator.GenerateReport("test.ideaCon",
				new List<(string, List<NorsokFormulaResult>)>
				{
					("CON1", new List<NorsokFormulaResult> { Check("M2", 12, "LE12") }),
				},
				expandAll: false, jointImages: null,
				topologies: new Dictionary<string, JointTopology> { ["CON1"] = topo });

			int at = html.IndexOf("how the plane was fixed", StringComparison.Ordinal);
			Assert.That(at, Is.GreaterThan(0));
			string row = html[at..(at + 400)];

			Assert.Multiple(() =>
			{
				Assert.That(row, Does.Contain("2.0&deg;"), "typeset, not '2deg'");
				Assert.That(row, Does.Not.Contain("2deg"), "the ASCII form is gone");
				Assert.That(row, Does.Contain("tool tolerance"), "declared as ours");
				Assert.That(row, Does.Contain("not a &sect;6.4 requirement"), "and not the norm's");
			});
		}

		/// <summary>
		/// The app HANDS the generator its topologies.
		///
		/// The section renders correctly and would render nothing at all if MainWindow stopped
		/// passing the dictionary — measured on the previous three steps, a change connected to
		/// nothing leaves every test green. On the source, because the alternative is a live service.
		/// </summary>
		[Test]
		public void TheAppPassesTheTopologiesToTheReport()
		{
			var dir = new System.IO.DirectoryInfo(System.IO.Path.GetDirectoryName(
				System.Reflection.Assembly.GetExecutingAssembly().Location)!);
			while (dir != null && !System.IO.Directory.Exists(
				System.IO.Path.Combine(dir.FullName, "NorsokChecker")))
				dir = dir.Parent;
			if (dir == null) Assert.Ignore("cannot locate the NorsokChecker source");

			string code = System.Text.RegularExpressions.Regex.Replace(
				System.IO.File.ReadAllText(System.IO.Path.Combine(
					dir!.FullName, "NorsokChecker", "MainWindow.Report.cs")),
				@"//[^\n]*", "");

			// The ARGUMENT, not the word. Matching "topologies" anywhere keeps finding the local
			// dictionary that builds it — measured: removing the argument from the call left the
			// declaration in place and this test green, which is the wiring defect it exists for.
			var call = System.Text.RegularExpressions.Regex.Match(code,
				@"GenerateReport\(([^;]*?)\);", System.Text.RegularExpressions.RegexOptions.Singleline);

			Assert.Multiple(() =>
			{
				Assert.That(code, Does.Contain("_topologyPerConnection"),
					"the report is built from the run's own topologies");
				Assert.That(call.Success, Is.True, "the report is generated somewhere in this file");
				Assert.That(call.Groups[1].Value, Does.Contain("topologies"),
					"and the topologies are an ARGUMENT to it — a dictionary nothing passes is a "
					+ "section that never renders");
			});
		}

		/// <summary>
		/// The projection carries the model's own numbers through untouched.
		///
		/// The Local* fields exist so the report can show provenance, and their whole value is that
		/// they are NOT recomputed. Asserted against the resolver, not the fixture: a copy that
		/// scaled or re-signed anything would make the left-hand column a second opinion rather than
		/// the model's own figure.
		/// </summary>
		[Test]
		public void TheLocalForcesAreCopiedFromTheModelWithoutArithmetic()
		{
			var m = new JointMemberData
			{
				Id = 2, Name = "M2",
				AxisX = new Vec3(0.6, 0.8, 0), AxisY = new Vec3(-0.8, 0.6, 0),
				AxisZ = new Vec3(0, 0, 1),
				Section = new JointSectionInfo { D = 0.0761, T = 0.0036 },
			};
			var sl = new IdeaStatiCa.Api.Connection.Model.ConLoadEffectSectionLoad
			{
				N = -142_100, Vy = 3_200, Vz = -1_100, Mx = 210, My = 4_700, Mz = -980,
			};

			var row = JointForceResolver.BraceForceInPlane(
				m, sl, new Vec3(0.7071, 0.7071, 0), new Vec3(0.5774, -0.5774, 0.5774));

			Assert.Multiple(() =>
			{
				Assert.That(row.LocalN, Is.EqualTo(-142_100), "N, verbatim");
				Assert.That(row.LocalVy, Is.EqualTo(3_200));
				Assert.That(row.LocalVz, Is.EqualTo(-1_100));
				Assert.That(row.LocalMx, Is.EqualTo(210));
				Assert.That(row.LocalMy, Is.EqualTo(4_700));
				Assert.That(row.LocalMz, Is.EqualTo(-980));

				// And the projection still differs from them, or the section shows one number twice.
				Assert.That(row.Mip, Is.Not.EqualTo(row.LocalMy).Within(1.0),
					"the projected in-plane moment is not the local My");
			});
		}
	}
}
