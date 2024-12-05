using FluentAssertions;
using ST_ConnectionRestApi;

namespace ST_RcsRestApiClient
{
	public class RcsRestApiIomTest : RcsRestApiBaseTest
	{

		[SetUp]
		public async Task SetUp()
		{
			RcsApiClient = await ApiFactory.CreateApiClient();
			if (RcsApiClient == null)
			{
				throw new Exception("RcsApiClient is not created");
			}

			
			//string iomProjectFilePath = Path.Combine(ProjectPath, "ImportOpenModel.xml");
			//this.Project = await RcsApiClient!.Project.CreateProjectFromIomFileAsync(iomProjectFilePath, CancellationToken.None);

			// Replace for IOM
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
	}
}
