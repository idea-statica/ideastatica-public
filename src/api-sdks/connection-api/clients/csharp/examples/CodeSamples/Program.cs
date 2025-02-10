using IdeaStatiCa.Api.Common;
using IdeaStatiCa.Api.Connection;
using IdeaStatiCa.ConnectionApi;
using System.Reflection;

namespace CodeSamples
{
	internal class Program
	{
		static async Task Main(string[] args)
		{
			Console.WriteLine("App Started.");
			Console.WriteLine("-----------");
			
			//SERVICE SELECTION
			
			Console.WriteLine("Please select an option:");
			Console.WriteLine("0. Run API service and attach");
			Console.WriteLine("1. Attach to an already running API service");

			IApiServiceFactory<IConnectionApiClient> service = null;

			while (true)
			{
				Console.Write("Enter your choice (0 or 1): ");
				string serviceChoice = Console.ReadLine();

				if (int.TryParse(serviceChoice, out int selection))
				{
					if (selection == 0)
					{
						// Run API service and attach
						string defaultPath = @"C:\Program Files\IDEA StatiCa\StatiCa 24.1";
						Console.WriteLine($"Provide path to IDEA StatiCa Directory. Hit Enter for default path ({defaultPath}).");

						string path = Console.ReadLine();
						if (string.IsNullOrWhiteSpace(path))
						{
							path = defaultPath;
							Console.WriteLine($"Using default path: {path}");
						}

						if (!Directory.Exists(path))
						{
							Console.WriteLine($"Error: The specified directory '{path}' does not exist. Please try again.");
							continue;
						}

						service = new ConnectionApiServiceRunner(path);
					}
					else if (selection == 1)
					{
						// Attach to already running API service
						string defaultBasePath = "http://localhost:5000";
						Console.WriteLine($"Provide base URL of the running API service. Hit Enter for default base URL ({defaultBasePath}).");

						string basePath = Console.ReadLine();
						if (string.IsNullOrWhiteSpace(basePath))
						{
							basePath = defaultBasePath;
							Console.WriteLine($"Using default base URL: {basePath}");
						}

						if (!Uri.TryCreate(basePath, UriKind.Absolute, out Uri uriResult) ||
							(uriResult.Scheme != Uri.UriSchemeHttp && uriResult.Scheme != Uri.UriSchemeHttps))
						{
							Console.WriteLine($"Error: The specified URL '{basePath}' is invalid. Please try again.");
							continue;
						}

						service = new ConnectionApiServiceAttacher(basePath);
					}

					break; // Exit the loop once the service is successfully initialized
				}
				else
				{
					Console.WriteLine("Invalid input. Please enter 0 or 1.");
				}
			}

			if (service != null)
			{
				Console.WriteLine("Service has been successfully initialized.");
			}
			else
			{
				Console.WriteLine("Service initialization failed. Please restart the application.");
			}

			Console.WriteLine("-----------");

			//CLIENT CREATION

			using (var conClient = await service.CreateApiClient())
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
}