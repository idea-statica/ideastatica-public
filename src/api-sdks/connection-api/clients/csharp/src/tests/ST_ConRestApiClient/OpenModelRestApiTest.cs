using FluentAssertions;

namespace ST_ConnectionRestApi
{
	public class OpenModelRestApiTest : ConRestApiBaseTest
	{
		[SetUp]
		public async Task SetUp()
		{
			//if (this.RunServer)
			//{
			//	ConnApiController = await ApiFactory.CreateConnectionApiClient();
			//}
			//else
			//{
			//	if (ApiUri == null)
			//	{
			//		throw new Exception("ApiUri is not set");
			//	}
			//}

			ConnectionApiClient = await ApiFactory.CreateApiClient();
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
		public async Task ShouldImportIOMTest()
		{
			string connProjectContainerFilePath = Path.Combine(ProjectPath, "OpenModelContainer.xml");
			var conProject = await ConnectionApiClient!.Project.CreateProjectFromIomFileAsync(connProjectContainerFilePath);

			conProject.Should().NotBeNull();
		}

		[Test]
		public async Task ShouldImportSelectionIOMTest()
		{
			string connProjectContainerFilePath = Path.Combine(ProjectPath, "multiple_connections.xml");

			var selection = new List<int>() { 19, 33};

			var conProject = await ConnectionApiClient!.Project.CreateProjectFromIomFileAsync(connProjectContainerFilePath, selection);

			conProject.Should().NotBeNull();

			conProject.Connections.Count.Should().Be(2);
		}
		

		[Test]
		public async Task ShouldUpdateConnectionbyIOMModel()
		{
			string connProjectContainerFilePath = Path.Combine(ProjectPath, "OneConnectionImport.xml");

			var conProject = await ConnectionApiClient!.Project.CreateProjectFromIomFileAsync(connProjectContainerFilePath);
			conProject.Should().NotBeNull();

			var projectId = conProject.ProjectId;


			//update
			string connProjectContainerFilePathUpdate = Path.Combine(ProjectPath, "OneConnectionUpdate.xml");

			var conProjectUpdated = await ConnectionApiClient!.Project.UpdateProjectFromIomFileAsync(projectId, connProjectContainerFilePathUpdate);
			conProjectUpdated.Should().NotBeNull();

		}
	}
}