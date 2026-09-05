using System.IO;
using NorsokChecker.Services;

namespace UT_NorsokChecker
{
	/// <summary>
	/// Finding the REST API service on this machine.
	///
	/// The SDK's ConnectionApiServiceRunner does none of this: it always starts a NEW service (so a
	/// second licence seat), never checks the version (so a 25.1 path fails later as a bare 404),
	/// and is cleaned up only by Dispose (so a killed app leaks the seat). These cover the part that
	/// can be tested without a service running.
	/// </summary>
	[TestFixture]
	public class ServiceLocatorTests
	{
		/// <summary>
		/// The floor is 26.0 because /api/4 — every endpoint this app calls — does not exist before
		/// it. A 25.1 service answers UnsupportedApiVersion on all of them.
		/// </summary>
		[Test]
		public void TheMinimumVersionIs26Point0()
		{
			Assert.That(ServiceLocator.MinVersion, Is.EqualTo(new Version(26, 0)));
		}

		[TestCase(@"C:\Program Files\IDEA StatiCa\StatiCa 26.0", 26, 0)]
		[TestCase(@"C:\Program Files\IDEA StatiCa\StatiCa 25.1", 25, 1)]
		// the standalone service install: its folder does NOT match "StatiCa <maj>.<min>", which is
		// the only shape the python reference looks for, so python would miss this machine's copy
		[TestCase(@"C:\Program Files\IDEA StatiCa\Connection API 26.1", 26, 1)]
		[TestCase(@"D:\builds\StatiCa 26.1\", 26, 1)]
		public void AVersionIsReadOutOfTheFolderName(string dir, int major, int minor)
		{
			Assert.That(ServiceLocator.VersionOfFolder(dir), Is.EqualTo(new Version(major, minor)));
		}

		[Test]
		public void AFolderWithNoVersionInItsNameReadsAsZero()
		{
			Assert.That(ServiceLocator.VersionOfFolder(@"C:\tools\connection-api").Major, Is.EqualTo(0));
		}

		/// <summary>
		/// The version a running service reports is a four-part string; only major.minor decides.
		/// An unparseable one is ACCEPTED: refusing to run over a format nobody has seen would be
		/// worse than trying and reporting what the service then says.
		/// </summary>
		[TestCase("26.0.5.1259", true)]
		[TestCase("25.1.5.1504", false)]
		[TestCase("24.0.0.1", false)]
		[TestCase("", true)]
		[TestCase(null, true)]
		[TestCase("something unexpected", true)]
		// 26.1 is covered by AReportedVersionIsCheckedAgainstBothEnds, which is where the ceiling
		// belongs — this case used to assert it was supported, and the measurement disproved that.
		public void SupportIsDecidedByMajorMinor(string? version, bool supported)
		{
			Assert.That(ServiceLocator.IsSupported(version), Is.EqualTo(supported));
		}

		/// <summary>
		/// Probing a port nothing listens on must return null rather than throw — "is one running"
		/// and "which one" are the same question, and an exception would have to be caught by every
		/// caller anyway.
		/// </summary>
		[Test]
		public async Task ProbingADeadPortReturnsNull()
		{
			// 9 is the discard port; nothing serves HTTP there
			var version = await ServiceLocator.RunningVersionAsync("http://localhost:9", timeoutSeconds: 1);

			Assert.That(version, Is.Null);
		}

		/// <summary>
		/// A scan of a directory that does not exist yields nothing instead of throwing.
		/// </summary>
		[Test]
		public void ScanningAMissingRootYieldsNothing()
		{
			Assert.That(ServiceLocator.FindInstalls(@"Z:\no such place"), Is.Empty);
		}

		/// <summary>
		/// A synthetic install tree, to prove the scan finds an exe and reads its version without
		/// depending on what happens to be installed on the machine running the test.
		/// </summary>
		[Test]
		public void TheScanFindsAnExeAndOrdersPreferredFirst()
		{
			string root = Path.Combine(Path.GetTempPath(), "norsok-locator-" + Guid.NewGuid().ToString("N"));
			try
			{
				foreach (string name in new[] { "StatiCa 25.1", "StatiCa 26.0", "StatiCa 26.1", "Whatever" })
				{
					string dir = Path.Combine(root, name);
					Directory.CreateDirectory(dir);
					File.WriteAllText(Path.Combine(dir, ServiceLocator.ExeName), "not a real exe");
				}

				var found = ServiceLocator.FindInstalls(root);

				Assert.Multiple(() =>
				{
					Assert.That(found, Has.Count.EqualTo(4), "one per folder holding the exe");
					Assert.That(found[0].Version, Is.EqualTo(ServiceLocator.PreferredVersion),
						"the preferred version comes first even though it is not the newest");
					Assert.That(found[1].Version, Is.EqualTo(new Version(26, 1)),
						"then newest down");
					Assert.That(found[2].Version, Is.EqualTo(new Version(25, 1)));
					Assert.That(found[3].Version.Major, Is.EqualTo(0),
						"a folder with no version in its name sorts last but is still a candidate");
				});
			}
			finally
			{
				try { Directory.Delete(root, recursive: true); } catch { }
			}
		}

		/// <summary>
		/// An installation OUTSIDE the conventional Program Files root is found, because the
		/// registry names its real directory. This is the case the registry lookup exists for — a
		/// pure directory scan of `C:\Program Files\IDEA StatiCa` would miss it entirely.
		///
		/// Simulated by pointing the scan at an arbitrary root, which is what the registry's
		/// InstallDir64 amounts to: a path from somewhere other than the convention.
		/// </summary>
		[Test]
		public void AnInstallationOutsideProgramFilesIsFound()
		{
			// deliberately not under Program Files, and not named like the convention either
			string root = Path.Combine(Path.GetTempPath(), "idea-elsewhere-" + Guid.NewGuid().ToString("N"));
			string dir = Path.Combine(root, "StatiCa 26.0");
			try
			{
				Directory.CreateDirectory(dir);
				File.WriteAllText(Path.Combine(dir, ServiceLocator.ExeName), "stub");

				var found = ServiceLocator.FindInstalls(root);

				Assert.Multiple(() =>
				{
					Assert.That(found, Has.Count.EqualTo(1));
					Assert.That(found[0].Directory, Is.EqualTo(dir));
					Assert.That(found[0].Directory, Does.Not.Contain("Program Files"),
						"the point of the test: the path is nowhere near the conventional root");
					Assert.That(found[0].Version, Is.EqualTo(new Version(26, 0)));
				});
			}
			finally { try { Directory.Delete(root, true); } catch { } }
		}

		/// <summary>
		/// The version ceiling. 26.1 IS installed on developer machines and must NOT be selected:
		/// measured 2026-08-27, the service answers every call but this app's API client (26.0.4)
		/// deserialises its IOM export to null, and §6.4 then rejects sound joints for overlapping
		/// feet. A wrong answer, not a visible failure — hence a hard exclusion.
		/// </summary>
		[Test]
		public void AVersionNewerThanTheClientSupportsIsNotUsable()
		{
			Assert.Multiple(() =>
			{
				Assert.That(ServiceLocator.IsUsableVersion(new Version(26, 0)), Is.True);
				Assert.That(ServiceLocator.IsUsableVersion(new Version(26, 1)), Is.False,
					"26.1 answers calls but its IOM cannot be read by the referenced client");
				Assert.That(ServiceLocator.IsUsableVersion(new Version(27, 0)), Is.False);
				Assert.That(ServiceLocator.IsUsableVersion(new Version(25, 1)), Is.False, "below /api/4");
				Assert.That(ServiceLocator.IsUsableVersion(new Version(0, 0)), Is.True,
					"a folder that states no version is still a candidate — it may be the right one");
			});
		}

		/// <summary>The same ceiling on a version STRING, as a running service reports it.</summary>
		[TestCase("26.0.5.1259", true)]
		[TestCase("26.0.6.0235", true)]
		[TestCase("26.1.0.2007", false)]
		[TestCase("27.0.0.1", false)]
		[TestCase("25.1.5.1504", false)]
		public void AReportedVersionIsCheckedAgainstBothEnds(string version, bool supported)
		{
			Assert.That(ServiceLocator.IsSupported(version), Is.EqualTo(supported));
		}

		/// <summary>
		/// With both installed, the scan offers 26.0 and the filter drops 26.1 — the combination
		/// that matters on this machine.
		/// </summary>
		[Test]
		public void With26Point0And26Point1PresentOnly26Point0IsUsable()
		{
			string root = Path.Combine(Path.GetTempPath(), "norsok-ceiling-" + Guid.NewGuid().ToString("N"));
			try
			{
				foreach (string name in new[] { "StatiCa 26.0", "StatiCa 26.1" })
				{
					string dir = Path.Combine(root, name);
					Directory.CreateDirectory(dir);
					File.WriteAllText(Path.Combine(dir, ServiceLocator.ExeName), "stub");
				}

				var usable = ServiceLocator.FindInstalls(root)
					.Where(i => ServiceLocator.IsUsableVersion(i.Version)).ToList();

				Assert.Multiple(() =>
				{
					Assert.That(usable, Has.Count.EqualTo(1), "26.1 must be excluded");
					Assert.That(usable[0].Version, Is.EqualTo(new Version(26, 0)));
				});
			}
			finally { try { Directory.Delete(root, true); } catch { } }
		}

		/// <summary>
		/// A folder without the exe is not an install, however it is named — otherwise the app would
		/// hand the runner a path that fails with FileNotFoundException later.
		/// </summary>
		[Test]
		public void AFolderWithoutTheExeIsNotAnInstall()
		{
			string root = Path.Combine(Path.GetTempPath(), "norsok-locator-" + Guid.NewGuid().ToString("N"));
			try
			{
				Directory.CreateDirectory(Path.Combine(root, "StatiCa 26.0"));   // no exe inside

				Assert.That(ServiceLocator.FindInstalls(root), Is.Empty);
			}
			finally
			{
				try { Directory.Delete(root, recursive: true); } catch { }
			}
		}
	}

	/// <summary>
	/// Reading the installed versions out of the registry — the part that answers "which versions
	/// are on this machine", which a single CurrentInstallDir value cannot.
	///
	/// Measured on the development machine 2026-08-27, with four installs present:
	///   HKLM\SOFTWARE\IDEAStatiCa\{25.1,26.0,26.1}\IDEAStatiCa\Designer  InstallDir64 + Version64
	///   HKLM\SOFTWARE\IDEAStatiCa\ConnectionApi\26.1                     InstallDir   + Version
	/// The python reference does not read these and its comment calls them "user details, not
	/// paths" — wrong, the paths sit one level deeper than it looked.
	///
	/// Explicit: the result depends on what is installed, so it reports rather than asserts a count.
	///
	/// NOT A TEST, and named accordingly. Both branches end in Assert.Pass — it is structurally
	/// incapable of failing, which is right for a diagnostic and wrong for anything called a test.
	/// It was `RegistryInstallTests`, and a fixture named Tests is read as coverage.
	/// </summary>
	[TestFixture, Explicit("Depends on what is installed on this machine")]
	[Category("Live")]
	public class RegistryInstallProbe
	{
		[Test]
		public void ReportWhatTheRegistryAndTheScanEachFind()
		{
			var fromRegistry = ServiceLocator.FromRegistry();
			TestContext.Out.WriteLine($"registry: {fromRegistry.Count} install(s)");
			foreach (var i in fromRegistry)
				TestContext.Out.WriteLine($"  {i.Label,-6} {i.Version}  {i.Directory}");

			var all = ServiceLocator.FindInstalls();
			TestContext.Out.WriteLine($"\nregistry + scan: {all.Count} install(s), best first");
			foreach (var i in all)
				TestContext.Out.WriteLine($"  {i.Label,-6} {i.Version}  {i.Directory}");

			TestContext.Out.WriteLine($"\nCurrentInstallDir: {ServiceLocator.RegistryInstallDir() ?? "(none)"}");

			// The claim being verified: the registry enumerates MORE than the one path
			// CurrentInstallDir holds. On a machine with a single install these are equal and the
			// test says so rather than failing.
			if (fromRegistry.Count > 1)
				Assert.Pass($"the registry enumerated {fromRegistry.Count} versions — "
					+ "more than CurrentInstallDir's single value");
			else
				Assert.Pass($"only {fromRegistry.Count} install(s) in the registry on this machine; "
					+ "the enumeration cannot be demonstrated here");
		}
	}
}
