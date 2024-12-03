using FluentAssertions;
using IdeaStatiCa.Api.Connection.Model;

namespace ST_ConnectionRestApi
{
	public class TemplateApplicationTests : ConRestApiBaseTest
	{
		private string ConnectionTemplate { get; set; }
		private string ParametricTemplate { get; set; }

		[OneTimeSetUp]
		public new async Task OneTimeSetUp()
		{
			// Read the XML string from the file
			string xmlString = File.ReadAllText("Projects/Corner-with-stud.contemp");
			string xmlStringParametric = File.ReadAllText("Projects/template 1.contemp");

			// Store the XML string to the ConnectionTemplate property
			ConnectionTemplate = xmlString;
			ParametricTemplate = xmlStringParametric;

			if (string.IsNullOrEmpty(ConnectionTemplate) || string.IsNullOrEmpty(ParametricTemplate))
			{
				throw new Exception("Connection template is empty");
			}

			await base.OneTimeSetUp();
		}

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
		public async Task ShouldGetTemplateConversion()
		{
			if(ConnectionApiClient == null)
			{
				throw new Exception("ConnectionApiClient is null");
			}

			string connProjectFilePath = Path.Combine(ProjectPath, "ConTemplateCorner-Empty.ideaCon");
			this.Project = await ConnectionApiClient.Project.OpenProjectAsync(connProjectFilePath);
			this.ActiveProjectId = Project.ProjectId;
			if (this.ActiveProjectId == Guid.Empty)
			{
				throw new Exception("Project is not opened");
			}

			this.Project = await ConnectionApiClient.Project.GetProjectDataAsync(ActiveProjectId);

			Project.Should().NotBeNull();
			Project.ProjectId.Should().NotBe(Guid.Empty);
			var connection = Project!.Connections.First();

			ConTemplateMappingGetParam getMappingParam = new ConTemplateMappingGetParam
			{
				Template = this.ConnectionTemplate
			};

			var templateMapping = await ConnectionApiClient.Template.GetDefaultTemplateMappingAsync(ActiveProjectId, connection.Id, getMappingParam);
			templateMapping.Should().NotBeNull();
			templateMapping!.Conversions.Should().NotBeNullOrEmpty();
			templateMapping.CountryCode.Should().Be("ECEN");
			templateMapping.Conversions.Count.Should().Be(5);

			var conversion1 = templateMapping.Conversions.First();
			conversion1.GetType().Name.Should().Be("CssTemplateConversion", "The type of the first conversion should be CssTemplateConversion");
		}

		[Test]
		public async Task ShouldApplyParametricTemplate()
		{
			if (ConnectionApiClient == null)
			{
				throw new Exception("ConnectionApiClient is null");
			}

			if (this.ParametricTemplate == null)
			{
				throw new Exception("ParametricTemplate is null");
			}

			string connProjectFilePath = Path.Combine(ProjectPath, "models.ideaCon");
			Project = await ConnectionApiClient.Project.OpenProjectAsync(connProjectFilePath, CancellationToken.None);
			this.ActiveProjectId = Project.ProjectId;

			var connection = Project!.Connections.Last();

			ConTemplateMappingGetParam getMappingParam = new ConTemplateMappingGetParam
			{
				Template = this.ParametricTemplate
			};

			var templateMapping = await ConnectionApiClient.Template.GetDefaultTemplateMappingAsync(ActiveProjectId, connection.Id, getMappingParam);
			if (templateMapping == null)
			{
				throw new Exception("Template mapping is null");
			}

			var conversions = templateMapping.Conversions.Where(x => x is PlateMaterialTemplateConversion || x is ElectrodeTemplateConversion);

			foreach (var conversion in conversions)
			{
				conversion.NewValue = "S 275";
			};


			var operationsEmptyConnection = await ConnectionApiClient.Operation.GetOperationsAsync(ActiveProjectId, connection.Id);
			operationsEmptyConnection.Count.Should().Be(0);

			var conApplyTemplateParam = new ConTemplateApplyParam
			{
				ConnectionTemplate = this.ParametricTemplate,
				Mapping = templateMapping
			};

			var applyTemplateResult = await ConnectionApiClient.Template.ApplyTemplateAsync(ActiveProjectId, connection.Id, conApplyTemplateParam);
			applyTemplateResult.Should().NotBeNull();

			var operationsFromTemplate = await ConnectionApiClient.Operation.GetOperationsAsync(ActiveProjectId, connection.Id);
			operationsFromTemplate.Count.Should().Be(6);

			var parameters = await ConnectionApiClient.Parameter.GetParametersAsync(ActiveProjectId, connection.Id, true);
			parameters.Count.Should().Be(10);
		}

		[Test]
		public async Task ShouldApplyTemplate()
		{
			if (ConnectionApiClient == null)
			{
				throw new Exception("ConnectionApiClient is null");
			}

			string connProjectFilePath = Path.Combine(ProjectPath, "ConTemplateCorner-Empty.ideaCon");
			this.Project = await ConnectionApiClient.Project.OpenProjectAsync(connProjectFilePath);
			this.ActiveProjectId = Project.ProjectId;
			if (this.ActiveProjectId == Guid.Empty)
			{
				throw new Exception("Project is not opened");
			}

			var connection = Project!.Connections.First();

			ConTemplateMappingGetParam getMappingParam = new ConTemplateMappingGetParam
			{
				Template = this.ConnectionTemplate
			};

			var templateMapping = await ConnectionApiClient.Template.GetDefaultTemplateMappingAsync(ActiveProjectId, connection.Id, getMappingParam);
			if (templateMapping == null)
			{
				throw new Exception("Template mapping is null");
			}

			var operationsEmptyConnection = await ConnectionApiClient.Operation.GetOperationsAsync(ActiveProjectId, connection.Id);
			operationsEmptyConnection.Count.Should().Be(0);

			// set cssId = 5 for stiffening member
			var conversion0 = templateMapping.Conversions[0];
			conversion0.NewTemplateId = "#5";

			var conApplyTemplateParam = new ConTemplateApplyParam
			{
				ConnectionTemplate = this.ConnectionTemplate,
				Mapping = templateMapping
			};


			var applyTemplateResult = await ConnectionApiClient.Template.ApplyTemplateAsync(ActiveProjectId, connection.Id, conApplyTemplateParam);
			applyTemplateResult.Should().NotBeNull();

			var operationsFromTemplate = await ConnectionApiClient.Operation.GetOperationsAsync(ActiveProjectId, connection.Id);
			operationsFromTemplate.Count.Should().Be(5);

			// TODO - validate cssId for stiffening member - shoul be 5
		}

		[Test]
		public async Task ShouldGetTemplate()
		{
			if (ConnectionApiClient == null)
			{
				throw new Exception("ConnectionApiClient is null");
			}

			string connProjectFilePath = Path.Combine(ProjectPath, "ConTemplateCorner-Empty.ideaCon");
			this.Project = await ConnectionApiClient.Project.OpenProjectAsync(connProjectFilePath);
			this.ActiveProjectId = Project.ProjectId;
			if (this.ActiveProjectId == Guid.Empty)
			{
				throw new Exception("Project is not opened");
			}

			var con1 = Project!.Connections.First();
			var templateString = await ConnectionApiClient.Template.CreateConTemplateAsync(ActiveProjectId, con1.Id);
			templateString.Should().NotBeNull();
			templateString.Should().NotBeEmpty();
		}
	}
}
