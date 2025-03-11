using IdeaRS.OpenModel.Connection;
using IdeaStatiCa.Api.Connection.Model;
using IdeaStatiCa.ConnectionApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace CodeSamples
{
	public partial class ClientExamples
	{
		public static async Task CalculateConnection(IConnectionApiClient conClient)
		{
			string filePath = "C:\\Users\\NathanLuke\\Desktop\\test-output\\FlexibleEndplate_stop_at_limit.ideaCon";

			ConProject conProject = await conClient.Project.OpenProjectAsync(filePath);

			var connections = await conClient.Connection.GetConnectionsAsync(conClient.ActiveProjectId);
			int connectionId = connections[0].Id;

			//Connection Setup does not work
			var connectionSetup = await conClient.Project.GetSetupAsync(conClient.ActiveProjectId);
			string jsonSetup = JsonConvert.SerializeObject(connectionSetup, Formatting.Indented);

			Console.WriteLine(jsonSetup);

			ConCalculationParameter parameter = new ConCalculationParameter()
			{
				ConnectionIds = new List<int> { connectionId },
				AnalysisType = ConAnalysisTypeEnum.Stress_Strain
			};

			var output = await conClient.Calculation
				.CalculateAsync(conClient.ActiveProjectId, parameter)
				.ConfigureAwait(false);

			if (output == null)
			{
				throw new Exception("Calculation provides no results");
			}

			await conClient.Project.CloseProjectAsync(conClient.ActiveProjectId);
		}

		public static async Task CalculateConnection_JointDesignResistance(IConnectionApiClient conClient)
		{
			//string filePath = "C:\\Users\\NathanLuke\\Desktop\\test-output\\FlexibleEndplate.ideaCon";
			string filePath = "C:\\Users\\NathanLuke\\Desktop\\test.ideaCon";


			ConProject conProject = await conClient.Project.OpenProjectAsync(filePath);

			var connections = await conClient.Connection.GetConnectionsAsync(conClient.ActiveProjectId);
			int connectionId = connections[0].Id;

			ConCalculationParameter parameter = new ConCalculationParameter()
			{
				ConnectionIds = new List<int> { connectionId },
				AnalysisType = ConAnalysisTypeEnum.Total_Design
			};

			var connectionSetup = await conClient.Project.GetSetupAsync(conClient.ActiveProjectId);
			string jsonSetup = JsonConvert.SerializeObject(connectionSetup, Formatting.Indented);

			Console.WriteLine(jsonSetup);

			List<string> rawResults = await conClient.Calculation.GetRawJsonResultsAsync(conClient.ActiveProjectId, parameter);
			string conResult = rawResults[0];

			if (rawResults != null)
			{
				dynamic obj = JsonConvert.DeserializeObject(conResult);
				Console.WriteLine(obj.totalCapacity["1"].appliedLoadPercentage); // John
			}
			else
			{
				throw new Exception("Calculation provides no results");
			}

			var output = await conClient.Calculation
				.CalculateAsync(conClient.ActiveProjectId, parameter)
				.ConfigureAwait(false);

			await conClient.Project.CloseProjectAsync(conClient.ActiveProjectId);
		}
	}
}
