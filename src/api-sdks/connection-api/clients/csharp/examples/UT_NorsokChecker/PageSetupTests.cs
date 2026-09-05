using NorsokChecker.Models;

namespace UT_NorsokChecker
{
	/// <summary>
	/// The page the PDF is printed on.
	///
	/// Until this existed the export called <c>PrintToPdfAsync(path, null)</c> and took WebView2's
	/// defaults, which are 8.5 × 11 in — **US Letter, not A4** — and
	/// <c>ShouldPrintBackgrounds = false</c>. The second one is the expensive default: this report
	/// encodes PASS/FAIL and the eleven-band utilisation scale in background colour, so the exported
	/// PDF had a failing joint looking like a passing one. The HTML already asked for the colours
	/// (<c>print-color-adjust: exact</c>) and WebView2 overrode it.
	///
	/// The arithmetic is tested here rather than through the dialog because a millimetre that reaches
	/// the API unconverted prints a wrong page WITHOUT failing — nothing throws, nothing looks broken,
	/// the margins are simply not what was asked for.
	/// </summary>
	[TestFixture]
	public class PageSetupTests
	{
		/// <summary>
		/// The defaults the user asked for: A4, 15 mm at the sides, 20 mm top and bottom.
		///
		/// The four margins are asserted SEPARATELY on purpose. A single uniform Margin field would
		/// satisfy a test that only checked "the margin is 15" while printing 15 mm at the top — the
		/// asymmetry is the requirement, not an accident of it.
		/// </summary>
		[Test]
		public void TheDefaultsAreA4WithAsymmetricMargins()
		{
			var s = new PageSetup();

			Assert.Multiple(() =>
			{
				Assert.That(s.IsLetter, Is.False, "A4, not WebView2's Letter");
				Assert.That(s.Landscape, Is.False, "portrait");

				Assert.That(s.MarginLeftMm, Is.EqualTo(15.0), "left");
				Assert.That(s.MarginRightMm, Is.EqualTo(15.0), "right");
				Assert.That(s.MarginTopMm, Is.EqualTo(20.0), "top — NOT 15");
				Assert.That(s.MarginBottomMm, Is.EqualTo(20.0), "bottom — NOT 15");

				Assert.That(s.PrintBackgrounds, Is.True,
					"on, or the PDF loses the PASS/FAIL colours and the utilisation scale");
			});
		}

		/// <summary>
		/// Millimetres to inches, which is the conversion the print API needs and the one place a
		/// silent error would live.
		/// </summary>
		[Test]
		public void MillimetresConvertToInches()
		{
			Assert.Multiple(() =>
			{
				Assert.That(PageSetup.MmToIn(25.4), Is.EqualTo(1.0).Within(1e-12), "an inch");
				Assert.That(PageSetup.MmToIn(15.0), Is.EqualTo(0.590551).Within(1e-6), "15 mm");
				Assert.That(PageSetup.MmToIn(20.0), Is.EqualTo(0.787402).Within(1e-6), "20 mm");
				Assert.That(PageSetup.MmToIn(0.0), Is.EqualTo(0.0), "zero stays zero");
			});
		}

		/// <summary>
		/// A4 in inches, and the margins exposed in the same unit the API takes.
		///
		/// 210 × 297 mm is 8.268 × 11.693 in. Asserting the INCH properties rather than the mm fields
		/// is the point: a property that returned millimetres would pass every test above and print a
		/// page 25 times too large.
		/// </summary>
		[Test]
		public void A4IsExpressedInInchesForTheApi()
		{
			var s = new PageSetup();

			Assert.Multiple(() =>
			{
				Assert.That(s.WidthInches, Is.EqualTo(8.2677).Within(1e-3), "A4 width");
				Assert.That(s.HeightInches, Is.EqualTo(11.6929).Within(1e-3), "A4 height");
				Assert.That(s.MarginLeftInches, Is.EqualTo(0.5906).Within(1e-3));
				Assert.That(s.MarginTopInches, Is.EqualTo(0.7874).Within(1e-3));
			});
		}

		/// <summary>Landscape swaps the edges; it does not rotate the margins.</summary>
		[Test]
		public void LandscapeSwapsTheEdges()
		{
			var portrait = new PageSetup();
			var landscape = new PageSetup { Landscape = true };

			Assert.Multiple(() =>
			{
				Assert.That(landscape.WidthInches, Is.EqualTo(portrait.HeightInches).Within(1e-9));
				Assert.That(landscape.HeightInches, Is.EqualTo(portrait.WidthInches).Within(1e-9));
				Assert.That(landscape.WidthInches, Is.GreaterThan(landscape.HeightInches),
					"landscape is wider than tall — a swap that did nothing would pass the two above "
					+ "if width and height were equal");
			});
		}

		/// <summary>Letter is the other size, and it is NOT A4 — the two must not collapse.</summary>
		[Test]
		public void LetterIsADifferentSizeFromA4()
		{
			var a4 = new PageSetup();
			var letter = new PageSetup { IsLetter = true };

			Assert.Multiple(() =>
			{
				Assert.That(letter.WidthInches, Is.EqualTo(8.5).Within(1e-2), "8.5 in wide");
				Assert.That(letter.HeightInches, Is.EqualTo(11.0).Within(1e-2), "11 in tall");
				Assert.That(letter.HeightInches, Is.Not.EqualTo(a4.HeightInches).Within(1e-2),
					"Letter is shorter than A4 — that difference is why the size is selectable");
			});
		}

		/// <summary>
		/// A setup the print API would reject, or that leaves no content area, is refused HERE so the
		/// export does not throw halfway through writing a file.
		/// </summary>
		[TestCase(-1.0, 15.0, 20.0, 20.0, TestName = "negative left margin")]
		[TestCase(15.0, -5.0, 20.0, 20.0, TestName = "negative right margin")]
		[TestCase(15.0, 15.0, -1.0, 20.0, TestName = "negative top margin")]
		[TestCase(120.0, 120.0, 20.0, 20.0, TestName = "sides wider than A4")]
		[TestCase(15.0, 15.0, 160.0, 160.0, TestName = "top and bottom taller than A4")]
		public void AnUnprintableSetupIsRejected(double l, double r, double t, double b)
		{
			var s = new PageSetup
			{
				MarginLeftMm = l, MarginRightMm = r, MarginTopMm = t, MarginBottomMm = b,
			};

			Assert.Multiple(() =>
			{
				Assert.That(s.IsValid(out string? error), Is.False);
				Assert.That(error, Is.Not.Null.And.Not.Empty, "and it says which margin");
			});
		}

		/// <summary>The known-good row: the defaults must pass the same validator.</summary>
		[Test]
		public void TheDefaultSetupIsValid()
		{
			Assert.That(new PageSetup().IsValid(out string? error), Is.True, error);
		}

		// WHY THE REPORT DOES NOT NUMBER ITS OWN PAGES — kept here as the record, because the
		// numbering was built, shipped and taken out, and this is what stops it being rebuilt the
		// same way.
		//
		// An offset page number CANNOT be rendered in a page margin box on this engine. Measured on
		// WebView2 1.0.2903.40 by printing three pages under six rules
		// (PrintedPageProbe.WhatCounterResetDoesToPageNumbering):
		//
		//   plain, no reset                   1,  2,  3
		//   @page { counter-reset: page 76 }  76, 76, 76   the reset re-applies on EVERY page
		//   @page:first { reset 77 }          77,  1,  2   applies once, then counting restarts
		//   reset + counter-increment         77, 77, 77
		//   document counter, in the content  77, 78, 79   works — but not in a margin box
		//   document counter, in @page box     0,  0,  0   page context sees only page/pages
		//
		// Reported from the running app as "start at 77 and every page says 76", which is both
		// halves of that at once. A setting that cannot keep its promise is worse than no setting,
		// so the footer is gone and the reader numbers whatever document this is bound into.
		//
		// If a PDF post-processing pass is ever added — the one /Outlines and the document
		// properties also need — numbering becomes possible there, where an offset is just an
		// integer. FooterCss is left as that seam.
		//
		// The GUARD is ReportPaginationTests.TheFooterIsEmpty, which asserts the same two facts
		// (FooterCss says `content: none`, no `counter(page)`) and additionally reads the rendered
		// document. A second test here asserted only the CSS half, so the two failed together on one
		// change and the weaker could not see the document at all.

		/// <summary>
		/// The export USES the setup — it does not pass null and take WebView2's defaults.
		///
		/// This is the assertion the whole class depends on, and the one nothing else covers: a
		/// PageSetup that is correct in every respect changes nothing if the export ignores it.
		/// Measured — reverting `PrintToPdfAsync(path, print)` to `(path, null)` left all 338 other
		/// tests green, which is exactly how the Letter-and-no-backgrounds default survived unnoticed
		/// until it was read off a PDF.
		///
		/// On the SOURCE, because the alternative is driving a real export: PrintToPdfAsync needs an
		/// initialised WebView2 and writes a file, and neither belongs in a unit test.
		/// </summary>
		[Test]
		public void TheExportPassesTheSetupToThePrinter()
		{
			var dir = new System.IO.DirectoryInfo(System.IO.Path.GetDirectoryName(
				System.Reflection.Assembly.GetExecutingAssembly().Location)!);
			while (dir != null && !System.IO.Directory.Exists(System.IO.Path.Combine(dir.FullName, "NorsokChecker")))
				dir = dir.Parent;
			if (dir == null) Assert.Fail("cannot locate the NorsokChecker source from the test output — this test "
				+ "reads the source, and skipping it would report a pass it did not earn");

			// Comments stripped: the prose there names PrintToPdfAsync and null while explaining the
			// defect, so a raw match would find the explanation.
			string code = System.Text.RegularExpressions.Regex.Replace(
				System.IO.File.ReadAllText(System.IO.Path.Combine(
					dir!.FullName, "NorsokChecker", "MainWindow.Report.cs")),
				@"//[^\n]*", "");

			Assert.Multiple(() =>
			{
				Assert.That(code, Does.Not.Contain("PrintToPdfAsync(norsokPdf, null)"),
					"null takes WebView2's defaults: US Letter and backgrounds OFF");
				Assert.That(code, Does.Contain("CreatePrintSettings()"),
					"the export builds real settings");
				foreach (string p in new[]
				{
					"PageWidth", "PageHeight",
					"MarginLeft", "MarginRight", "MarginTop", "MarginBottom",
					"ShouldPrintBackgrounds",
				})
					Assert.That(code, Does.Contain($"print.{p} ="), $"{p} is set from the setup");
			});
		}

		/// <summary>
		/// Clone is a real copy. The dialog edits a clone so Cancel really cancels — a shallow
		/// hand-back would leave half an edited setup behind when the user changed their mind.
		/// </summary>
		[Test]
		public void CloneDoesNotShareState()
		{
			var original = new PageSetup();
			var copy = original.Clone();

			copy.MarginTopMm = 33.0;
			copy.IsLetter = true;
			copy.PrintBackgrounds = false;

			Assert.Multiple(() =>
			{
				Assert.That(original.MarginTopMm, Is.EqualTo(20.0), "the original keeps its margin");
				Assert.That(original.IsLetter, Is.False);
				Assert.That(original.PrintBackgrounds, Is.True);
			});
		}







		/// <summary>
		/// The app HANDS the footer rule to the report. Every test above can pass while the export
		/// ignores the settings entirely — measured three times in this project: a change that is
		/// correct, fully unit-tested and connected to nothing leaves the suite green.
		///
		/// Matched inside the CALL's parentheses, not anywhere in the file: the word "FooterCss"
		/// stays alive in the file after the argument is deleted, because the method is defined
		/// there too.
		/// </summary>
		[Test]
		public void TheAppPassesTheFooterRuleToTheReport()
		{
			var dir = new System.IO.DirectoryInfo(System.IO.Path.GetDirectoryName(
				System.Reflection.Assembly.GetExecutingAssembly().Location)!);
			while (dir != null && !System.IO.Directory.Exists(
				System.IO.Path.Combine(dir.FullName, "NorsokChecker")))
				dir = dir.Parent;
			if (dir == null) Assert.Fail("cannot locate the NorsokChecker source from the test output — this test "
				+ "reads the source, and skipping it would report a pass it did not earn");

			string code = System.Text.RegularExpressions.Regex.Replace(
				System.IO.File.ReadAllText(System.IO.Path.Combine(
					dir!.FullName, "NorsokChecker", "MainWindow.Report.cs")),
				@"//[^\n]*", "");

			var call = System.Text.RegularExpressions.Regex.Match(code,
				@"GenerateReport\(([^;]*?)\);", System.Text.RegularExpressions.RegexOptions.Singleline);

			Assert.Multiple(() =>
			{
				Assert.That(call.Success, Is.True, "the app builds a report at all");
				Assert.That(call.Groups[1].Value, Does.Contain("FooterCss"),
					"and passes the footer rule as an argument");
				Assert.That(call.Groups[1].Value, Does.Contain("_pageSetup"),
					"built from the export's own page setup, not from a default");
			});
		}

	}
}
