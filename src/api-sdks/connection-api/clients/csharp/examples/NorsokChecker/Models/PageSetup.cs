namespace NorsokChecker.Models
{
	/// <summary>
	/// How the page footer is numbered. Three modes, because this report has two lives: it is read
	/// on its own, and it is bound into someone else's calculation package.
	/// </summary>
	internal enum FooterMode
	{
		/// <summary>
		/// "NORSOK N-004 — 4 / 173". The default, and what a standalone report wants: the "n / m"
		/// form is self-scoping, so a reader can cite page 7 of the check without ambiguity.
		/// </summary>
		Local,

		/// <summary>
		/// A single running number from <see cref="PageSetup.FooterStartAt"/>, with NO total.
		/// For a report inserted at a known position in a larger document: printing "47 / 173"
		/// inside a 400-page package states something false about the package.
		/// </summary>
		Continuous,

		/// <summary>
		/// No footer. For a host document that paginates and numbers everything itself — and it is
		/// also the answer to the one real objection to a footer in the bottom margin, which is that
		/// the host's own footer would collide with ours.
		/// </summary>
		Off,
	}

	/// <summary>
	/// The page the PDF report is printed on: size, orientation, the four margins, and whether
	/// background colours reach the paper.
	///
	/// A model rather than fields on the dialog, for one reason: the millimetre-to-inch conversion.
	/// WebView2's print settings are in INCHES while an engineer sets margins in millimetres, and a
	/// slip in that arithmetic prints a silently wrong page — nothing throws, nothing looks broken,
	/// the margins are simply not what was asked for. Here it can be asserted without a window.
	///
	/// Until this existed the export called <c>PrintToPdfAsync(path, null)</c>, taking WebView2's
	/// defaults: 8.5 × 11 in (US LETTER, not A4) and — worse — <c>ShouldPrintBackgrounds = false</c>,
	/// which drops every background colour. The report leans on those: a PASS card is a green wash, a
	/// FAIL red, the utilisation badge is colour-coded on an eleven-band scale. In the exported PDF
	/// all of it was white, so PASS and FAIL were told apart by a 4 px rule and the word alone.
	/// </summary>
	internal sealed class PageSetup
	{
		/// <summary>ISO A4, in millimetres. The only size besides Letter this report needs.</summary>
		internal const double A4WidthMm = 210.0;
		internal const double A4HeightMm = 297.0;

		/// <summary>US Letter, in millimetres — 8.5 × 11 in, WebView2's own default.</summary>
		internal const double LetterWidthMm = 215.9;
		internal const double LetterHeightMm = 279.4;

		private const double MmPerInch = 25.4;

		/// <summary>A4 by default; Letter is what WebView2 would otherwise impose.</summary>
		internal bool IsLetter { get; set; }

		internal bool Landscape { get; set; }

		// The user's own defaults, and deliberately NOT uniform: 15 mm at the sides, 20 mm top and
		// bottom. The settings API takes four separate margins, so there is no reason to average
		// them into one number — and a single Margin field would print 15 mm at the top while
		// satisfying any test that only checked "the margin is 15".
		internal double MarginLeftMm { get; set; } = 15.0;
		internal double MarginRightMm { get; set; } = 15.0;
		internal double MarginTopMm { get; set; } = 20.0;
		internal double MarginBottomMm { get; set; } = 20.0;

		/// <summary>
		/// ON by default. See the class remarks: off is what WebView2 does, and it costs the report
		/// its entire colour vocabulary.
		/// </summary>
		internal bool PrintBackgrounds { get; set; } = true;

		/// <summary>
		/// How the footer numbers pages. <see cref="FooterMode.Local"/> by default — today's
		/// behaviour, and what a standalone report wants.
		/// </summary>
		internal FooterMode FooterMode { get; set; } = FooterMode.Local;

		/// <summary>
		/// First page number in <see cref="FooterMode.Continuous"/>, for a report inserted into a
		/// larger document at a known position. Ignored in the other modes.
		/// </summary>
		internal int FooterStartAt { get; set; } = 1;

		/// <summary>
		/// What the footer calls the document. Empty suppresses it.
		///
		/// Worth keeping available even though "n / m" is self-scoping: in
		/// <see cref="FooterMode.Continuous"/> the number is indistinguishable from the host
		/// document's own, so the label is the only thing telling a reader which document they are
		/// in. Suppressing it is a legitimate choice, not a sensible default.
		/// </summary>
		internal string FooterLabel { get; set; } =
			Services.NorsokHtmlReportGenerator.DefaultFooterLabel;

		internal static double MmToIn(double mm) => mm / MmPerInch;

		/// <summary>The page in inches, orientation applied — what CoreWebView2PrintSettings wants.</summary>
		internal double WidthInches => MmToIn(Landscape ? LongEdgeMm : ShortEdgeMm);

		internal double HeightInches => MmToIn(Landscape ? ShortEdgeMm : LongEdgeMm);

		private double ShortEdgeMm => IsLetter ? LetterWidthMm : A4WidthMm;
		private double LongEdgeMm => IsLetter ? LetterHeightMm : A4HeightMm;

		internal double MarginLeftInches => MmToIn(MarginLeftMm);
		internal double MarginRightInches => MmToIn(MarginRightMm);
		internal double MarginTopInches => MmToIn(MarginTopMm);
		internal double MarginBottomInches => MmToIn(MarginBottomMm);

		/// <summary>
		/// Why the margins cannot simply be handed to the API: every margin setter throws
		/// ArgumentException on a negative value, and PageWidth/PageHeight on anything ≤ 0. A margin
		/// wider than the page is not rejected by the API but leaves no content area, so it is
		/// checked here too — the dialog uses this to refuse a bad setup instead of letting the
		/// export throw halfway through.
		/// </summary>
		internal bool IsValid(out string? error)
		{
			foreach (var (name, mm) in new[]
			{
				("Left", MarginLeftMm), ("Right", MarginRightMm),
				("Top", MarginTopMm), ("Bottom", MarginBottomMm),
			})
			{
				if (mm < 0)
				{
					error = $"{name} margin cannot be negative.";
					return false;
				}
			}

			double pageWidthMm = Landscape ? LongEdgeMm : ShortEdgeMm;
			double pageHeightMm = Landscape ? ShortEdgeMm : LongEdgeMm;

			if (MarginLeftMm + MarginRightMm >= pageWidthMm)
			{
				error = "The side margins leave no room for content.";
				return false;
			}
			if (MarginTopMm + MarginBottomMm >= pageHeightMm)
			{
				error = "The top and bottom margins leave no room for content.";
				return false;
			}

			if (FooterMode == FooterMode.Continuous && FooterStartAt < 1)
			{
				error = "The first page number must be 1 or more.";
				return false;
			}

			error = null;
			return true;
		}

		internal PageSetup Clone() => new()
		{
			IsLetter = IsLetter,
			Landscape = Landscape,
			MarginLeftMm = MarginLeftMm,
			MarginRightMm = MarginRightMm,
			MarginTopMm = MarginTopMm,
			MarginBottomMm = MarginBottomMm,
			PrintBackgrounds = PrintBackgrounds,
			FooterMode = FooterMode,
			FooterStartAt = FooterStartAt,
			FooterLabel = FooterLabel,
		};
	}
}
