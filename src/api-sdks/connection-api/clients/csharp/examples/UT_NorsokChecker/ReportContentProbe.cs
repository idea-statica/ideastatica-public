using System.IO;
using NorsokChecker.Models;
using NorsokChecker.Services;
using NorsokChecker.Services.Chapters;
using NorsokChecker.Services.Norsok64;
using IdeaStatiCa.ConnectionApi;

namespace UT_NorsokChecker
{
	/// <summary>
	/// Generates the report from REAL test_cs results and reports what is in it — the derivation
	/// blocks, the group headings, and anything that landed in "Other Checks".
	///
	/// Written to answer one question with a measurement rather than by reading the renderer: the
	/// per-connection detail is reported missing, and the code path that would drop it
	/// (FormulaLatex has no §6.4 keys, so the formula block is skipped) explains only part of it.
	///
	/// Probe: needs a live 26.0 service and the .ideaCon.
	/// </summary>
	[TestFixture, Category("Probe"), Explicit("Needs a live service and the real .ideaCon")]
	public class ReportContentProbe
	{
		private const string IdeaCon =
			@"C:\Users\OndrejSkorunka\Claude\01_Folders\NORSOK\ideacon\test_cs.ideaCon";

		private static string SetupDir =>
			Environment.GetEnvironmentVariable("IDEASTATICA_SETUP_DIR")
			?? @"C:\Program Files\IDEA StatiCa\StatiCa 26.0";

		private ConnectionApiServiceRunner? _runner;

		[OneTimeSetUp]
		public void Setup()
		{
			if (!File.Exists(IdeaCon)) Assert.Ignore($"no test project at {IdeaCon}");
			if (!Directory.Exists(SetupDir)) Assert.Ignore($"no IDEA StatiCa install at {SetupDir}");
			_runner = new ConnectionApiServiceRunner(SetupDir);
		}

		[OneTimeTearDown]
		public void Teardown() => _runner?.Dispose();

		[Test]
		public async Task WhatIsActuallyInTheReport()
		{
			void Log(string _) { }

			var client = await _runner!.CreateApiClient();
			var project = await client.Project.OpenProjectAsync(IdeaCon);
			await new ProjectSettingsService(client, Log).ApplyNorsokFactorsAsync(project.ProjectId);

			var crossSections = await client.Material.GetCrossSectionsAsync(project.ProjectId);
			var sectionMap = JointSectionMap.FromCrossSections(crossSections.Cast<object>());

			// CON1 assesses cleanly, CON5 is rejected — one of each
			var wanted = new[] { "CON1", "CON5" };
			var results = new List<(string, List<NorsokFormulaResult>)>();

			foreach (var con in (project.Connections ?? new()).Where(c => wanted.Contains(c.Name)))
			{
				var rows = new List<NorsokFormulaResult>();
				var les = await client.LoadEffect.GetLoadEffectsAsync(project.ProjectId, con.Id, isPercentage: false);
				var ctx = new ChapterContext
				{
					Client = client, ProjectId = project.ProjectId, ConnectionId = con.Id,
					ConnectionName = con.Name!, LoadEffects = les, SectionMap = sectionMap, Log = Log,
				};
				foreach (var ch in ChapterRegistry.All)
					rows.AddRange((await ch.EvaluateAsync(ctx, CancellationToken.None)).Rows);

				results.Add((con.Name!, rows));

				TestContext.Out.WriteLine($"{con.Name}: {rows.Count} row(s)");
				foreach (var r in rows.Take(4))
					TestContext.Out.WriteLine(
						$"    Section='{r.Section}' JointDetail={(r.JointDetail == null ? "NULL" : "set")}"
						+ $"  Vars={r.Variables?.Count ?? 0}  Title='{r.Title}'");
			}

			string html = NorsokHtmlReportGenerator.GenerateReport("test_cs.ideaCon", results, expandAll: true);

			string outPath = Path.Combine(
				Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)!,
				"report-probe.html");
			File.WriteAllText(outPath, html);

			TestContext.Out.WriteLine();
			TestContext.Out.WriteLine("=== WHAT THE REPORT CONTAINS ===");
			foreach (var (what, needle) in new[]
			{
				("check cards", "<details class='check-card"),
				("group headings", "class='chapter-header'"),
				("\"Other Checks\" bucket", "Other Checks"),
				("derivation blocks", "<div class='deriv-block'>"),
				("derivation steps", "<div class='deriv-step'>"),
				("formula blocks (FormulaLatex)", "class='formula-block'"),
				("connection summary table", "class='connection-table'"),
				("  its rows", "class='con-verdict"),
				("joint geometry tables", "class='geom-table'"),
				("  its brace rows", "<td>M"),
				("KaTeX", "katex"),
			})
			{
				int n = System.Text.RegularExpressions.Regex.Matches(
					html, System.Text.RegularExpressions.Regex.Escape(needle)).Count;
				TestContext.Out.WriteLine($"  {what,-32} {n}");
			}

			TestContext.Out.WriteLine($"\n  html {html.Length} chars → {outPath}");

			await client.Project.CloseProjectAsync(project.ProjectId);
			Assert.Pass("measurement only");
		}
	}
}
