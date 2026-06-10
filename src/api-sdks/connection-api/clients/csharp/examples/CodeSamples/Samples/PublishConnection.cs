using IdeaStatiCa.Api.Connection.Model;
using IdeaStatiCa.ConnectionApi;

namespace CodeSamples
{
	public partial class ClientExamples
	{
		/// <summary>
		/// Publishes the design of a connection as a template into the user's private set in the Connection Library.
		/// </summary>
		/// <param name="conClient">The connected API Client</param>
		public static async Task PublishConnection(IConnectionApiClient conClient)
		{
			string filePath = "inputs/simple cleat connection.ideaCon";
			await conClient.Project.OpenProjectAsync(filePath);

			var connections = await conClient.Connection.GetConnectionsAsync(conClient.ActiveProjectId);
			int connectionId = connections[0].Id;

			//Publish the connection design to the Private set (use ConDesignSetType.Company to share it with the whole company).
			ConTemplatePublishParam publishParam = new ConTemplatePublishParam
			{
				Name = "Simple cleat connection",
				Author = "Connection API example",
				CompanyName = "IDEA StatiCa",
				DesignSetType = ConDesignSetType.Private
			};

			bool published = await conClient.ConnectionLibrary.PublishConnectionAsync(conClient.ActiveProjectId, connectionId, publishParam);

			Console.WriteLine(published
				? $"Connection '{connections[0].Name}' was published to the {publishParam.DesignSetType} design set."
				: $"Publishing of connection '{connections[0].Name}' failed.");

			//Close the opened project.
			await conClient.Project.CloseProjectAsync(conClient.ActiveProjectId);
		}
	}
}
