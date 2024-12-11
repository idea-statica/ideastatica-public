using System.Reflection;
using IdeaStatiCa.RcsApi.Api;
using IdeaStatiCa.RcsApi.Model;
using IdeaStatiCa.RcsApi.Client;
using IdeaStatiCa.RcsApi;

namespace CodeSamples
{
	internal class Program
	{
		static async Task Main(string[] args)
		{
			// Create the client which is connected to the service.
			RcsApiServiceAttacher serviceAttacher = new RcsApiServiceAttacher("http://localhost:5000");
			using (var rcsClient = await serviceAttacher.CreateApiClient())
			{
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
						await (Task)selectedMethod.Invoke(null, new object[] { rcsClient });

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
}
