using IdeaStatiCa.ConnectionApi.Model;
using FluentAssertions;

namespace ST_ConnectionRestApi
{
	public class ConnectionRestApiTests : ConRestApiBaseTest
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

			string connProjectFilePath = Path.Combine(ProjectPath, "Simple-1-ECEN.ideaCon");
			this.Project = await ConnectionApiClient.Project.OpenProjectAsync(connProjectFilePath);
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
			if (ConnectionApiClient != null)
			{
				await ConnectionApiClient!.DisposeAsync();
			}
		}

		[Test]
		public async Task ShouldGetConProjectData()
		{
			var projectData = await ConnectionApiClient!.Project.GetProjectDataAsync(ActiveProjectId);

			projectData.Should().NotBeNull();
			projectData.ProjectInfo.Name.Should().Be("Name - Simple-1-ECEN");
			projectData.ProjectInfo.Description.Should().Be("Description - Simple-1-ECEN");
			projectData.ProjectInfo.ProjectNumber.Should().Be("12345");
		}

		[Test]
		public async Task ShouldCloseProject()
		{
			await ConnectionApiClient!.Project!.CloseProjectAsync(ActiveProjectId.ToString());
		}

		[Test]
		public async Task ShouldGetConnections()
		{
			var connections = await ConnectionApiClient!.Connection!.GetAllConnectionsDataAsync(ActiveProjectId);
			connections.Should().NotBeNull();
			connections.Count.Should().Be(3);

			var con1 = connections[0];
			con1.Id.Should().Be(1);
			con1.AnalysisType.Should().Be(ConAnalysisTypeEnum.StressStrain);
			con1.Name.Should().Be("1Col-2Beams-Welded");
			con1.Description.Should().Be("Welded connection");

			var con2 = connections[1];
			con2.Id.Should().Be(2);
			con2.AnalysisType.Should().Be(ConAnalysisTypeEnum.Stiffness);

			var con3 = connections[2];
		}

		[Test]
		public async Task ShouldGetConnection()
		{
			// request connection id = 2
			var con2 = await ConnectionApiClient!.Connection!.GetConnectionDataAsync(ActiveProjectId, 2);
			con2.Id.Should().Be(2);
			con2.AnalysisType.Should().Be(ConAnalysisTypeEnum.Stiffness);
		}

		[Test]
		public async Task ShouldDownloadConnection()
		{
			string tempFileName = Path.GetTempFileName()!;
			try
			{
				await ConnectionApiClient!.Project!.SaveProjectAsync(ActiveProjectId, tempFileName);

				bool fileExists = File.Exists(tempFileName);
				fileExists.Should().BeTrue("Ideacon project should be downloaded");

				FileInfo fileInfo = new FileInfo(tempFileName);
				long fileSize = fileInfo.Length;

				fileSize.Should().BeGreaterThan(0, "The downloaded file should not be empty");
			}
			finally
			{
				File.Delete(tempFileName);
			}
		}

		public async Task ShouldUpdateConnection()
		{
			const string NewConnectionName = "Updated name";
			var con1 = await ConnectionApiClient!.Connection!.GetConnectionDataAsync(ActiveProjectId, 1);
			con1.Id.Should().Be(1);

			con1.Name.Should().Be("1Col-2Beams-Welded");
			con1.Name = NewConnectionName;

			var updatedConnection1 = await ConnectionApiClient!.Connection!.UpdateConnectionDataAsync(ActiveProjectId, 1, con1);

			updatedConnection1.Id.Should().Be(1);
			updatedConnection1.Name.Should().Be(NewConnectionName, "The data in the response should include updated name of connection");

			var con1_updated = await ConnectionApiClient!.Connection!.GetConnectionDataAsync(ActiveProjectId, 1);
			updatedConnection1.Id.Should().Be(1);
			updatedConnection1.Name.Should().Be(NewConnectionName, "The change should be persistent");
		}

		[Test]
		public async Task ShouldGetConnectionIOMModel()
		{
			var con1 = await ConnectionApiClient!.Connection!.GetConnectionDataAsync(ActiveProjectId, 1);
			var conData = await ConnectionApiClient!.Export!.ExportConnectionDataAsync(ActiveProjectId, con1.Id);
			conData.Should().NotBeNull();
		}

		[Test]
		public async Task ShouldGetAllMembers()
		{
			var members = await ConnectionApiClient!.Member!.GetAllMemberDataAsync(ActiveProjectId, 1);
			members.Count.Should().Be(3);

			var mem1 = members[0];
			mem1.Id.Should().Be(1);
			mem1.Name.Should().Be("C");
			mem1.CrossSectionId.Should().Be(1);
			mem1.Active.Should().BeTrue();
			mem1.IsBearing.Should().BeTrue();
			mem1.IsContinuous.Should().BeTrue();
			mem1.MirrorY.Should().BeFalse();
			mem1.MirrorZ.Should().BeFalse();

			var mem2 = members[1];
			mem2.Id.Should().Be(2);
			mem2.Name.Should().Be("B1");
			mem2.CrossSectionId.Should().Be(2);
			mem2.Active.Should().BeTrue();
			mem2.IsBearing.Should().BeFalse();
			mem2.IsContinuous.Should().BeFalse();
			mem2.MirrorY.Should().BeFalse();
			mem2.MirrorZ.Should().BeFalse();

			var mem3 = members[2];
			mem3.Id.Should().Be(3);
			mem3.Name.Should().Be("B2");
			mem3.CrossSectionId.Should().Be(3);
			mem3.Active.Should().BeTrue();
			mem3.IsBearing.Should().BeFalse();
			mem3.IsContinuous.Should().BeFalse();
			mem3.MirrorY.Should().BeFalse();
			mem3.MirrorZ.Should().BeFalse();
		}

		[Test]
		public async Task ShouldGetOneMember()
		{
			var member = await ConnectionApiClient!.Member!.GetMemberDataAsync(ActiveProjectId, 1, 1);

			member.Id.Should().Be(1);
			member.Name.Should().Be("C");
			member.CrossSectionId.Should().Be(1);
			member.Active.Should().BeTrue();
			member.IsBearing.Should().BeTrue();
			member.IsContinuous.Should().BeTrue();
			member.MirrorY.Should().BeFalse();
			member.MirrorZ.Should().BeFalse();
		}

		[Test]
		public async Task SetBearingMember()
		{
			var member = await ConnectionApiClient!.Member!.GetMemberDataAsync(ActiveProjectId, 1, 1);

			member.Id.Should().Be(1);
			member.Name.Should().Be("C");
			member.CrossSectionId.Should().Be(1);
			member.Active.Should().BeTrue();
			member.IsBearing.Should().BeTrue();
			member.IsContinuous.Should().BeTrue();
			member.MirrorY.Should().BeFalse();
			member.MirrorZ.Should().BeFalse();

			var bearingMember = await ConnectionApiClient!.Member!.SetBearingMemberAsync(ActiveProjectId, 1, 2);
			bearingMember.IsBearing.Should().BeTrue();

			member = await ConnectionApiClient!.Member!.GetMemberDataAsync(ActiveProjectId, 1, 1);
			member.IsBearing.Should().BeFalse();
		}

		[Test]
		public async Task ShouldUpdateMember()
		{
			var member = await ConnectionApiClient!.Member!.GetMemberDataAsync(ActiveProjectId, 3, 1);

			member.Name = "D";
			member.CrossSectionId = 2;
			member.IsContinuous = false;
			member.MirrorY = true;
			member.MirrorZ = true;

			var updatedMember = await ConnectionApiClient!.Member!.UpdateMemberAsync(ActiveProjectId, 3, 1, member);
			updatedMember.Name.Should().Be("D");
			updatedMember.CrossSectionId?.Should().Be(2);
			updatedMember.IsContinuous.Should().BeFalse();
			updatedMember.MirrorY.Should().BeTrue();
			updatedMember.MirrorZ.Should().BeTrue();
		}

		[Test]
		public async Task ShouldGetConnectionData()
		{
			var con1 = Project!.Connections.First();
			con1.Id.Should().Be(1);

			var connectionData = await ConnectionApiClient!.Export!.ExportConnectionDataAsync(ActiveProjectId, 1);
			connectionData.Should().NotBeNull();
		}

		[Test]
		public async Task ShouldExportConnectionToIfc()
		{
			var con1 = Project!.Connections.First();
			con1.Id.Should().Be(1);

			string tempFileName = Path.GetTempFileName()!;
			try
			{
				await ConnectionApiClient!.Export!.ExportConToIfcFileAsync(ActiveProjectId, con1.Id, tempFileName);

				bool fileExists = File.Exists(tempFileName);
				fileExists.Should().BeTrue("Ifc should be saved");

				FileInfo fileInfo = new FileInfo(tempFileName);
				long fileSize = fileInfo.Length;

				fileSize.Should().BeGreaterThan(0, "The downlifcoaded file should not be empty");
			}
			finally
			{
				File.Delete(tempFileName);
			}
		}

		[Test]
		public async Task ShouldCalculateStressStrain()
		{
			var con1 = Project!.Connections.First();
			con1.Id.Should().Be(1);

			List<int> conToCalc = new List<int>() { con1.Id };
			ConCalculationParameter conCalculationParameter = new ConCalculationParameter()
			{
				AnalysisType = ConAnalysisTypeEnum.StressStrain,
				ConnectionIds = new List<int>() { con1.Id }
			};

			var cbfemResults = await ConnectionApiClient!.Calculation!.CalculateAsync(ActiveProjectId, conCalculationParameter);
			cbfemResults.Should().NotBeNull();
			cbfemResults.Count.Should().Be(1);
			var res1 = cbfemResults[0];
			res1.Passed.Should().BeTrue();
			res1.ResultSummary.Count.Should().Be(4);
		}

		[Test]
		public async Task ShouldCalculateBuckling()
		{
			var con1 = Project!.Connections.First();
			con1.Id.Should().Be(1);

			List<int> conToCalc = new List<int>() { con1.Id };
			ConCalculationParameter conCalculationParameter = new ConCalculationParameter()
			{
				AnalysisType = ConAnalysisTypeEnum.Buckling,
				ConnectionIds = new List<int>() { con1.Id }
			};

			var cbfemResults = await ConnectionApiClient!.Calculation!.CalculateAsync(ActiveProjectId, conCalculationParameter);;
			cbfemResults.Should().NotBeNull();
			cbfemResults.Count.Should().Be(1);
			var res1 = cbfemResults[0];
			res1.Passed.Should().BeTrue();
			res1.ResultSummary.Count.Should().Be(4);

			//check buckling
			var bucklingResult = res1.ResultSummary.Last();
			bucklingResult.Skipped.Should().BeFalse();
			bucklingResult.Name.Equals("Buckling");
		}

		[Test]
		public async Task ShouldGetResult()
		{
			string connProjectFilePath = Path.Combine(ProjectPath, "Parametric.ideaCon");

			using (var apiClient2 = await ApiFactory.CreateConnectionApiClient())
			{
				var project2 = await apiClient2!.Project.OpenProjectAsync(connProjectFilePath);

				var con1 = project2.Connections.First();
				List<int> conToCalc = new List<int>() { con1.Id };

				ConCalculationParameter conCalculationParameter = new ConCalculationParameter()
				{
					AnalysisType = ConAnalysisTypeEnum.StressStrain,
					ConnectionIds = new List<int>() { con1.Id }
				};

				await apiClient2!.Calculation!.CalculateAsync(project2.ProjectId, conCalculationParameter);

				var cbfemResults = await apiClient2!.Calculation!.GetResultsAsync(project2.ProjectId, conCalculationParameter);
				cbfemResults.Should().NotBeEmpty();
			}
		}

		[Test]
		public async Task ShouldGetProductionCost()
		{
			var con1 = Project!.Connections.First();
			var cost = await ConnectionApiClient!.Connection.GetProductionCostAsync(ActiveProjectId, con1.Id);
			cost.Should().NotBeNull();
			cost.TotalEstimatedCost.Should().BeGreaterThan(0);
		}

		// TODO - not working

		[Test]
		public async Task ShouldGetAndUpdateConnectionSetup()
		{
			var connectionSetup = await ConnectionApiClient!.Project.GetSetupAsync(ActiveProjectId);

			connectionSetup.HssLimitPlasticStrain.Should().Be(0.01);

			connectionSetup.HssLimitPlasticStrain = 0.02;

			var updateResponse = await ConnectionApiClient!.Project.UpdateSetupAsync(ActiveProjectId, connectionSetup);

			var updatedConnectionSetup = await ConnectionApiClient!.Project!.GetSetupAsync(ActiveProjectId);

			updatedConnectionSetup.HssLimitPlasticStrain.Should().Be(0.02);
		}

		[Test]
		public async Task ShouldGetSceneData()
		{
			var con1 = Project!.Connections.First();
			var sceneData = await ConnectionApiClient!.Presentation.GetDataScene3DAsync(ActiveProjectId, con1.Id);
			sceneData.Should().NotBeNull();
			sceneData.Vertices.Should().NotBeEmpty();
		}
	}
}