using NorsokChecker.Services.Norsok64;

namespace UT_NorsokChecker
{
	/// <summary>
	/// Pins the two distinct reasons §6.4 can refuse a member's section, which the app used to
	/// collapse into one: "not a tubular section" and "tubular, but its dimensions are unknown".
	///
	/// The old behaviour said `not CHS (CHS 219.1/10)` about a section that plainly is a CHS —
	/// factually wrong, and it sent the reader looking for the wrong problem. The python reference
	/// separates them (extract.py `_section_reject`), and PYTHON_STOPGAP.md said this would be
	/// carried over.
	/// </summary>
	[TestFixture]
	public class SectionRejectionTests
	{
		[Test]
		public void ATubularSectionWithDimensions_IsAccepted()
		{
			var s = new JointSectionInfo
			{
				Name = "CHS 219.1/10", TypeName = "RolledCHS",
				IsCHS = true, D = 219.1, T = 10.0,
			};

			Assert.That(s.RejectReason("Chord"), Is.Null);
			Assert.That(s.HasDimensions, Is.True);
		}

		/// <summary>The regression: a tube must not be told it is not a tube.</summary>
		[Test]
		public void ATubularSectionWithoutDimensions_SaysSo_NotThatItIsNotCHS()
		{
			var s = new JointSectionInfo
			{
				Name = "CHS 219.1/10", TypeName = "RolledCHS",
				IsCHS = true, D = null, T = null,
			};

			string? why = s.RejectReason("M3");

			Assert.That(why, Is.Not.Null);
			Assert.Multiple(() =>
			{
				Assert.That(why, Does.Contain("tubular but its D/T are unknown"));
				Assert.That(why, Does.Not.Contain("not CHS"),
					"a tubular section must never be reported as not tubular");
				Assert.That(why, Does.Contain("M3"), "the message names the member");
			});
		}

		/// <summary>A genuinely non-tubular section names its real type.</summary>
		[Test]
		public void ANonTubularSection_NamesItsType()
		{
			var s = new JointSectionInfo
			{
				Name = "IPE100", TypeName = "RolledI", IsCHS = false, D = 100, T = 4.1,
			};

			string? why = s.RejectReason("M3");

			Assert.Multiple(() =>
			{
				Assert.That(why, Does.Contain("IPE100"));
				Assert.That(why, Does.Contain("RolledI"), "the real section type, not just 'not CHS'");
				Assert.That(why, Does.Contain("tubular (circular hollow) sections only"));
			});
		}

		/// <summary>The geometry note explains WHY the dimensions are missing when it is known.</summary>
		[Test]
		public void TheGeometryNoteIsCarriedIntoTheMessage()
		{
			var s = new JointSectionInfo
			{
				Name = "PIPE127STD", TypeName = "RolledCHS", IsCHS = true,
				GeomNote = "only 3 facet(s) in the model — not a modelled tube wall",
			};

			Assert.That(s.RejectReason("M1"), Does.Contain("only 3 facet(s)"));
		}

		/// <summary>Falls back to a plain reason rather than an empty sentence.</summary>
		[Test]
		public void WithoutAGeometryNoteTheMessageStillReads()
		{
			var s = new JointSectionInfo { Name = "CHS30,3", TypeName = "RolledCHS", IsCHS = true };

			Assert.That(s.RejectReason("M5"),
				Does.Contain("dimensions could not be read from the model"));
		}
	}
}
