using System.IO;
using IdeaStatiCa.Api.Connection.Model;
using Newtonsoft.Json.Linq;
using NorsokChecker.Models;
using NorsokChecker.Services;
using NorsokChecker.Services.Formulas;
using NorsokChecker.Services.Norsok64;

namespace UT_NorsokChecker
{
	/// <summary>
	/// Smoke tests for the §6.4 auto-topology report path: JointCheckRow → BuildResultFromRow card →
	/// NorsokHtmlReportGenerator derivation blocks (per-class table, K-per-gap, chord-stress trail,
	/// validity). Uses the KT fixture — the richest case (two K gaps + mixed K/Y classification).
	/// </summary>
	[TestFixture]
	public class JointReportTests
	{
		private static JointTopology BuildKtTopology()
		{
			string dir = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData");
			var fixtures = JObject.Parse(File.ReadAllText(Path.Combine(dir, "topology_fixtures.json")));

			var sections = new Dictionary<int, JointSectionInfo>();
			foreach (var cs in (JArray)fixtures["crossSections"]!)
			{
				var (d, t) = JointSectionInfo.ParseChs((string?)cs["name"]);
				sections[(int)cs["id"]!] = new JointSectionInfo
				{
					Name = (string?)cs["name"], D = d, T = t, IsCHS = d != null,
					Fy = (double?)cs["material"]?["element"]?["fy"],
				};
			}
			var fx = ((JArray)fixtures["fixtures"]!).First(f => (string?)f["name"] == "KT_TEST");
			var members = fx["members"]!
				.Select(j => j.ToObject<ConMember>()!)
				.Select(m => JointMemberData.FromConMember(m,
					sections.GetValueOrDefault(m.CrossSectionId ?? -1) ?? new JointSectionInfo()))
				.ToList();
			var les = fx["loadEffects"]!.Select(j => j.ToObject<ConLoadEffect>()!).ToList();
			return new JointTopologyBuilder().Build(members, les);
		}

		[Test]
		public void BuildResultFromRow_CarriesDetailAndClassification()
		{
			var topo = BuildKtTopology();
			var row = topo.JointChecks[0].Rows.First(r => r.Name == "KA");   // diagonal with 2 K gaps + Y remainder
			var card = TubularJointCheck.BuildResultFromRow(row, "LE1");

			Assert.Multiple(() =>
			{
				Assert.That(card.JointDetail, Is.SameAs(row), "detail attached for the report");
				Assert.That(card.Section, Is.EqualTo("6.4.3.6"));
				Assert.That(card.Utilization, Is.EqualTo(row.Util).Within(1e-12));
				Assert.That(card.Passed, Is.EqualTo(row.Passed));
				Assert.That(card.Title, Does.Contain("KA"));
				// classification fractions surfaced as variables
				Assert.That(card.Variables.Any(v => v.Symbol == "frK"), "frK variable");
				// two balancing gaps → two K-gap breakdown variables
				Assert.That(card.Variables.Count(v => v.Symbol.StartsWith("K gap")), Is.EqualTo(2), "K per-gap rows");
			});
		}

		[Test]
		public void HtmlReport_RendersDerivationBlocks()
		{
			var topo = BuildKtTopology();
			var results = new List<NorsokFormulaResult>();
			foreach (var r in topo.JointChecks[0].Rows.Where(r => !r.Skipped))
				results.Add(TubularJointCheck.BuildResultFromRow(r, "LE1"));

			string html = NorsokHtmlReportGenerator.GenerateReport(
				"UT", new[] { ("KT_TEST", results) }, expandAll: true);

			Assert.Multiple(() =>
			{
				Assert.That(html, Does.Contain("deriv-block"), "derivation block rendered");
				Assert.That(html, Does.Contain("K/Y/X classification"), "classification block");
				Assert.That(html, Does.Contain("Per-class axial resistance"), "per-class table");
				Assert.That(html, Does.Contain("K resistance per balancing gap"), "K per-gap table (KT case)");
				Assert.That(html, Does.Contain("Chord stresses at this brace footprint"), "chord-stress trail");
				Assert.That(html, Does.Contain("Validity ranges"), "validity table");
				Assert.That(html, Does.Contain("active-class"), "active classes highlighted");
				// all three braces got a card
				Assert.That(html, Does.Contain("KA"));
				Assert.That(html, Does.Contain("KV"));
				Assert.That(html, Does.Contain("KB"));
			});
		}
	}
}
