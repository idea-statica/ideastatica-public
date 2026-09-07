using IdeaStatiCa.ConnectionApi;

namespace ST_ConRestApiClient
{
	/// <summary>
	/// The service locator, against a synthetic installation tree rather than the machine's own - so the
	/// test says the same thing on a build agent with no IDEA StatiCa installed as on a developer's
	/// machine with four of them.
	/// </summary>
	[TestFixture]
	public class ConnectionApiServiceLocatorTests
	{
		private string _root = null!;

		[SetUp]
		public void CreateInstallationTree()
		{
			_root = Path.Combine(Path.GetTempPath(), "ConApiLocatorTests", Guid.NewGuid().ToString("N"));

			// the root as an installation itself (a portable copy), two versioned ones beside it, and a
			// directory that is not an installation at all
			Executable(_root);
			Executable(Path.Combine(_root, "StatiCa 26.0"));
			Executable(Path.Combine(_root, "Connection API 26.1"));
			Directory.CreateDirectory(Path.Combine(_root, "Documentation"));
		}

		[TearDown]
		public void RemoveInstallationTree()
		{
			try { Directory.Delete(_root, recursive: true); } catch (IOException) { }
		}

		private static void Executable(string directory)
		{
			Directory.CreateDirectory(directory);
			File.WriteAllText(Path.Combine(directory, ConnectionApiServiceLocator.ExecutableName), "");
		}

		[Test]
		public void FindInstallations_TakesTheRootAndItsSubdirectories()
		{
			var found = ConnectionApiServiceLocator.FindInstallations(_root);

			Assert.That(found.Select(i => i.Directory), Is.EquivalentTo(new[]
			{
				_root,
				Path.Combine(_root, "StatiCa 26.0"),
				Path.Combine(_root, "Connection API 26.1"),
			}), "a directory without the executable is not an installation");

			Assert.That(found.Select(i => i.ExecutablePath),
				Is.All.EndsWith(ConnectionApiServiceLocator.ExecutableName));
		}

		[Test]
		public void FindInstallations_OfARootThatDoesNotExist_IsEmptyRatherThanAFailure()
		{
			Assert.That(
				ConnectionApiServiceLocator.FindInstallations(Path.Combine(_root, "not installed here")),
				Is.Empty);
		}

		[Test]
		public void FindInstallations_ReadsTheVersionFromTheExecutable()
		{
			// these are empty files, so there is no version to read - and that must not drop them:
			// an executable whose version cannot be read is still an executable
			var found = ConnectionApiServiceLocator.FindInstallations(_root);

			Assert.That(found, Is.Not.Empty);
			Assert.That(found.Select(i => i.Version), Is.All.EqualTo(new Version(0, 0, 0, 0)));
		}

		[Test]
		public void IsServiceInstallation_AsksForTheExecutable()
		{
			Assert.Multiple(() =>
			{
				Assert.That(ConnectionApiServiceLocator.IsServiceInstallation(_root), Is.True);
				Assert.That(ConnectionApiServiceLocator.IsServiceInstallation(
					Path.Combine(_root, "Documentation")), Is.False);
				Assert.That(ConnectionApiServiceLocator.IsServiceInstallation(
					Path.Combine(_root, "not installed here")), Is.False);
				Assert.That(ConnectionApiServiceLocator.IsServiceInstallation(null), Is.False);
				Assert.That(ConnectionApiServiceLocator.IsServiceInstallation(" "), Is.False);
			});
		}

		[Test]
		public async Task GetRunningServiceVersion_WhereNothingListens_IsNullRatherThanAFailure()
		{
			// port 1 needs privileges to bind and nothing serves HTTP there
			string? version = await ConnectionApiServiceLocator.GetRunningServiceVersionAsync(
				"http://localhost:1", TimeSpan.FromMilliseconds(500));

			Assert.That(version, Is.Null);
		}

		[Test]
		public async Task GetRunningServiceVersion_WithoutAUrl_IsNull()
		{
			Assert.That(await ConnectionApiServiceLocator.GetRunningServiceVersionAsync(null), Is.Null);
			Assert.That(await ConnectionApiServiceLocator.GetRunningServiceVersionAsync("  "), Is.Null);
		}
	}
}
