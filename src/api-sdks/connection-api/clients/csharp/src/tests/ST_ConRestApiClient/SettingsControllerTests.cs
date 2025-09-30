using FluentAssertions;
using ST_ConnectionRestApi;

namespace ST_ConRestApiClient
{
	public class SettingsControllerTests : ConRestApiBaseTest
	{
		[SetUp]
		public async Task SetUp()
		{

			ConnectionApiClient = await ApiFactory.CreateApiClient();

			string connProjectFilePath = Path.Combine(ProjectPath, "Simple-1-ECEN.ideaCon");
			this.Project = await ConnectionApiClient.Project.OpenProjectAsync(connProjectFilePath);
			this.ActiveProjectId = Project.ProjectId;
			if (this.ActiveProjectId == Guid.Empty)
			{
				throw new Exception("Project is not opened");
			}

			this.Project = await ConnectionApiClient.Project.GetProjectDataAsync(ActiveProjectId);

			Project.Should().NotBeNull();
			Project.ProjectId.Should().NotBe(Guid.Empty);
		}

		[TearDown]
		public async Task TearDown()
		{
			if (ConnectionApiClient != null)
			{
				await ConnectionApiClient!.DisposeAsync();
			}
		}

		[Test]
		public async Task ShouldGetProjectSettings()
		{
			var projectSettings = await ConnectionApiClient!.Settings.GetSettingsAsync(ActiveProjectId);
			projectSettings.Count.Should().Be(167);
		}

		[TestCase("calculation", 22)]
		[TestCase("analysis", 25)]
		[TestCase("EN", 76)]
		[TestCase("CalculationCommon", 60)]
		[TestCase("non-existing-setting", 0)]
		[TestCase(null, 167)]
		public async Task ShouldGetProjectSettingsWithFilter(string? search, int count)
		{
			var con1 = Project!.Connections.First();
			var projectSettings = await ConnectionApiClient!.Settings.GetSettingsAsync(ActiveProjectId, search);
			projectSettings.Count.Should().Be(count);
		}

		[TestCase("calculationCommon/Checks/Shared/OptimalCheckLevel@01", 0.025)]
		[TestCase("calculationCommon/Checks/Shared/WarningCheckLevel@01", 0.92)]
		[TestCase("calculationCommon/Checks/Shared/LocalDeformationLimit@01", 0.025)]
		public async Task ShouldUpdateProjectSettings(string settingName, double value)
		{
			var con1 = Project!.Connections.First();
			var projectSettings = await ConnectionApiClient!.Settings.GetSettingsAsync(ActiveProjectId);

			var calculationSetting = projectSettings.FirstOrDefault(x => x.Key == settingName);

			calculationSetting.Should().NotBeNull();

			var update = new Dictionary<string, object>
				{ { settingName, value } };

			await ConnectionApiClient!.Settings.UpdateSettingsAsync(ActiveProjectId, update);

			var updatedSettings = await ConnectionApiClient!.Settings.GetSettingsAsync(ActiveProjectId);

			var updatedSetting = updatedSettings.FirstOrDefault(x => x.Key == settingName);
			updatedSetting.Should().NotBeNull();
			updatedSetting.Value.Should().Be(value);
		}
	}
}
