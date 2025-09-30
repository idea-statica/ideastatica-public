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
	}
}
