using System.IO;
using System.Net.Http;
using IdeaStatiCa.ConnectionApi;

namespace UT_NorsokChecker
{
	/// <summary>
	/// Does the IOM export work on service 26.1, and if the C# client returns null for it, is that
	/// the service's doing or the client's?
	///
	/// Measured over raw HTTP against 26.1.0.2007 (2026-08-27): the endpoint
	/// `/api/4/projects/{p}/connections/{c}/export-iom-connection-data` answers **200 with 418 kB
	/// and all six beams**, M1 as RolledCHS with 32 facets — exactly the data D/T is read from. So
	/// an earlier note in this repo saying "against service 26.1 this call returns a NULL
	/// ConnectionData" was wrong about the CAUSE: the service returns the model.
	///
	/// This test pins the distinction, because the two have opposite fixes: a broken service means
	/// waiting for a service build, a broken client means either an upgrade or reading the payload
	/// ourselves.
	///
	/// Explicit: needs a service running on the default port and test_cs.ideaCon.
	/// </summary>
	[TestFixture, Explicit("Needs a Connection REST API on port 5000 and test_cs.ideaCon")]
	[Category("Live")]
	public class IomExportVersionTests
	{
		private const string IdeaCon =
			@"C:\Users\OndrejSkorunka\Claude\01_Folders\NORSOK\ideacon\test_cs.ideaCon";
		private const string Base = "http://localhost:5000";

		[Test]
		public async Task TheServiceReturnsTheModelEvenWhenTheClientDoesNot()
		{
			Assert.That(File.Exists(IdeaCon), $"{IdeaCon} must exist");

			string? version = await NorsokChecker.Services.ServiceLocator.RunningVersionAsync(Base);
			Assert.That(version, Is.Not.Null, "no service is answering on port 5000");
			TestContext.Out.WriteLine($"service: {version}");

			// ── the same call the app makes, through the generated client ──
			var client = await new ConnectionApiServiceAttacher(Base).CreateApiClient();
			var project = await client.Project.OpenProjectAsync(IdeaCon);
			var conns = await client.Connection.GetConnectionsAsync(project.ProjectId);
			var con1 = conns.First(c => c.Name == "CON1");

			var viaClient = await client.Export.ExportIomConnectionDataAsync(project.ProjectId, con1.Id);
			int beamsViaClient = viaClient?.Beams?.Count ?? -1;
			TestContext.Out.WriteLine($"via the C# client : "
				+ (viaClient == null ? "NULL ConnectionData" : $"{beamsViaClient} beam(s)"));

			// ── and the same endpoint over raw HTTP, on the same open project ──
			// The point of doing both in ONE test on ONE project: it removes every difference
			// except the client, so whichever answer differs is the client's.
			using var http = new HttpClient { Timeout = TimeSpan.FromMinutes(2) };
			http.DefaultRequestHeaders.Add("ClientId", ClientIdOf(client));
			http.DefaultRequestHeaders.Add("Accept", "application/json");
			var raw = await http.GetAsync(
				$"{Base}/api/4/projects/{project.ProjectId}/connections/{con1.Id}"
				+ "/export-iom-connection-data");
			string body = await raw.Content.ReadAsStringAsync();
			TestContext.Out.WriteLine($"via raw HTTP      : {(int)raw.StatusCode} "
				+ $"{raw.Content.Headers.ContentType}, {body.Length} chars");

			await client.Project.CloseProjectAsync(project.ProjectId);

			Assert.Multiple(() =>
			{
				Assert.That((int)raw.StatusCode, Is.EqualTo(200),
					"the SERVICE must return the model; a non-200 here would mean it is the service");
				Assert.That(body, Does.Contain("\"beams\""),
					"and the payload must carry the beams array");
				// This is the assertion that documents the defect. It FAILS on 26.1 today, and that
				// failure is the finding — not a broken test.
				Assert.That(beamsViaClient, Is.GreaterThan(0),
					$"the client deserialised {beamsViaClient} beams out of a {body.Length}-char "
					+ "payload the service returned successfully — the loss is in the client");
			});
		}

		/// <summary>
		/// The ClientId the attacher negotiated, so the raw call runs in the SAME session as the
		/// client's — a different session would not see the open project, and the comparison would
		/// be between two different things.
		/// </summary>
		private static string ClientIdOf(IConnectionApiClient client)
		{
			// the generated client exposes it as a property on the configuration it was built with
			var prop = client.GetType().GetProperty("ClientId")
				?? client.GetType().GetProperty("ClientIdValue");
			string? id = prop?.GetValue(client)?.ToString();
			Assert.That(id, Is.Not.Null.And.Not.Empty,
				"could not read the ClientId off the client — the raw call needs the same session");
			return id!;
		}
	}
}
