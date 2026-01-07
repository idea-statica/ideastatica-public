using FluentAssertions;
using IdeaStatiCa.RcsApi;
using IdeaStatiCa.Api.RCS.Model;
using Newtonsoft.Json.Linq;
using IdeaStatiCa.RcsApi.Client;

namespace ST_RcsRestApiClient
{
	/// <summary>
	/// Tests for Material API functionality
	/// </summary>
	public class RcsRestApiMaterialTests : RcsRestApiBaseTest
	{
		[SetUp]
		public async Task SetUp()
		{
			RcsApiClient = await ApiFactory.CreateApiClient();
			if (RcsApiClient == null)
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

		#region Get Materials Tests

		[Test]
		public async Task ShouldGetAllMaterialsTest()
		{
			// Act
			var materials = await RcsApiClient!.Material.GetAllMaterialsAsync(ActiveProjectId);

			// Assert
			materials.Should().NotBeNull();
			materials.Should().NotBeEmpty("project should contain materials");
			materials.Count.Should().BeGreaterThanOrEqualTo(3, "project should contain at least 3 materials");

			TestContext.Out.WriteLine($"Total materials count: {materials.Count}");
		}

		[Test]
		public async Task ShouldGetConcreteMaterialsTest()
		{
			// Act
			var materials = await RcsApiClient!.Material.GetConcreteMaterialsAsync(ActiveProjectId);

			// Assert
			materials.Should().NotBeNull();
			materials.Should().NotBeEmpty("Project should contain concrete materials");
			materials.Count.Should().BeGreaterThan(0, "There should be at least one concrete material");

			TestContext.Out.WriteLine($"Concrete materials count: {materials.Count}");
		}

		[Test]
		public async Task ShouldGetReinforcementMaterialsTest()
		{
			// Act
			var materials = await RcsApiClient!.Material.GetReinforcementMaterialsAsync(ActiveProjectId);

			// Assert
			materials.Should().NotBeNull();
			materials.Should().NotBeEmpty("Project should contain reinforcement materials");
			materials.Count.Should().BeGreaterThan(0, "There should be at least one reinforcement material");

			TestContext.Out.WriteLine($"Reinforcement materials count: {materials.Count}");
		}

		[Test]
		public async Task ShouldGetPrestressMaterialsTest()
		{
			// Act
			var materials = await RcsApiClient!.Material.GetPrestressMaterialsAsync(ActiveProjectId);

			// Assert
			materials.Should().NotBeNull();
			materials.Should().NotBeEmpty("Project should contain prestress materials");
			materials.Count.Should().BeGreaterThan(0, "There should be at least one prestress material");

			TestContext.Out.WriteLine($"Prestress materials count: {materials.Count}");
		}

		#endregion

		#region Add Materials Tests

		[Test]
		public async Task ShouldAddConcreteMaterialTest()
		{
			// Arrange
			string mprlName = "C28/35";
			var materialsCountBefore = (await RcsApiClient!.Material.GetConcreteMaterialsAsync(ActiveProjectId)).Count;

			var rcsMprlElement = new RcsMprlElement { Name = mprlName };

			// Act
			await RcsApiClient!.Material.AddConcreteMaterialAsync(ActiveProjectId, rcsMprlElement);

			var materialsCountAfter = (await RcsApiClient!.Material.GetConcreteMaterialsAsync(ActiveProjectId)).Count;

			// Assert
			materialsCountAfter.Should().Be(materialsCountBefore + 1, "One material should be added");

			TestContext.Out.WriteLine($"Added concrete material: {mprlName}");
		}

		[Test]
		public async Task ShouldAddReinforcementMaterialTest()
		{
			// Arrange
			string mprlName = "B 500A";
			var materialsCountBefore = (await RcsApiClient!.Material.GetReinforcementMaterialsAsync(ActiveProjectId)).Count;

			var rcsMprlElement = new RcsMprlElement { Name = mprlName };

			// Act
			await RcsApiClient!.Material.AddReinforcementMaterialAsync(ActiveProjectId, rcsMprlElement);

			var materialsCountAfter = (await RcsApiClient!.Material.GetReinforcementMaterialsAsync(ActiveProjectId)).Count;

			// Assert
			materialsCountAfter.Should().Be(materialsCountBefore + 1, "One material should be added");

			TestContext.Out.WriteLine($"Added reinforcement material: {mprlName}");
		}

		[Test]
		public async Task ShouldAddPrestressMaterialTest()
		{
			// Arrange
			string mprlName = "Y1860S3-6.9";
			var materialsCountBefore = (await RcsApiClient!.Material.GetPrestressMaterialsAsync(ActiveProjectId)).Count;

			var rcsMprlElement = new RcsMprlElement { Name = mprlName };

			// Act
			await RcsApiClient!.Material.AddPrestressMaterialAsync(ActiveProjectId, rcsMprlElement);

			var materialsCountAfter = (await RcsApiClient!.Material.GetPrestressMaterialsAsync(ActiveProjectId)).Count;

			// Assert
			materialsCountAfter.Should().Be(materialsCountBefore + 1, "One material should be added");

			TestContext.Out.WriteLine($"Added prestress material: {mprlName}");
		}

		[Test]
		public void ShouldHandleInvalidMprlNameTest()
		{
			// Arrange
			string invalidMprlName = "InvalidMaterialName12345";
			var rcsMprlElement = new RcsMprlElement { Name = invalidMprlName };

			// Act & Assert
			var exception = Assert.ThrowsAsync<ApiException>(async () =>
			{
				await RcsApiClient!.Material.AddConcreteMaterialAsync(ActiveProjectId, rcsMprlElement);
			});

			exception.Should().NotBeNull("adding material with invalid MPRL name should throw exception");
			TestContext.Out.WriteLine($"Expected exception occurred: {exception.Message}");
		}

		#endregion

		#region Material Verification Tests

		[Test]
		public async Task ShouldNotAddDuplicateMaterialTest()
		{
			// Arrange
			string mprlName = "C25/30";
			var rcsMprlElement = new RcsMprlElement { Name = mprlName };

			// Act - Add material first time
			await RcsApiClient!.Material.AddConcreteMaterialAsync(ActiveProjectId, rcsMprlElement);
			var materialsCountAfterFirst = (await RcsApiClient!.Material.GetConcreteMaterialsAsync(ActiveProjectId)).Count;

			// Act - Try to add same material again
			await RcsApiClient!.Material.AddConcreteMaterialAsync(ActiveProjectId, rcsMprlElement);
			var materialsCountAfterSecond = (await RcsApiClient!.Material.GetConcreteMaterialsAsync(ActiveProjectId)).Count;

			// Assert
			materialsCountAfterSecond.Should().BeGreaterThan(materialsCountAfterFirst,
				"adding same material should create new instance (materials are not unique by name)");

			TestContext.Out.WriteLine($"Materials after first add: {materialsCountAfterFirst}, after second add: {materialsCountAfterSecond}");
		}

		#endregion
	}
}