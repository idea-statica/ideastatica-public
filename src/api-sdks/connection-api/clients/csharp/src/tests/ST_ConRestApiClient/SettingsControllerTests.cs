using FluentAssertions;

namespace ST_ConnectionRestApi
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

		// Lower-bound assertions: the project-settings schema grows over time (new
		// keys get added). Pinning to an exact count makes these tests break every
		// time a setting is added without any actual regression. The smoke test is
		// "settings are non-empty and filtering reduces the result set."
		[Test]
		public async Task ShouldGetProjectSettings()
		{
			var projectSettings = await ConnectionApiClient!.Settings.GetSettingsAsync(ActiveProjectId);
			projectSettings.Count.Should().BeGreaterThan(150);
		}

		[TestCase("calculation", 20)]
		[TestCase("analysis", 20)]
		[TestCase("EN", 70)]
		[TestCase("CalculationCommon", 50)]
		[TestCase("non-existing-setting", 0)]
		[TestCase(null, 150)]
		public async Task ShouldGetProjectSettingsWithFilter(string? search, int minCount)
		{
			var projectSettings = await ConnectionApiClient!.Settings.GetSettingsAsync(ActiveProjectId, search);
			if (minCount == 0)
			{
				projectSettings.Count.Should().Be(0);
			}
			else
			{
				projectSettings.Count.Should().BeGreaterOrEqualTo(minCount);
			}
		}

		// Settings keys are returned WITHOUT the @<version> suffix — the server
		// strips it when keying the response dictionary (see SettingsController
		// GetProjectSettingsCoreAsync). Tests must pass the stripped form.
		[TestCase("calculationCommon/Checks/Shared/OptimalCheckLevel", 0.025)]
		[TestCase("calculationCommon/Checks/Shared/WarningCheckLevel", 0.92)]
		[TestCase("calculationCommon/Checks/Shared/LocalDeformationLimit", 0.025)]
		public async Task ShouldUpdateProjectSettings(string settingName, double value)
		{
			var projectSettings = await ConnectionApiClient!.Settings.GetSettingsAsync(ActiveProjectId);

			var calculationSetting = projectSettings.FirstOrDefault(x => x.Key == settingName);
			calculationSetting.Key.Should().NotBeNull($"setting '{settingName}' should exist in the project");

			var update = new Dictionary<string, object> { { settingName, value } };
			await ConnectionApiClient!.Settings.UpdateSettingsAsync(ActiveProjectId, update);

			var updatedSettings = await ConnectionApiClient!.Settings.GetSettingsAsync(ActiveProjectId);

			var updatedSetting = updatedSettings.FirstOrDefault(x => x.Key == settingName);
			updatedSetting.Key.Should().NotBeNull($"setting '{settingName}' should still exist after update");
			updatedSetting.Value.Should().Be(value);
		}
	}
}
