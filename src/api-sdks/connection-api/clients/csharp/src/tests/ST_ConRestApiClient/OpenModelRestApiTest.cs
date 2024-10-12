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

			ConnectionApiClient = await ApiFactory.CreateConnectionApiClient();
		}

		[TearDown]
		public async Task TearDown()
		{
			if (ConnectionApiClient != null)
			{
				await ConnectionApiClient!.DisposeAsync();
			}
		}


		//[Test]
		//public async Task ShouldImportIOMTest()
		//{
		//	//var iomImportOption = new ConIomImportOptions();

		//	//var connectionIds = new List<int>();
		//	//iomImportOption.ConnectionsToCreate = connectionIds;

		//	string connProjectContainerFilePath = Path.Combine(ProjectPath, "OpenModelContainer.xml");

		//	var conProject = await ConnectionApiClient.OpenFromIomFileAsync(connProjectContainerFilePath);


		//}

		//[Test]
		//public async Task ShouldUpdateConnectionbyIOMModel()
		//{
		//	var iomImportOption = new ConIomImportOptions();

		//	var connectionIds = new List<int>();
		//	iomImportOption.ConnectionsToCreate = connectionIds;

		//	string connProjectContainerFilePath = Path.Combine(ProjectPath, "OneConnectionImport.xml");

		//	var conProject = await ConnApiController!.CreateProjectFromIomContainerFileAsync(connProjectContainerFilePath, iomImportOption, CancellationToken.None);
		//	conProject.Should().NotBeNull();


		//	//update
		//	string connProjectContainerFilePathUpdate = Path.Combine(ProjectPath, "OneConnectionUpdate.xml");

		//	var conProjectUpdated = await ConnApiController!.UpdateProjectFromIomContainerFileAsync(connProjectContainerFilePathUpdate, CancellationToken.None);
		//	conProjectUpdated.Should().BeTrue();

		//}

		//[Test]
		//public async Task ShouldImportIOMTestAndUpdateCodeSetup()
		//{
		//	var iomImportOption = new ConIomImportOptions();

		//	var connectionIds = new List<int>();
		//	iomImportOption.ConnectionsToCreate = connectionIds;

		//	string connProjectContainerFilePath = Path.Combine(ProjectPath, "IomContainerFromPython.xml");

		//	var conProject = await ConnApiController!.CreateProjectFromIomContainerFileAsync(connProjectContainerFilePath, iomImportOption, CancellationToken.None);
		//	conProject.Should().NotBeNull();

		//	var connectionSetup = await ConnApiController!.GetConnectionSetupAsync(CancellationToken.None);

		//	connectionSetup.HssLimitPlasticStrain.Should().Be(0.01);

		//	connectionSetup.HssLimitPlasticStrain = 0.02;

		//	var updateResponse = await ConnApiController!.UpdateConnectionSetupAsync(connectionSetup, CancellationToken.None);

		//	var updatedConnectionSetup = await ConnApiController!.GetConnectionSetupAsync(CancellationToken.None);

		//	updatedConnectionSetup.HssLimitPlasticStrain.Should().Be(0.02);
		//}
	}
}