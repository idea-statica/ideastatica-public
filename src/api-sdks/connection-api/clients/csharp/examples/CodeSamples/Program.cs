using IdeaStatiCa.ConnectionApi;
using IdeaStatiCa.ConnectionApi.Api;
using IdeaStatiCa.ConnectionApi.Model;
using System.Reflection;

namespace CodeSamples
{
	internal class Program
	{
		static async Task Main(string[] args)
		{
			// Create the client which is connected to the service.
			ConnectionApiClientFactory clientFactory = new ConnectionApiClientFactory("http://localhost:5000");
			ConnectionApiClient conClient = (ConnectionApiClient)await clientFactory.CreateConnectionApiClient();

			// Get example methods from ClientExamples using reflection
			MethodInfo[] exampleMethods = ClientExamples.GetExampleMethods();

			Console.WriteLine("Select an Example to Run:");

			// List example methods
			for (int i = 0; i < exampleMethods.Length; i++)
			{
				Console.WriteLine($"{i + 1}. {exampleMethods[i].Name}");
			}
			Console.WriteLine("0. Exit");

			while (true)
			{
				string choice = Console.ReadLine();
				if (int.TryParse(choice, out int selectedIndex) && selectedIndex >= 0 && selectedIndex <= exampleMethods.Length)
				{
					if (selectedIndex == 0)
					{
						Console.WriteLine("Exiting...");
						return;
					}

					// Invoke the selected method
					MethodInfo selectedMethod = exampleMethods[selectedIndex - 1];
					await (Task)selectedMethod.Invoke(null, new object[] { conClient });

					Console.WriteLine("Select another Example or Exit:");
				}
				else
				{
					Console.WriteLine("Invalid choice, please try again.");
				}
			}
		}
	}
}