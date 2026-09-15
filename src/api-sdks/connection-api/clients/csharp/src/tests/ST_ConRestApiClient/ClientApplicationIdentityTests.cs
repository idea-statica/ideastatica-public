using IdeaStatiCa.ConnectionApi;

namespace ST_ConRestApiClient
{
	/// <summary>
	/// What an application is allowed to call itself towards the service. The value travels in a
	/// request header on every call, so the rules are about what a header can safely carry.
	/// </summary>
	[TestFixture]
	public class ClientApplicationIdentityTests
	{
		[Test]
		public void AnApplicationAndItsVersion_AreJoinedByASlash()
		{
			Assert.Multiple(() =>
			{
				Assert.That(ClientApplicationIdentity.Format("NorsokChecker", "1.4.2"),
					Is.EqualTo("NorsokChecker/1.4.2"));
				Assert.That(ClientApplicationIdentity.Format("NorsokChecker"),
					Is.EqualTo("NorsokChecker"), "the version is optional");
				Assert.That(ClientApplicationIdentity.Format(" NorsokChecker ", " 1.4.2 "),
					Is.EqualTo("NorsokChecker/1.4.2"), "trimmed, because a name is typed by a developer");
			});
		}

		/// <summary>
		/// Nothing to send is not an error - it is the previous behaviour, where calls are counted but
		/// not attributed.
		/// </summary>
		[Test]
		public void WithoutAName_ThereIsNoHeaderToSend()
		{
			Assert.Multiple(() =>
			{
				Assert.That(ClientApplicationIdentity.Format(null), Is.Null);
				Assert.That(ClientApplicationIdentity.Format(""), Is.Null);
				Assert.That(ClientApplicationIdentity.Format("   "), Is.Null);
				Assert.That(ClientApplicationIdentity.Format(null, "1.4.2"), Is.Null,
					"a version without a name identifies nothing");
			});
		}

		/// <summary>
		/// A header value with a control character in it is rejected outright by some HTTP stacks and
		/// mangled by others, so a careless name must not be able to break every request the client
		/// makes. The value stays recognisable rather than disappearing.
		/// </summary>
		[Test]
		public void AnythingAHeaderCannotCarry_IsReplacedRatherThanSent()
		{
			Assert.Multiple(() =>
			{
				Assert.That(ClientApplicationIdentity.Format("Norsok\r\nChecker"),
					Is.EqualTo("Norsok__Checker"), "a header injection attempt is not a name");
				Assert.That(ClientApplicationIdentity.Format("Norsok\tChecker"),
					Is.EqualTo("Norsok_Checker"));
				Assert.That(ClientApplicationIdentity.Format("Kontrola spojů"),
					Is.EqualTo("Kontrola spoj_"), "printable ASCII only");
				Assert.That(ClientApplicationIdentity.Format("Norsok/Checker", "1.0"),
					Is.EqualTo("Norsok_Checker/1.0"), "the slash is reserved for joining the version");
			});
		}

		[Test]
		public void ALongName_IsCutRatherThanSentWhole()
		{
			string value = ClientApplicationIdentity.Format(new string('x', 100));

			Assert.That(value, Has.Length.EqualTo(ClientApplicationIdentity.MaxLength));
			Assert.That(value, Is.EqualTo(new string('x', ClientApplicationIdentity.MaxLength)));
		}

		/// <summary>
		/// Attribution is by application AND version, so a version must never arrive half-written: a
		/// value cut mid-field reads as a different version, which is worse than no version at all.
		/// The name is what gives way.
		/// </summary>
		[Test]
		public void WhenBothDoNotFit_TheNameGivesWayAndTheVersionStaysWhole()
		{
			Assert.Multiple(() =>
			{
				Assert.That(ClientApplicationIdentity.Format(new string('x', 63), "1.4.2"),
					Is.EqualTo(new string('x', 58) + "/1.4.2"),
					"no dangling separator and no lost version");
				Assert.That(ClientApplicationIdentity.Format(new string('x', 60), "1.4.2"),
					Is.EqualTo(new string('x', 58) + "/1.4.2"),
					"1.4.2 must not arrive as 1.4");
				Assert.That(ClientApplicationIdentity.Format("Norsok", new string('9', 100)),
					Is.EqualTo("Norsok"),
					"a version that cannot fit whole is dropped, not trimmed");
			});

			Assert.That(ClientApplicationIdentity.Format(new string('x', 100), "1.4.2"),
				Has.Length.EqualTo(ClientApplicationIdentity.MaxLength));
		}
	}
}
