using FluentAssertions;
using ST_RcsRestApiClient;

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

			string iomProjectFilePath = Path.Combine(ProjectPath, "ImportOpenModel.xml");
			this.Project = await RcsApiClient!.Project.CreateProjectFromIomFileAsync(iomProjectFilePath, CancellationToken.None);

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
			try
			{
				var projectData = await RcsApiClient!.Project.GetActiveProjectAsync();
				projectData.Should().NotBeNull();
				projectData.ProjectData.Code.Should().Be("ECEN");

				var xmlFilePath = Path.Combine(ProjectPath, "ExportOpenModel.xml");
				await RcsApiClient!.Project.SaveProjectAsync(this.ActiveProjectId, xmlFilePath, CancellationToken.None);

				bool isSaved = File.Exists(xmlFilePath);
				isSaved.Should().BeTrue("Project is not saved");

				FileInfo fi = new FileInfo(xmlFilePath);
				fi.Length.Should().BeGreaterThan(0);
			}
			finally
			{
				if (File.Exists(Path.Combine(ProjectPath, "ExportOpenModel.xml")))
				{
					File.Delete(Path.Combine(ProjectPath, "ExportOpenModel.xml"));
				}
			}
		}
	}
}
