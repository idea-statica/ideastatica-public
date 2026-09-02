namespace UT_NorsokChecker
{
	/// <summary>
	/// The Results overview shortens a title to its subject, dropping what the per-chapter tab
	/// already carries.
	///
	/// Every input below is a real title the app produces. Worth its own fixture because the rule
	/// looks simple and is not: a §6.4 title uses an em dash BOTH to separate the subject from the
	/// brace ("Tubular Joint — M1") and to append the elaboration ("— outside validity range"), so
	/// cutting at the first one loses the brace name and cutting at none of them keeps everything.
	/// </summary>
	[TestFixture]
	public class ShortTitleTests
	{
		[TestCase("Tubular Joint — M1 (K 100% / Y 0% / X 0%)",
			"Tubular Joint — M1",
			"the K/Y/X split is a column on the §6.4 tab")]
		[TestCase("Tubular Joint — M1 (K 0% / Y 0% / X 100%) — outside validity range (6.4.3.1)",
			"Tubular Joint — M1",
			"and so is the validity range")]
		[TestCase("Tubular Joint — M3 — outside validity range (6.4.3.1)",
			"Tubular Joint — M3",
			"the elaboration can arrive without a bracket")]
		[TestCase("Outside the scope of §6.4", "Outside the scope of §6.4",
			"nothing to strip")]
		[TestCase("Assumption", "Assumption", "a note keeps its one word")]
		// No "§6.4" in front of it any more: the card prints §{Section} beside the title, so carrying
		// the clause here rendered "§6.4 §6.4 could not be evaluated" in the exported report.
		[TestCase("Could not be evaluated", "Could not be evaluated",
			"and so does the blocked-chapter row")]
		public void ATitleKeepsItsSubjectAndLosesTheRest(string title, string expected, string why)
		{
			Assert.That(NorsokChecker.MainWindow.ShortTitle(title), Is.EqualTo(expected), why);
		}

		/// <summary>Empty and null survive — the grid binds whatever comes back.</summary>
		[TestCase(null)]
		[TestCase("")]
		public void NothingInNothingOut(string? title)
		{
			Assert.That(NorsokChecker.MainWindow.ShortTitle(title), Is.Empty);
		}
	}
}
