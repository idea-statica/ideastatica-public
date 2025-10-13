using FluentAssertions;
using IdeaStatiCa.Api.RCS.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Xml.Linq;

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
		public async Task ShouldGetAndUpdateCodeSettingsDouble()
		{
			var sections = await RcsApiClient!.Section.SectionsAsync(this.ActiveProjectId);
			sections.Should().NotBeNull();

			var currentSettingsJson = await RcsApiClient!.Project.GetCodeSettingsAsync(ActiveProjectId);
			currentSettingsJson.Should().NotBeNull();
			currentSettingsJson.Should().NotBeEmpty();

			JArray currentSettings = JArray.Parse(currentSettingsJson);
			currentSettings.Should().NotBeNull();
			currentSettings.Should().HaveCountGreaterThan(0);

			var sampleSetting = currentSettings.FirstOrDefault();
			sampleSetting.Should().NotBeNull();
			sampleSetting!["id"]?.Value<int>().Should().BeGreaterThan(-1);
			sampleSetting["type"]?.ToString().Should().NotBeNullOrEmpty();
			sampleSetting["value"].Should().NotBeNull();

			var doubleSetting = currentSettings
				.FirstOrDefault(s => s["type"]?.ToString() == "System.Double");

			if (doubleSetting != null)
			{
				var originalValue = doubleSetting["value"];
				var settingId = doubleSetting["id"]!.Value<int>();

				var newSettings = new List<RcsSetting >
				{
					new RcsSetting
					{
						Id = settingId,
						Type = "System.Double",
						Value = 2.5
					}
				};

				var updateResult = await RcsApiClient!.Project.UpdateCodeSettingsAsync(this.ActiveProjectId, newSettings!);
				updateResult.Should().BeTrue();

				var updatedSettingsJson = await RcsApiClient!.Project.GetCodeSettingsAsync(ActiveProjectId);
				var updatedSettings = JArray.Parse(updatedSettingsJson);

				var updatedSetting = updatedSettings
					.FirstOrDefault(s => s["id"]?.Value<int>() == settingId);

				updatedSetting.Should().NotBeNull();
				updatedSetting!["value"]!.Value<double>().Should().Be(2.5);
			}
		}

		[Test]
		public async Task ShouldGetAndUpdateCodeSettignsSetupTable()
		{
			var sections = await RcsApiClient!.Section.SectionsAsync(this.ActiveProjectId);
			sections.Should().NotBeNull();

			var currentSettingsJson = await RcsApiClient!.Project.GetCodeSettingsAsync(ActiveProjectId);
			currentSettingsJson.Should().NotBeNull();
			currentSettingsJson.Should().NotBeEmpty();

			JArray currentSettings = JArray.Parse(currentSettingsJson);
			var setupTableSetting = currentSettings
				.FirstOrDefault(s => s["type"]?.ToString()?.Contains("SetupTable_W_max_1992_1_1") == true);

			if (setupTableSetting != null)
			{
				var originalValue = setupTableSetting["value"];
				var settingId = setupTableSetting["id"]!.Value<int>();
				var settingType = setupTableSetting["type"]!.ToString();

				var newSettings = new List<RcsSetting>
				{
					new RcsSetting
					{
						Id = settingId,
						Type = settingType,
						Value = new Dictionary<string, object>
						{
							["X0_XC1_PCB"] = 1.55,
							["X0_XC1_PCU"] = 1.55,

							["XC2_XC3_PCB_CV"] = 1.55,
							["XC2_XC3_PCB_DV"] = 1.55,
							["XC2_XC3_PCU"] = 1.55,

							["XD_XS_XF_PCB_CV"] = 1.55,
							["XD_XS_XF_PCB_DV"] = 1.55,
							["XD_XS_XF_PCU"] = 1.5
						}
					}
				};

				var updateResult = await RcsApiClient!.Project.UpdateCodeSettingsAsync(this.ActiveProjectId, newSettings);
				updateResult.Should().BeTrue();

				var updatedSettingsJson = await RcsApiClient!.Project.GetCodeSettingsAsync(ActiveProjectId);
				var updatedSettings = JArray.Parse(updatedSettingsJson);

				var updatedSetting = updatedSettings
					.FirstOrDefault(s => s["id"]?.Value<int>() == settingId);

				updatedSetting.Should().NotBeNull();
				updatedSetting!["value"].Should().NotBeNull();

				updatedSetting.SelectToken("value.X0_XC1_PCB")?.Value<double>().Should().Be(1.55);
				updatedSetting.SelectToken("value.X0_XC1_PCU")?.Value<double>().Should().Be(1.55);
			}
		}

		[Test]
		public async Task ShouldGetAndUpdateCodeSettignsSetup2Values()
		{
			var sections = await RcsApiClient!.Section.SectionsAsync(this.ActiveProjectId);
			sections.Should().NotBeNull();

			var currentSettingsJson = await RcsApiClient!.Project.GetCodeSettingsAsync(ActiveProjectId);
			currentSettingsJson.Should().NotBeNull();
			currentSettingsJson.Should().NotBeEmpty();

			JArray currentSettings = JArray.Parse(currentSettingsJson);
			var setup2ValuesSetting = currentSettings
				.FirstOrDefault(s => s["type"]?.ToString()?.Contains("Setup2Values") == true);

			if (setup2ValuesSetting != null)
			{
				var settingId = setup2ValuesSetting["id"]!.Value<int>();
				var settingType = setup2ValuesSetting["type"]!.ToString();

				var updateSettings = new List<RcsSetting>
				{
					new RcsSetting
					{
						Id = settingId,
						Type = settingType,
						Value = new Dictionary<string, object>
						{
							["Value1"] = 1.2,
							["Value2"] = 2.2
						}
					}
				};

				var updateResult = await RcsApiClient!.Project.UpdateCodeSettingsAsync(this.ActiveProjectId, updateSettings);
				updateResult.Should().BeTrue();

				var updatedSettingsJson = await RcsApiClient!.Project.GetCodeSettingsAsync(ActiveProjectId);
				var updatedSettings = JArray.Parse(updatedSettingsJson);

				var updatedSetting = updatedSettings
					.FirstOrDefault(s => s["id"]?.Value<int>() == settingId);

				updatedSetting.Should().NotBeNull();

				updatedSetting!.SelectToken("value.Value1")?.Value<double>().Should().Be(1.2);
				updatedSetting.SelectToken("value.Value2")?.Value<double>().Should().Be(2.2);
			}
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
				TestContext.Out.WriteLine($"Writing swagger.json to {swaggerJsonPath}");
				File.WriteAllText(swaggerJsonPath, swaggerJson);
			}
			else
			{
				TestContext.Out.WriteLine($"OpenAPI_WORK_DIRECTORY ('{openApi_Dir}') is not set or does not exist");
			}
		}
	}
}