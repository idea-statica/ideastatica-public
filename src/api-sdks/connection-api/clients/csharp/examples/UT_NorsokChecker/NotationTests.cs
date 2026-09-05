using NorsokChecker.Models;
using NorsokChecker.Services;
using NorsokChecker.Services.Norsok64;

namespace UT_NorsokChecker
{
	/// <summary>
	/// The report writes the §6.4 quantities the way NORSOK N-004 writes them.
	///
	/// Two problems, both found by reading a rendered card. The check condition used M_y,Rd while the
	/// derivation under it used M_Rd,ip — five occurrences of each scheme in one document, for the
	/// same quantity. And every formula wrote a bare f_y while the inputs table printed TWO f_y
	/// values (chord and brace), so with both steels S355 nothing on the page said which one had
	/// been substituted.
	///
	/// The direction was settled by READING THE NORM (§6.4.3.6, p. 31–32 of N-004u3-16016541.pdf),
	/// not by reading the code: eq (6.57) is written with M_y,Sd / M_y,Rd / M_z,Sd / M_z,Rd, and its
	/// where-list defines M_y as the in-plane and M_z as the out-of-plane moment. So y/z is the
	/// NORM's notation and ip/op was ours — the check condition was already right and the derivation
	/// was the outlier. A code-only reading would have unified the other way.
	///
	/// The engine keeps its own property names (MRdIp, MipSd): a developer reads those, a customer
	/// reads the report.
	/// </summary>
	[TestFixture]
	public class NotationTests
	{
		/// <summary>One assessed brace, through the real engine so the derivation actually renders.</summary>
		private static NorsokFormulaResult AssessedBrace()
		{
			var inputs = Joint64Input.FromSI(
				D: 0.141, T: 0.0065, fyChord: 355e6,
				d: 0.076, t: 0.0035, fyBrace: 355e6,
				thetaDeg: 60.0, g: 0.047,
				frK: 1.0, frY: 0.0, frX: 0.0,
				nSd: -10e3, mipSd: -1e3, mopSd: 0.5e3,
				sigmaASd: 9.27e6, sigmaMySd: -25.48e6, sigmaMzSd: 0,
				gammaM: 1.15);

			var engine = Norsok64Engine.CheckJoint(inputs);

			return new NorsokFormulaResult
			{
				Section = "6.4.3.6", Equation = "6.57",
				Title = "Tubular Joint — M1",
				Utilization = 0.476, Passed = true,
				JointDetail = new JointCheckRow
				{
					Name = "M1", Skipped = false, Util = 0.476, Passed = true,
					Engine = engine, Inputs = inputs, DomClass = "K",
					Classification = new KyxClass
					{
						Name = "M1", FrK = 1.0, FrY = 0, FrX = 0,
						NSd = -10e3, MipSd = -1e3, MopSd = 0.5e3,
					},
					// Required, or the chord-stress block is not drawn at all — and then the test
					// asserting that the CHORD moments keep ip/op would be checking the absence of a
					// section that was never rendered. (The first run of this fixture did exactly
					// that; DerivationContentTests carries the same warning for the same reason.)
					ChordStress = new ChordStressRow
					{
						Name = "M1",
						A = 2.747e-3, I = 6.2252e-6, R = 0.0705, Side = 1,
						NChord = 25.5e3, MipChord = 2.25e3, MopChord = 0.0,
						SigmaA = 9.27e6, SigmaMy = -25.48e6, SigmaMz = 0.0,
					},
				},
			};
		}

		/// <summary>
		/// A brace whose classification is X, so the per-mode Y/X block renders.
		///
		/// Needed because the K brace above never reaches it: that block is written per ACTIVE class
		/// (`if (frac <= 1e-9) continue`), so a pure-K fixture leaves its N_Rd formula unrendered —
		/// and a test asserting on that formula could not fail. Measured: an oracle reverting exactly
		/// that formula passed until this brace was added.
		/// </summary>
		private static NorsokFormulaResult XBrace()
		{
			var inputs = Joint64Input.FromSI(
				D: 0.141, T: 0.0065, fyChord: 355e6,
				d: 0.076, t: 0.0035, fyBrace: 355e6,
				thetaDeg: 55.0, g: 0.0,
				frK: 0.0, frY: 0.0, frX: 1.0,
				nSd: -8e3, mipSd: -0.8e3, mopSd: 0.3e3,
				sigmaASd: 5.0e6, sigmaMySd: -12.0e6, sigmaMzSd: 0,
				gammaM: 1.15);

			var engine = Norsok64Engine.CheckJoint(inputs);

			return new NorsokFormulaResult
			{
				Section = "6.4.3.6", Equation = "6.57",
				Title = "Tubular Joint — M2",
				Utilization = 0.30, Passed = true,
				JointDetail = new JointCheckRow
				{
					Name = "M2", Skipped = false, Util = 0.30, Passed = true,
					Engine = engine, Inputs = inputs, DomClass = "X",
					Classification = new KyxClass
					{
						Name = "M2", FrK = 0, FrY = 0, FrX = 1.0,
						NSd = -8e3, MipSd = -0.8e3, MopSd = 0.3e3,
					},
				},
			};
		}

		/// <summary>
		/// Both braces, so every per-mode block of the derivation is exercised — K on one, X on the
		/// other. A report built from one classification renders only that mode's formulas.
		/// </summary>
		private static string Report() =>
			NorsokHtmlReportGenerator.GenerateReport(
				"test.ideaCon",
				new List<(string, List<NorsokFormulaResult>)>
				{
					("CON1", new List<NorsokFormulaResult> { AssessedBrace(), XBrace() }),
				},
				expandAll: false);

		/// <summary>
		/// ONE name per quantity, across the whole card. Asserted as an ABSENCE, because that is the
		/// only form that catches a partial rename: leaving one M_Rd,ip behind reintroduces exactly
		/// the two-schemes problem this fixes, and a test that only looked for M_y,Rd would pass.
		/// </summary>
		[Test]
		public void TheMomentSymbolsAreTheNormsThroughout()
		{
			string html = Report();

			Assert.Multiple(() =>
			{
				Assert.That(html, Does.Not.Contain("M_{Rd,ip}"), "no ip/op resistance symbol survives");
				Assert.That(html, Does.Not.Contain("M_{Rd,op}"));
				Assert.That(html, Does.Not.Contain("M_{ip,Sd}"), "nor the action symbols");
				Assert.That(html, Does.Not.Contain("M_{op,Sd}"));

				Assert.That(html, Does.Contain("M_{y,Rd}"), "and the norm's symbols are there");
				Assert.That(html, Does.Contain("M_{z,Rd}"));
			});
		}

		/// <summary>
		/// The chord's own moments use y/z too, with a ,chord index.
		///
		/// They are NOT terms of eq (6.57) — they are the chord's moments on the way to sigma, and
		/// the norm gives them no symbol of its own. But they are resolved into the SAME plane as the
		/// brace's M_y / M_z: JointForceResolver projects both onto `nb = ex × bx`, the plane the
		/// chord forms with that particular brace (`JointForceResolver.cs:109-118`, `:191-198`). So
		/// ip/op beside a y/z table would suggest two planes where there is one, which is what the
		/// user asked to fix — the index says which member the moment belongs to.
		/// </summary>
		[Test]
		public void TheChordMomentsUseTheSameLettersWithAChordIndex()
		{
			string html = Report();

			Assert.Multiple(() =>
			{
				Assert.That(html, Does.Contain("M_{y,chord}"),
					"the chord's in-plane moment shares the brace's plane, so it shares its letter");
				Assert.That(html, Does.Contain("M_{z,chord}"));
				Assert.That(html, Does.Not.Contain("M_{ip,chord}"),
					"and the old notation is gone, or one table would mix both");
			});
		}

		/// <summary>
		/// All three resistances the check condition names have a formula, side by side.
		///
		/// Printing only N_Rd would leave two of the condition's three terms unexplained — a reader
		/// meeting M_y,Rd in the inequality with nothing to say what it is.
		///
		/// The block MOVED, and this test moved with it. It used to sit at the top of every check
		/// card, symbolically, 40 times over in the reviewed report — while the same three
		/// resistances appeared a few lines below in each card WITH THEIR NUMBERS. The announcement
		/// was the duplicate, so it is now stated once, in chapter 3. The property it guards is
		/// unchanged: all three, together, aligned on their shared base.
		/// </summary>
		[Test]
		public void TheDesignResistanceBlockCarriesAllThree()
		{
			string html = Report();

			// In chapter 3 now. Anchored on the chapter heading and cut at the first connection, so
			// this cannot pass on a check card's own maths further down the document.
			int at = html.IndexOf("How the checks are made", StringComparison.Ordinal);
			Assert.That(at, Is.GreaterThan(0), "chapter 3 is rendered");
			int end = html.IndexOf("class='connection-header", at, StringComparison.Ordinal);
			string block = end > at ? html[at..end] : html[at..];

			Assert.Multiple(() =>
			{
				Assert.That(block, Does.Contain("N_{Rd}"), "axial");
				Assert.That(block, Does.Contain("M_{y,Rd}"), "in-plane bending");
				Assert.That(block, Does.Contain("M_{z,Rd}"), "out-of-plane bending");
				Assert.That(block, Does.Contain(@"\begin{aligned}"),
					"aligned, so the shared base and the two differences are visible at a glance");
			});
		}

		/// <summary>
		/// y and z are EXPLAINED, and specifically as the joint plane's — ONCE, in chapter 3.
		///
		/// The user's point: everywhere else in this application y and z are a member's local axes,
		/// so a reader who knows the rest of the app would derive the wrong meaning. The
		/// disambiguating half of the sentence is what matters — "in-plane" alone does not answer
		/// the concern that raised it.
		///
		/// It used to be a per-card legend AND chapter 3's sign-conventions sentence, word for word,
		/// so the report said it 41 times. The card's copy is gone; the sentence is the one that
		/// stays, and it took the (§6.4.3.6) reference with it.
		///
		/// SCOPED to chapter 3, unlike the version this replaces. That one searched the whole
		/// document, where "in-plane", "out-of-plane" and "joint plane" all occur in the method
		/// prose regardless of the legend — so rewording the legend to the OPPOSITE meaning would
		/// have left it green. Measured before rewriting it.
		/// </summary>
		[Test]
		public void ChapterThreeSaysWhichPlaneYAndZRefersTo()
		{
			string html = Report();

			int at = html.IndexOf("Sign conventions", StringComparison.Ordinal);
			Assert.That(at, Is.GreaterThan(0), "the sign-conventions paragraph");
			int end = html.IndexOf("</p>", at, StringComparison.Ordinal);
			string para = end > at ? html[at..end] : html[at..Math.Min(at + 600, html.Length)];

			Assert.Multiple(() =>
			{
				Assert.That(para, Does.Contain("in-plane"), "y is named");
				Assert.That(para, Does.Contain("out-of-plane"), "and z");
				Assert.That(para, Does.Contain("joint plane"),
					"and WHICH plane — the joint's, which is the part that stops someone reading "
					+ "them as the member's local axes");
				Assert.That(para, Does.Match(@"local y and z|local axes"),
					"said explicitly, because that is the wrong reading being guarded against");
				Assert.That(para, Does.Contain("6.4.3.6"),
					"with the clause, which used to be carried only by the card's legend");
			});
		}

		/// <summary>
		/// And the card does NOT repeat it. The point of moving it was that it was said 41 times.
		/// </summary>
		[Test]
		public void TheCardDoesNotRepeatTheSignConvention()
		{
			string html = Report();

			Assert.That(html, Does.Not.Contain("M<sub>y</sub> = in-plane"),
				"the per-card legend is gone — chapter 3 carries the sentence");
		}

		/// <summary>
		/// The INPUTS TABLE names its two yields as well, not just the formulas.
		///
		/// Missed on the first pass and found by the user on a rendered card: the formulas had been
		/// subscripted to f_y,chord while the table above them still printed a bare "f_y" on both the
		/// chord and the brace row. That is the exact confusion the change set out to remove — two
		/// values that look like one repeated, and with both steels S355 the numbers coincide too, so
		/// nothing on the page distinguishes them.
		/// </summary>
		[Test]
		public void TheInputsTableNamesBothYields()
		{
			string html = Report();

			Assert.Multiple(() =>
			{
				Assert.That(html, Does.Contain("f<sub>y,chord</sub>"),
					"the chord row says WHICH yield it carries — it is the one in eq (6.52)/(6.53)");
				Assert.That(html, Does.Contain("f<sub>y,brace</sub>"),
					"and the brace row too, so the pair does not read as one value repeated");
				Assert.That(html, Does.Not.Contain("(f<sub>y</sub> ="),
					"and neither row is left unnamed");
			});
		}

		/// <summary>
		/// THE GATE: no user-visible string anywhere in the app uses the old ip/op notation.
		///
		/// The notation lives in fourteen hand-written places — XAML column headers, the adapter's
		/// symbols, the generator's LaTeX, the 3D view's caption — with no shared constant, because
		/// a WPF DataGrid header cannot take one without dragging every label through x:Static. So
		/// the consistency is held by this test instead: it does not stop someone typing "M_ip", it
		/// stops that reaching a build. It also covers places that do not exist yet, which a
		/// constant would not.
		///
		/// Source-level on purpose. The alternative — render everything and grep — cannot reach a
		/// column header without constructing the window, and would miss a control the test does not
		/// know to open.
		/// </summary>
		[Test]
		public void NoUserVisibleStringUsesTheOldIpOpNotation()
		{
			var dir = new System.IO.DirectoryInfo(System.IO.Path.GetDirectoryName(
				System.Reflection.Assembly.GetExecutingAssembly().Location)!);
			while (dir != null && !System.IO.Directory.Exists(System.IO.Path.Combine(dir.FullName, "NorsokChecker")))
				dir = dir.Parent;
			if (dir == null) Assert.Ignore("cannot locate the NorsokChecker source from the test output");
			string app = System.IO.Path.Combine(dir!.FullName, "NorsokChecker");

			// M_ip / M_op / V_ip / V_op as a whole symbol. The engine's OWN identifiers (MipSd,
			// MRdIp, MipChord) are deliberately not matched — they are code, not text a customer
			// reads, and renaming them would touch the engine for no reader's benefit.
			var oldNotation = new System.Text.RegularExpressions.Regex(@"\b[MV]_(ip|op)\b");

			var offenders = new List<string>();
			foreach (var file in System.IO.Directory.EnumerateFiles(app, "*.*", System.IO.SearchOption.AllDirectories))
			{
				string ext = System.IO.Path.GetExtension(file).ToLowerInvariant();
				if (ext != ".cs" && ext != ".xaml") continue;
				// §6.3 and CBFEM are shelved and not rendered; their notation is not this rule's business.
				if (file.Contains("_Mothballed", StringComparison.OrdinalIgnoreCase)) continue;

				var lines = System.IO.File.ReadAllLines(file);
				for (int i = 0; i < lines.Length; i++)
				{
					string line = lines[i];
					// Comments explain the rule and name the old notation while doing so.
					string code = System.Text.RegularExpressions.Regex.Replace(line, @"(///.*|//.*|<!--.*)", "");
					if (oldNotation.IsMatch(code))
						offenders.Add($"{System.IO.Path.GetFileName(file)}:{i + 1}  {line.Trim()}");
				}
			}

			Assert.That(offenders, Is.Empty,
				"eq (6.57) writes M_y / M_z (in-plane / out-of-plane), and the app says so everywhere "
				+ "the user can see:\n  " + string.Join("\n  ", offenders));
		}

		/// <summary>
		/// Every §6.4 formula says WHICH yield it uses. Eq (6.52)/(6.53) key on the chord
		/// (Norsok64Engine.cs:117); the brace yield reaches a §6.4 result only through Q_g's φ, and
		/// only for an overlapped joint.
		/// </summary>
		[Test]
		public void EveryFormulaNamesTheChordYield()
		{
			string html = Report();

			// Count bare f_y inside math: f_y NOT followed by a comma or a subscript brace. A
			// substring test cannot do this — "f_y" is a prefix of "f_{y,chord}" — so the regex is
			// what makes the assertion mean anything.
			var bare = System.Text.RegularExpressions.Regex.Matches(html, @"f_y(?![,{A-Za-z])");

			Assert.Multiple(() =>
			{
				Assert.That(bare.Count, Is.Zero,
					$"{bare.Count} formula(s) still write a bare f_y, so the reader cannot tell which "
					+ "of the two yield values in the inputs table was substituted");
				Assert.That(html, Does.Contain("f_{y,chord}"), "and the chord yield is named");
			});
		}
	}
}
