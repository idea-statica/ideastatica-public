using NorsokChecker.Services;

namespace UT_NorsokChecker
{
	/// <summary>
	/// The report and the derivation window must typeset their formulas WITHOUT INTERNET.
	///
	/// They used to load KaTeX from cdn.jsdelivr.net, so on an offline machine the equations came
	/// out as raw LaTeX source — `$$\dfrac{f_y T^2}{\gamma_M \sin\theta}$$` — which is unreadable in
	/// a document that is a deliverable. KaTeX is now embedded (0.62 MB, MIT, see
	/// Resources/THIRD_PARTY_NOTICES.md).
	///
	/// This is worth a test rather than a look, because BOTH halves fail silently: a csproj that
	/// loses its EmbeddedResource lines still compiles, and a stray CDN URL still renders — as long
	/// as the machine happens to be online. Neither shows up in a build log.
	/// </summary>
	[TestFixture]
	public class OfflineFormulaTests
	{
		[Test]
		public void TheKatexResourcesAreEmbeddedInTheAssembly()
		{
			Assert.That(NorsokHtmlReportGenerator.KatexIsEmbedded, Is.True,
				"katex.min.js / katex.min.css / katex-auto-render.min.js must be embedded resources; "
				+ "check the EmbeddedResource entries in NorsokChecker.csproj");
		}

		/// <summary>A derivation page carries the library inline and reaches for nothing.</summary>
		[Test]
		public void AGeneratedPageFetchesNothingFromTheNetwork()
		{
			string html = NorsokHtmlReportGenerator.GenerateDerivationPage(SkippedRow(), "M1");

			Assert.Multiple(() =>
			{
				Assert.That(html, Does.Not.Contain("cdn.jsdelivr.net"),
					"no CDN — that is the whole point");
				Assert.That(html, Does.Not.Contain("https://"),
					"nothing may be fetched over the network at all");
				Assert.That(html, Does.Contain("renderMathInElement"),
					"and the typesetting must still be invoked");
			});
		}

		/// <summary>
		/// The library really is IN the page, not merely referenced. A page that names
		/// renderMathInElement without shipping it would pass the test above and render nothing.
		/// </summary>
		[Test]
		public void TheLibraryTravelsWithThePage()
		{
			string html = NorsokHtmlReportGenerator.GenerateDerivationPage(SkippedRow(), "M1");

			Assert.Multiple(() =>
			{
				// 269 kB of JS plus 359 kB of CSS: a page that carries them cannot be small
				Assert.That(html.Length, Is.GreaterThan(500_000),
					$"the page is {html.Length} chars — too small to contain KaTeX");
				Assert.That(html, Does.Contain("data:font/woff2;base64,"),
					"the fonts must be inline too, or the glyphs fall back offline");
			});
		}

		/// <summary>
		/// No relative font URL survives. This is the failure the inlining exists to prevent: the
		/// page is handed to WebView2 as a string, so `url(fonts/…)` has no base to resolve against
		/// and would fail even where the files exist.
		/// </summary>
		[Test]
		public void NoFontIsLoadedFromARelativePath()
		{
			string html = NorsokHtmlReportGenerator.GenerateDerivationPage(SkippedRow(), "M1");

			Assert.That(html, Does.Not.Contain("url(fonts/"),
				"a relative font path cannot resolve in a NavigateToString page");
		}

		/// <summary>
		/// A minimal row: the page has to be generated for something, and a skipped brace exercises
		/// the shortest path through the renderer.
		/// </summary>
		private static NorsokChecker.Services.Norsok64.JointCheckRow SkippedRow() => new()
		{
			Name = "M1",
			Skipped = true,
			Reason = "no transverse force",
		};
	}
}
