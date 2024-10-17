using FluentAssertions;

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

			ConnectionApiClient = await ApiFactory.CreateConnectionApiClient();

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
		//public async Task ShouldGetTemplateConversion()
		//{
		//	ConTemplateMappingGetParam getMappingParam = new ConTemplateMappingGetParam
		//	{
		//		Template = this.ConnectionTemplate
		//	};

		//	var connection = Project!.Connections.First();
		//	var templateMapping = await ConnectionApiClient!.Template.GetDefaultTemplateMappingAsync(ActiveProjectId, connection.Id, getMappingParam);
		//	templateMapping.Should().NotBeNull();
		//	templateMapping!.Conversions.Should().NotBeNullOrEmpty();
		//	templateMapping.CountryCode.Should().Be("ECEN");
		//	templateMapping.Conversions.Count.Should().Be(5);

		//	var conversion1 = templateMapping.Conversions.First();
		//	conversion1.GetType().Name.Should().Be("CssTemplateConversion", "The type of the first conversion should be CssTemplateConversion");
		//}

		//[Test]
		//public async Task ShouldApplyParametricTemplate()
		//{
		//	string connProjectFilePath = Path.Combine(ProjectPath, "models.ideaCon");
		//	Project = await ConnApiController!.OpenProjectAsync(connProjectFilePath, CancellationToken.None);

		//	var connection = Project!.Connections.Last();
		//	var templateMapping = await ConnApiController!.GetTemplateMappingAsync(connection.Id, this.ParametricTemplate, CancellationToken.None);
		//	if (templateMapping == null)
		//	{
		//		throw new Exception("Template mapping is null");
		//	}

		//	var mappings = templateMapping.Conversions.Where(x => x is PlateMaterialTemplateConversion || x is ElectrodeTemplateConversion);
				
		//	foreach (var mapping in mappings)
		//	{
		//		mapping.NewValue = "S 275";
		//	};


		//	var operationsEmptyConnection = await ConnApiController!.GetOperationsAsync(connection.Id, CancellationToken.None);
		//	operationsEmptyConnection.Count.Should().Be(0);

		//	var applyTemplateResult = await ConnApiController!.ApplyConnectionTemplateAsync(connection.Id, this.ParametricTemplate, templateMapping, CancellationToken.None);
		//	applyTemplateResult.Should().NotBeNull();

		//	var operationsFromTemplate = await ConnApiController!.GetOperationsAsync(connection.Id, CancellationToken.None);
		//	operationsFromTemplate.Count.Should().Be(6);

		//	var parameters = await ConnApiController!.GetParametersAsync(connection.Id, true);
		//	parameters.Count.Should().Be(10);
		//}

		//[Test]
		//public async Task ShouldApplyTemplate()
		//{
		//	var connection = Project!.Connections.First();
		//	var templateMapping = await ConnApiController!.GetTemplateMappingAsync(connection.Id, this.ConnectionTemplate, CancellationToken.None);
		//	if (templateMapping == null)
		//	{
		//		throw new Exception("Template mapping is null");
		//	}

		//	var operationsEmptyConnection = await ConnApiController!.GetOperationsAsync(connection.Id, CancellationToken.None);
		//	operationsEmptyConnection.Count.Should().Be(0);

		//	// set cssId = 5 for stiffening member
		//	var conversion0 = templateMapping.Conversions[0];
		//	conversion0.NewTemplateId = "#5";

		//	var applyTemplateResult = await ConnApiController!.ApplyConnectionTemplateAsync(connection.Id, this.ConnectionTemplate, templateMapping, CancellationToken.None);
		//	applyTemplateResult.Should().NotBeNull();

		//	var operationsFromTemplate = await ConnApiController!.GetOperationsAsync(connection.Id, CancellationToken.None);
		//	operationsFromTemplate.Count.Should().Be(5);

		//	// TODO - validate cssId for stiffening member - shoul be 5
		//}

		//[Test]
		//public async Task ShouldGetTemplate()
		//{
		//	var con1 = Project!.Connections.First();
		//	var templateString = await ConnApiController!.GetConnectionTemplateAsync(con1.Id, CancellationToken.None);
		//	templateString.Should().NotBeNull();
		//	templateString.Should().NotBeEmpty();
		//}
	}
}
