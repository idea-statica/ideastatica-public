using FluentAssertions;
using IdeaRS.OpenModel.CrossSection;
using IdeaRS.OpenModel.Material;

namespace ST_ConnectionRestApi
{
	public class MaterialControllerTests : ConRestApiBaseTest
	{
		[SetUp]
		public async Task SetUp()
		{
			//if (this.RunServer)
			//{
			//	ConnectionApiClient = await ApiFactory.CreateConnectionApiClient();
			//}
			//else
			//{
			//	if (ApiUri == null)
			//	{
			//		throw new Exception("ApiUri is not set");
			//	}

			//	ConnectionApiClient = await ApiFactory.CreateConnectionApiClient(ApiUri);
			//}

			ConnectionApiClient = await ApiFactory.CreateConnectionApiClient();

			string connProjectFilePath = Path.Combine(ProjectPath, "Parametric.ideaCon");
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
		public async Task ShouldGetMaterialsAsync()
		{
			var materials = await ConnectionApiClient!.Material.GetAllMaterialsAsync(ActiveProjectId);
			materials.Should().NotBeNull();
		}

		[Test]
		public async Task ShouldGetCrossSectionsAsync()
		{
			var crossSections = await ConnectionApiClient!.Material.GetCrossSectionsAsync(ActiveProjectId);
			crossSections.Count.Should().Be(1);

			if (crossSections.First() is CrossSection css)
			{
				css.Name.Should().Be("CHS400,10");
			}
			else
			{
				throw new Exception("Cross section failed");
			}
		}

		[Test]
		public async Task ShouldGetBoltAssembliesAsync()
		{
			var ba = await ConnectionApiClient!.Material!.GetBoltAssembliesAsync(ActiveProjectId);
			ba.Count().Should().Be(1);

			if (ba.First() is BoltAssembly boltAssembly)
			{
				boltAssembly.Name.Should().Be("M16 8.8");
			}
			else
			{
				throw new Exception("Bolt assembly failed");
			}
		}

		[Test]
		public async Task ShouldAddCrossSectionAsync()
		{
			var css = await ConnectionApiClient!.Material!.GetCrossSectionsAsync(ActiveProjectId);

			var newCss = new IdeaStatiCa.ConnectionApi.Model.ConMprlCrossSection
			{
				MprlName = "IPE240",
				MaterialName = "S 450"
			};

			var added = await ConnectionApiClient!.Material!.AddCrossSectionAsync(ActiveProjectId, newCss);

			var updated = await ConnectionApiClient!.Material!.GetCrossSectionsAsync(ActiveProjectId);
			updated.Count().Should().Be(css.Count() + 1);

			if (updated.Last() is CrossSectionParameter addedCss)
			{
				addedCss.Name.Should().Be("IPE240");
				if (addedCss.Material.Element is MatSteelEc2 material)
				{
					material.Name.Should().Be("S 450");
				}
				else
				{
					throw new Exception("Material incorrectly assigned");
				}
			}
			else
			{
				throw new Exception("Cross section not added");
			}
		}

		[Test]
		public async Task ShouldAddBoltAssembly()
		{
			var boltAssemblies = await ConnectionApiClient!.Material!.GetBoltAssembliesAsync(ActiveProjectId);

			var newBa = new IdeaStatiCa.ConnectionApi.Model.ConMprlElement
			{
				MprlName = "M20 10.9"
			};

			await ConnectionApiClient!.Material!.AddBoltAssemblyAsync(ActiveProjectId, newBa);

			var added = await ConnectionApiClient!.Material!.GetBoltAssembliesAsync(ActiveProjectId);
			if (added.Last() is BoltAssembly ba)
			{
				ba.Name.Should().Be("M20 10.9");
				if (ba.BoltGrade.Element is MaterialBoltGrade material)
				{
					material.Name.Should().Be("10.9");
				}
				else
				{
					throw new Exception("Bolt grade failed");
				}
			}
			else
			{
				throw new Exception("Bolt assembly not added");
			}
		}

		[Test]
		public async Task ShouldAddMaterial()
		{
			var materials = await ConnectionApiClient!.Material!.GetAllMaterialsAsync(ActiveProjectId);

			var newMaterialSteel = new IdeaStatiCa.ConnectionApi.Model.ConMprlElement
			{
				MprlName = "S 450",
			};

			var result = await ConnectionApiClient!.Material!.AddMaterialSteelAsync(ActiveProjectId, newMaterialSteel);

			var added = (await ConnectionApiClient!.Material!.GetAllMaterialsAsync(ActiveProjectId)).Last();

			if (added is MatSteel matSteel)
			{
				matSteel.Name.Should().Be("S 450");
			}
			else
			{
				throw new Exception("Bolt assembly failed");
			}
		}
	}
}
