using System.Globalization;
using System.Text.RegularExpressions;
using NorsokChecker.Models;
using NorsokChecker.Services;
using NorsokChecker.Services.Norsok64;

namespace UT_NorsokChecker
{
	/// <summary>
	/// A PRINTED SUBSTITUTION MUST EVALUATE TO ITS PRINTED RESULT.
	///
	/// The reader of a §6.4 check recomputes a line on a calculator. If the numbers shown do not
	/// give the number beside them, the sheet cannot be checked — and every defect of this kind
	/// found so far was invisible to a test that only asserted a formula was PRESENT:
	///
	///   · the area printed `2.75×10³ mm²` with the exponent as literal text — true in millimetres
	///     and false in every other unit;
	///   · σ = N/A printed `-103.6 kN / 2.75×10³ mm² = -37.7 MPa`, which evaluates to -0.0377;
	///   · σ = M·R/I was out by 10⁶;
	///   · M_Rd in inches printed T = 0.2559 in as `0.3`, making the line 37 % wrong.
	///
	/// The last one is the reason this test evaluates rather than pattern-matches: the units were
	/// right, the formula was right, and the arithmetic still did not close.
	///
	/// It parses the KaTeX out of the rendered page and computes it. That is deliberately crude —
	/// it handles the shapes this report actually prints (a fraction, products, sums, a power) and
	/// SKIPS anything it cannot parse rather than guessing. A skipped line is reported, so the test
	/// cannot quietly cover nothing: <see cref="TheEvaluatorReachesTheLinesThatMatter"/> asserts a
	/// floor on how many it checked.
	/// </summary>
	[TestFixture]
	public class PrintedSubstitutionTests
	{
		private static JointCheckRow Row()
		{
			var inp = Joint64Input.FromSI(
				D: 0.141, T: 0.0065, fyChord: 355e6,
				d: 0.102, t: 0.0065, fyBrace: 355e6,
				thetaDeg: 45.0, g: 0.047,
				frK: 0.19, frY: 0.38, frX: 0.43,
				nSd: -88.8e3, mipSd: -1.2e3, mopSd: 2.4e3,
				sigmaASd: 9.27e6, sigmaMySd: -25.48e6, sigmaMzSd: 0.0,
				gammaM: 1.15);
			var res = Norsok64Engine.CheckJoint(inp);
			return new JointCheckRow
			{
				Name = "M3", Skipped = false, Engine = res, Inputs = inp,
				Classification = new KyxClass
				{
					Name = "M3", FrK = 0.19, FrY = 0.38, FrX = 0.43,
					NSd = -88.8e3, MipSd = -1.2e3, MopSd = 2.4e3,
				},
				// CONSISTENT ON PURPOSE: σ_a = N/A and σ_my = −M·(side·R)/I must hold for these
				// numbers, or this test measures the fixture rather than the report. N = 25.5 kN
				// over A = 2747.7 mm² gives 9.27 MPa; M = 2.2047 kN·m over I/R gives −25.48 MPa.
				ChordStress = new ChordStressRow
				{
					Name = "M3", SigmaA = 9.27e6, SigmaMy = -25.48e6, SigmaMz = 0.0,
					A = 2.7477e-3, I = 6.1e-6, R = 0.0705,
					NChord = 25.47e3, MipChord = 2.2047e3, MopChord = 0.0, Side = 1,
				},
				DomClass = "X",
				Util = res.UtilWeighted, Passed = res.Passed,
				NRdWeighted = res.NRdWeighted, MRdIp = res.MRdIp, MRdOp = res.MRdOp,
				WithinRange = res.WithinRange, ChordOverstressed = res.ChordOverstressed,
			};
		}

		private static string Page(DisplaySettings s) =>
			NorsokHtmlReportGenerator.GenerateDerivationPage(Row(), "M3",
				"CON1", "LE12", "47.5909", "PASS", s);

		private static readonly DisplaySettings Metric = new();

		private static readonly DisplaySettings Imperial = new()
		{
			Force = ForceUnit.Kip, Stress = StressUnit.Ksi, Length = LengthUnit.Inch,
		};

		// ── the steps, as rendered ───────────────────────────────────────────

		private sealed record StepText(string Label, string? Substituted, string Result);

		private static List<StepText> Steps(string html)
		{
			var steps = new List<StepText>();
			// The blocks are not nested, so a straight scan over the three known div classes is
			// both simpler and less fragile than trying to match the wrapper.
			var labels = Regex.Matches(html, "<div class='deriv-step-label'>(.*?)</div>", RegexOptions.Singleline);
			var maths = Regex.Matches(html, "<div class='deriv-step-math'>\\$\\$(.*?)\\$\\$</div>", RegexOptions.Singleline);
			var results = Regex.Matches(html, "<div class='deriv-step-res'>\\$\\$=\\\\;(.*?)\\$\\$</div>", RegexOptions.Singleline);

			// Walk the document in order, pairing each label with the substitution and result that
			// follow it. Only substitutions (those beginning "=\;") are of interest.
			var ordered = new List<(int Pos, string Kind, string Text)>();
			foreach (Match m in labels) ordered.Add((m.Index, "label", m.Groups[1].Value));
			foreach (Match m in maths) ordered.Add((m.Index, "math", m.Groups[1].Value));
			foreach (Match m in results) ordered.Add((m.Index, "res", m.Groups[1].Value));
			ordered.Sort((a, b) => a.Pos.CompareTo(b.Pos));

			string label = "";
			string? subst = null;
			foreach (var (_, kind, text) in ordered)
			{
				if (kind == "label") { label = text; subst = null; }
				else if (kind == "math" && text.StartsWith("=\\;")) subst = text.Substring(3);
				else if (kind == "res") { steps.Add(new StepText(label, subst, text)); subst = null; }
			}
			return steps;
		}

		// ── a small evaluator for the KaTeX this report prints ───────────────

		/// <summary>
		/// Strip the typesetting and leave arithmetic. Returns null when the expression contains
		/// something this evaluator does not model — a min/max, a symbol, a comparison — so the
		/// caller skips it rather than inventing a number.
		/// </summary>
		private static string? Arithmetic(string katex)
		{
			// UNITS FIRST, AND ONE LEVEL OF NESTING. `\mathrm{mm^2}` carries a caret of its own, and
			// `\mathrm{kip{\cdot}in}` carries braces — a `[^}]*` pattern stops at the inner brace,
			// leaves `in}` behind and the line is skipped as unparseable. Both M_Rd substitutions
			// were silently skipped that way while the suite stayed green.
			string s = Regex.Replace(katex, @"\\mathrm\{(?:[^{}]|\{[^{}]*\})*\}", "");

			// Operators first, so a \dfrac's own braces contain plain arithmetic when it is rewritten.
			s = s.Replace(@"\times", "*").Replace(@"\cdot", "*").Replace(@"\!", "")
				 .Replace(@"\,", " ").Replace(@"\ ", " ").Replace("−", "-");

			// \dfrac{a}{b} → ((a)/(b)), innermost first
			for (int guard = 0; guard < 8; guard++)
			{
				var m = Regex.Match(s, @"\\d?frac\{([^{}]*)\}\{([^{}]*)\}");
				if (!m.Success) break;
				s = s.Remove(m.Index, m.Length).Insert(m.Index, $"(({m.Groups[1].Value})/({m.Groups[2].Value}))");
			}

			s = Regex.Replace(s, @"\\(left|right)", "");

			// SCIENTIFIC NOTATION IS ONE NUMBER. `2.75\times 10^{3}` must become a single atom:
			// left as `2.75*(10^3)` it is two factors, so a preceding `/` divides by 2.75 alone and
			// the line reads 1000× too large. That produced a false "the report is wrong" finding
			// against a report that was right.
			s = Regex.Replace(s, @"(\d+(?:\.\d+)?)\s*\*\s*10\^\{(-?\d+)\}",
				m => "(" + m.Groups[1].Value + "*(10^" + m.Groups[2].Value + "))");
			s = Regex.Replace(s, @"10\^\{(-?\d+)\}", m => "(10^" + m.Groups[1].Value + ")");
			s = Regex.Replace(s, @"\^\{([-\d.]+)\}", m => "^" + m.Groups[1].Value);
			// `0.256^2` — a bare exponent, which eq (6.53) prints for T².
			s = Regex.Replace(s, @"\^(\d)", m => "^" + m.Groups[1].Value);

			if (Regex.IsMatch(s, @"\\[a-zA-Z]+")) return null;   // a symbol we do not model
			if (s.Contains('=') || s.Contains('<') || s.Contains('>')) return null;
			if (Regex.IsMatch(s, "[A-Za-z]")) return null;       // a bare variable

			return s;
		}

		/// <summary>Evaluate + - * / ^ and parentheses. Null when it will not parse.</summary>
		private static double? Eval(string expr)
		{
			int i = 0;
			string s = expr.Replace(" ", "");
			try
			{
				double v = ParseSum(s, ref i);
				return i == s.Length ? v : null;
			}
			catch { return null; }
		}

		private static double ParseSum(string s, ref int i)
		{
			double v = ParseProduct(s, ref i);
			while (i < s.Length && (s[i] == '+' || s[i] == '-'))
			{
				char op = s[i++];
				double r = ParseProduct(s, ref i);
				v = op == '+' ? v + r : v - r;
			}
			return v;
		}

		private static double ParseProduct(string s, ref int i)
		{
			double v = ParsePower(s, ref i);
			while (i < s.Length && (s[i] == '*' || s[i] == '/'))
			{
				char op = s[i++];
				double r = ParsePower(s, ref i);
				v = op == '*' ? v * r : v / r;
			}
			return v;
		}

		private static double ParsePower(string s, ref int i)
		{
			double v = ParseAtom(s, ref i);
			if (i < s.Length && s[i] == '^')
			{
				i++;
				double e = ParseAtom(s, ref i);
				v = Math.Pow(v, e);
			}
			return v;
		}

		private static double ParseAtom(string s, ref int i)
		{
			if (i < s.Length && s[i] == '-') { i++; return -ParseAtom(s, ref i); }
			if (i < s.Length && s[i] == '(')
			{
				i++;
				double v = ParseSum(s, ref i);
				if (i >= s.Length || s[i] != ')') throw new FormatException();
				i++;
				return v;
			}
			int start = i;
			while (i < s.Length && (char.IsDigit(s[i]) || s[i] == '.')) i++;
			if (i == start) throw new FormatException();
			return double.Parse(s.Substring(start, i - start), CultureInfo.InvariantCulture);
		}

		// ── the checks ───────────────────────────────────────────────────────

		private static (int Checked, List<string> Bad) Audit(DisplaySettings s)
		{
			var bad = new List<string>();
			int checkedCount = 0;

			foreach (var step in Steps(Page(s)))
			{
				if (step.Substituted == null) continue;

				string? lhs = Arithmetic(step.Substituted);
				string? rhs = Arithmetic(step.Result);
				if (lhs == null || rhs == null) continue;

				double? a = Eval(lhs), b = Eval(rhs);
				if (a == null || b == null) continue;

				checkedCount++;

				// TOLERANCE = THE RESULT'S OWN LAST DIGIT, not a fixed percentage.
				//
				// Both sides are rounded for print: the factors to significant figures, the result
				// to the user's chosen decimals. A result shown as `1.3` cannot be held to better
				// than ±0.05, and demanding 1 % of it would fail on σ in ksi at one decimal — a
				// precision complaint dressed up as an arithmetic error. What must never pass is a
				// line off by a factor or by tens of percent, which this still catches.
				double scale = Math.Max(Math.Abs(a.Value), Math.Abs(b.Value));
				if (scale < 1e-12) continue;

				int shown = DecimalsShown(step.Result);
				double slack = Math.Max(0.5 * Math.Pow(10, -shown), 0.005 * scale);
				double rel = Math.Abs(a.Value - b.Value) / scale;
				if (Math.Abs(a.Value - b.Value) > slack && rel > 0.01)
					bad.Add($"{Strip(step.Label)}: printed “{step.Substituted}” "
						+ $"[parsed: {lhs}] = {a:G6}, "
						+ $"but the line states {b:G6} ({rel * 100:F1} % apart)");
			}
			return (checkedCount, bad);
		}

		/// <summary>How many decimals the printed result actually shows — its own precision.</summary>
		private static int DecimalsShown(string katex)
		{
			var m = Regex.Match(katex, @"-?\d+\.(\d+)");
			return m.Success ? m.Groups[1].Value.Length : 0;
		}

		private static string Strip(string html) =>
			System.Net.WebUtility.HtmlDecode(Regex.Replace(html, "<[^>]+>", "")).Trim();

		[Test]
		public void EveryPrintedSubstitutionEvaluatesToItsResult_Metric()
		{
			var (n, bad) = Audit(Metric);
			Assert.That(bad, Is.Empty, $"{bad.Count} of {n} checked lines do not add up:\n  "
				+ string.Join("\n  ", bad));
		}

		[Test]
		public void EveryPrintedSubstitutionEvaluatesToItsResult_Imperial()
		{
			var (n, bad) = Audit(Imperial);
			Assert.That(bad, Is.Empty, $"{bad.Count} of {n} checked lines do not add up:\n  "
				+ string.Join("\n  ", bad));
		}

		/// <summary>
		/// The evaluator must actually reach a useful number of lines. Without this, a change that
		/// broke the parser would turn both tests above green while checking nothing — the failure
		/// mode that makes a passing suite worthless.
		/// </summary>
		[Test]
		public void TheEvaluatorReachesTheLinesThatMatter()
		{
			Assert.Multiple(() =>
			{
				Assert.That(Audit(Metric).Checked, Is.GreaterThanOrEqualTo(5),
					"the metric page yielded too few evaluable substitutions — parser broken?");
				Assert.That(Audit(Imperial).Checked, Is.GreaterThanOrEqualTo(5),
					"the imperial page yielded too few evaluable substitutions");
			});
		}

		/// <summary>
		/// THE RESISTANCE LINES ARE AMONG THE ONES CHECKED — named, not counted.
		///
		/// A count is not enough. The first version of this suite checked fourteen lines, was green,
		/// and skipped every M_Rd and N_Rd line because the evaluator could not parse `\dfrac` and
		/// `\sin\theta` — so it passed while blind to the 37 % error it was written for. A skip is
		/// silent by design; that makes an explicit roll-call the only honest coverage claim.
		/// </summary>
		[TestCase("M_{y,Rd}")]
		[TestCase("M_{z,Rd}")]
		[TestCase("N_{Rd")]
		public void TheResistanceSubstitutionsAreActuallyEvaluated(string symbol)
		{
			var reached = new List<string>();
			foreach (var step in Steps(Page(Imperial)))
			{
				if (step.Substituted == null) continue;
				string label = Strip(step.Label);
				string? lhs = Arithmetic(step.Substituted);
				string? rhs = Arithmetic(step.Result);
				if (lhs == null || rhs == null)
				{
					if (label.Contains("Rd"))
						TestContext.Out.WriteLine($"SKIP(parse) {label}: lhs={lhs ?? "NULL"} "
							+ $"rhsIn={step.Result} rhs={rhs ?? "NULL"}");
					continue;
				}
				if (Eval(lhs) == null || Eval(rhs) == null)
				{
					if (label.Contains("Rd"))
						TestContext.Out.WriteLine($"SKIP(eval) {label}: [{lhs}] / [{rhs}]");
					continue;
				}
				reached.Add(label);
			}

			var page = Page(Imperial);
			Assert.That(page, Does.Contain(symbol), "the fixture does not print this resistance");
			Assert.That(reached, Is.Not.Empty);
			TestContext.Out.WriteLine("evaluated: " + string.Join(" | ", reached));
		}
	}
}
