using IdeaStatiCa.ConnectionApi;
using System.Reflection;

namespace UT_ConRestApiClient
{
	/// <summary>
	/// What the runner says when the service it starts never serves anything.
	/// </summary>
	/// <remarks>
	/// Work item 37164: a customer's service would not come up, and every diagnostic the runner
	/// produced was either absent or wrong - the caller was told to install .NET 8 by a hard-coded
	/// message while the service had long since moved to a later runtime, and the service's own
	/// output, the one place the reason was written down, was thrown away. A child process that dies
	/// this way writes no Windows Event Log entry of its own, so there was nothing left to look at.
	///
	/// The tests drive a stand-in executable (<c>FakeConnectionRestApi</c>) that never opens a
	/// listener, so the runner always ends up on its failure path.
	/// </remarks>
	[TestFixture]
	public class ConnectionApiServiceRunnerTests
	{
		private static readonly TimeSpan ShortTimeout = TimeSpan.FromSeconds(3);

		/// <summary>
		/// Directory holding the stand-in service, built alongside this test project.
		/// </summary>
		private static string FakeServiceDirectory
		{
			get
			{
				var testDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
				var directory = Path.GetFullPath(Path.Combine(
					testDirectory, "..", "..", "..", "..", "FakeConnectionRestApi",
					"bin", GetConfiguration(), "net10.0"));

				// Named the same on every platform: the runner looks for a fixed ".exe", and the
				// stand-in project carries the extension in its assembly name where the SDK does not
				// add one. A miss here means the stand-in did not build, not that the runner is wrong.
				var executable = Path.Combine(directory, "IdeaStatiCa.ConnectionRestApi.exe");
				Assert.That(File.Exists(executable), Is.True,
					$"The stand-in service was not built at '{executable}'.");
				return directory;
			}
		}

		private static string GetConfiguration()
		{
#if DEBUG
			return "Debug";
#else
			return "Release";
#endif
		}

		[SetUp]
		public void SetUp()
		{
			Environment.SetEnvironmentVariable("FAKE_SERVICE_MODE", null);
			Environment.SetEnvironmentVariable("FAKE_SERVICE_EXIT_CODE", null);
		}

		[TearDown]
		public void TearDown()
		{
			Environment.SetEnvironmentVariable("FAKE_SERVICE_MODE", null);
			Environment.SetEnvironmentVariable("FAKE_SERVICE_EXIT_CODE", null);
		}

		[Test]
		public void CreateApiClient_ServiceExitsOnStartup_ReportsExitCodeAndServiceOutput()
		{
			Environment.SetEnvironmentVariable("FAKE_SERVICE_MODE", "exit");
			Environment.SetEnvironmentVariable("FAKE_SERVICE_EXIT_CODE", "150");

			using (var runner = new ConnectionApiServiceRunner(FakeServiceDirectory, startupTimeout: ShortTimeout))
			{
				var ex = Assert.ThrowsAsync<InvalidOperationException>(async () => await runner.CreateApiClient());

				Assert.Multiple(() =>
				{
					Assert.That(ex!.Message, Does.Contain("exited with code 150"),
						"The exit code is the single most useful fact about a service that dies on startup.");
					Assert.That(ex.Message, Does.Contain("fake service stdout"),
						"Standard output of the service has to reach the caller.");
					Assert.That(ex.Message, Does.Contain("fake service stderr"),
						"Standard error of the service has to reach the caller.");
				});
			}
		}

		[Test]
		public void CreateApiClient_ServiceExitsOnStartup_DoesNotClaimTheMachineNeedsDotNet8()
		{
			Environment.SetEnvironmentVariable("FAKE_SERVICE_MODE", "exit");

			using (var runner = new ConnectionApiServiceRunner(FakeServiceDirectory, startupTimeout: ShortTimeout))
			{
				var ex = Assert.ThrowsAsync<InvalidOperationException>(async () => await runner.CreateApiClient());

				// The old message named 'Microsoft.AspNetCore.App (version 8.0.0)' unconditionally and
				// linked to a download for it. Following that advice on a service built against a later
				// runtime installs the wrong thing and the service still does not start.
				Assert.That(ex!.Message, Does.Not.Contain("8.0.0"));
			}
		}

		[Test]
		public void CreateApiClient_ServiceNeverServes_ReportsTheTimeoutAndStopsTheProcess()
		{
			Environment.SetEnvironmentVariable("FAKE_SERVICE_MODE", "hang");

			using (var runner = new ConnectionApiServiceRunner(FakeServiceDirectory, startupTimeout: ShortTimeout))
			{
				var ex = Assert.ThrowsAsync<InvalidOperationException>(async () => await runner.CreateApiClient());

				Assert.Multiple(() =>
				{
					Assert.That(ex!.Message, Does.Contain("did not answer its heartbeat"));
					Assert.That(ex.Message, Does.Contain("fake service stdout"),
						"Even a service that hangs usually says something before it does.");
				});
			}
		}

		[Test]
		public void CreateApiClient_AfterAFailedStart_TriesAgainInsteadOfReusingTheDeadService()
		{
			Environment.SetEnvironmentVariable("FAKE_SERVICE_MODE", "exit");

			using (var runner = new ConnectionApiServiceRunner(FakeServiceDirectory, startupTimeout: ShortTimeout))
			{
				Assert.ThrowsAsync<InvalidOperationException>(async () => await runner.CreateApiClient());

				// The runner used to keep the half-started process in its field, so every later call
				// skipped starting anything and handed back a URL nothing was listening on - reported
				// to the caller as a bare "connection actively refused".
				var second = Assert.ThrowsAsync<InvalidOperationException>(async () => await runner.CreateApiClient());
				Assert.That(second!.Message, Does.Contain("exited with code"));
			}
		}

		[Test]
		public void Dispose_AfterAFailedStart_DoesNotThrow()
		{
			Environment.SetEnvironmentVariable("FAKE_SERVICE_MODE", "exit");

			var runner = new ConnectionApiServiceRunner(FakeServiceDirectory, startupTimeout: ShortTimeout);
			Assert.ThrowsAsync<InvalidOperationException>(async () => await runner.CreateApiClient());

			// Disposing is what a 'using' and a window's Closed handler do; killing an already dead
			// process must not turn that into a second failure on top of the first.
			Assert.DoesNotThrow(() => runner.Dispose());
			Assert.DoesNotThrow(() => runner.Dispose());
		}

		[Test]
		public void CreateApiClient_ExecutableMissing_NamesThePathItLookedIn()
		{
			var missing = Path.Combine(Path.GetTempPath(), "no-idea-statica-here-" + Guid.NewGuid().ToString("N"));

			using (var runner = new ConnectionApiServiceRunner(missing, startupTimeout: ShortTimeout))
			{
				var ex = Assert.ThrowsAsync<FileNotFoundException>(async () => await runner.CreateApiClient());
				Assert.That(ex!.Message, Does.Contain(missing));
			}
		}
	}
}
