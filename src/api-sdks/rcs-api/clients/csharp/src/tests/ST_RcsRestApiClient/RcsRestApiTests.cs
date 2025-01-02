using FluentAssertions;
using IdeaStatiCa.Api.RCS.Model;
using IdeaStatiCa.Api.RCS;
using System.Net;
using System.Xml.Linq;
using static System.Collections.Specialized.BitVector32;
using IdeaRS.OpenModel.Concrete;
using System.Web;

namespace ST_RcsRestApiClient
{
	public class RcsRestApiTests : RcsRestApiBaseTest
	{
		[SetUp]
		public async Task SetUp()
		{
			RcsApiClient = await ApiFactory.CreateApiClient();
			if(RcsApiClient == null)
			{
				throw new Exception("RcsApiClient is not created");
			}

			string rcsProjectFilePath = Path.Combine(ProjectPath, "Project1.IdeaRcs");
			this.Project = await RcsApiClient!.Project.OpenProjectAsync(rcsProjectFilePath);
			this.ActiveProjectId = Project.ProjectId;
			if (this.ActiveProjectId == Guid.Empty)
			{
				throw new Exception("Project is not opened");
			}

			Project.Should().NotBeNull();
			Project.ProjectId.Should().NotBe(Guid.Empty);
		}

		[TearDown]
		public async Task TearDown()
		{
			if (RcsApiClient != null)
			{
				await RcsApiClient!.DisposeAsync();
			}
		}

		[Test]
		public async Task ShouldGetRcsProjectData()
		{
			var projectData = await RcsApiClient!.Project.GetActiveProjectAsync();
			projectData.Should().NotBeNull();

			projectData.ProjectData.Code.Should().Be("ECEN");
		}

		[Test]
		public async Task ShouldCalculateProject()
		{
			var briefResults = await RcsApiClient!.Calculation.CalculateAsync(this.ActiveProjectId, new RcsCalculationParameters());

			briefResults.Should().NotBeNull();
			briefResults.Count.Should().Be(2);
		}

		[Test]
		public async Task ShouldGetReinforcedCrossSection()
		{
			var reinfCssInProj = await RcsApiClient!.CrossSection.ReinforcedCrossSectionsAsync(this.ActiveProjectId);
			reinfCssInProj.Should().NotBeNull();

			reinfCssInProj.Count.Should().Be(1);

			var reinfCss1 = reinfCssInProj.First();
			reinfCss1.Id.Should().Be(1);
			reinfCss1.Name.Should().Be("R 1");
		}

		[Test]
		public async Task ShouldGetSections()
		{
			var sections = await RcsApiClient!.Section.SectionsAsync(this.ActiveProjectId);

			sections.Should().NotBeNull();

			sections.Count.Should().Be(2);

			var sect2 = sections[1];
			sect2.Id.ToString().Should().Be("2");
			sect2.Description.Should().Be("S 2");
			sect2.CheckMemberId.Should().Be(1);
			sect2.RCSId.Should().Be(1);
		}

		[Test]
		public async Task ShouldGetCodeSettins()
		{
			var sections = await RcsApiClient!.Section.SectionsAsync(this.ActiveProjectId);

			sections.Should().NotBeNull();

			var settingsString = await RcsApiClient!.Project.GetCodeSettingsAsync(ActiveProjectId);
			settingsString.Should().NotBeNull();
			settingsString.Should().NotBeEmpty();

			XDocument xmlDoc;
			using (var reader = new StringReader(settingsString))
			{
				xmlDoc = XDocument.Load(reader);
			}

			// setup value for gamma_c 
			var setupValues = xmlDoc.Descendants("SetupValue");
			XElement gamma_c_element = setupValues.Where(x => x.Descendants("Id").FirstOrDefault()?.Value == "2").FirstOrDefault();

			gamma_c_element.Should().NotBeNull();

			// update value1
			var gamma_c_val1 = gamma_c_element.Descendants("Value1").First();
			gamma_c_val1.Value.Should().Be("1.5");
			gamma_c_val1.Value = "1.4";

			var newSettings = new List<RcsSetting>();
			var updateResult = await RcsApiClient!.Project.UpdateCodeSettingsAsync(this.ActiveProjectId, newSettings);

			updateResult.Should().BeTrue();
		}

		[Test]
		public async Task ShouldGetSwaggerJson()
		{
			// do the test only if ApiUri is set
			if (this.ApiUri == null)
			{
				return;
			}

			// Add relative part to ApiUri
			UriBuilder uriBuilderApiVer = new UriBuilder(this.ApiUri);

			var uriBuilder = new UriBuilder(uriBuilderApiVer.Scheme, uriBuilderApiVer.Host, uriBuilderApiVer.Port);
			uriBuilder.Path += "swagger/1.0/swagger.json";
			var swaggerJsonUri = uriBuilder.Uri;

			// Perform HTTP GET request for ApiUri
			HttpClient httpClient = new HttpClient();
			HttpResponseMessage response = await httpClient.GetAsync(swaggerJsonUri);
			response.StatusCode.Should().Be(HttpStatusCode.OK);

			string swaggerJson = await response.Content.ReadAsStringAsync();
			swaggerJson.Should().NotBeNullOrEmpty();

			var openApi_Dir = Environment.GetEnvironmentVariable("OpenAPI_WORK_DIRECTORY");
			if (!string.IsNullOrEmpty(openApi_Dir) && Directory.Exists(openApi_Dir))
			{
				var swaggerJsonPath = Path.Combine(openApi_Dir, "rcs-OpenAPI.json");
				TestContext.WriteLine($"Writing swagger.json to {swaggerJsonPath}");
				File.WriteAllText(swaggerJsonPath, swaggerJson);
			}
			else
			{
				TestContext.WriteLine($"OpenAPI_WORK_DIRECTORY ('{openApi_Dir}') is not set or does not exist");
			}
		}
	}
}