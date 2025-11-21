using FluentAssertions;
using IdeaRS.OpenModel.Material;
using IdeaStatiCa.Api.Connection.Model.Material;
using ST_ConnectionRestApi;

namespace ST_ConnectionRestApi
{
	internal class OperationsControllerTests : ConRestApiBaseTest
	{
		[SetUp]
		public async Task SetUp()
		{
			ConnectionApiClient = await ApiFactory.CreateApiClient();

			string connProjectFilePath = Path.Combine(ProjectPath, "models.ideaCon");
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
		public async Task ModifyCommonOperationsPropertiesTest()
		{
			var connectionId = Project!.Connections.First().Id;

			var newPlate = new ConMprlElement { MprlName = "S 450" };
			await ConnectionApiClient!.Material!.AddMaterialSteelAsync(ActiveProjectId, newPlate);
			var steelMaterials = await ConnectionApiClient!.Material!.GetSteelMaterialsAsync(ActiveProjectId);
			var addedPlateMat = (MatSteelEc2)steelMaterials.Last();

			var newWeld = new ConMprlElement { MprlName = "S 275" };
			await ConnectionApiClient!.Material!.AddMaterialWeldAsync(ActiveProjectId, newWeld);
			var weldMaterials = await ConnectionApiClient!.Material!.GetWeldingMaterialsAsync(ActiveProjectId);
			var addedWeldMat = (MatWeldingEc2)weldMaterials.Last();

			var operationProperties = new IdeaStatiCa.ConnectionApi.Model.ConOperationCommonProperties()
			{
				PlateMaterialId = addedPlateMat.Id,
				WeldMaterialId = addedWeldMat.Id,
			};

			await ConnectionApiClient!.Operation!.UpdateCommonOperationPropertiesAsync(ActiveProjectId, connectionId, operationProperties);

			var commonProperties = await ConnectionApiClient!.Operation!.GetCommonOperationPropertiesAsync(ActiveProjectId, connectionId);

			commonProperties!.PlateMaterialId!.Value.Should().Be(addedPlateMat.Id);
			commonProperties!.WeldMaterialId!.Value.Should().Be(addedWeldMat.Id);
		}

		[Test]
		public async Task ShouldPredesignWelds_FullStrength_Test()
		{
			var connectionId = Project!.Connections.First().Id;

			var res = await ConnectionApiClient!.Operation!.PreDesignWeldsAsync(ActiveProjectId, connectionId, IdeaStatiCa.Api.Connection.Model.Connection.ConWeldSizingMethodEnum.FullStrength);

			res.Should().Be("\"Connection 2 welds were set PredesignWeldsToFullStrength.\"");
		}
	}
}
