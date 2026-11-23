using IdeaRS.OpenModel;
using IOM.SteelFrameModel;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace IOM.SteelFrameWeb
{
	/// <summary>
	/// Builds the steel frame in IOM, saves it, and has the IDEA StatiCa Viewer web service create an IDEA StatiCa
	/// Connection project from it. It needs no IDEA StatiCa on this computer.
	/// </summary>
	internal static class Program
	{
		/// <summary>
		/// The web service that creates a Connection project from an IOM file.
		/// </summary>
		private const string ServiceUrl = "https://viewer.ideastatica.com/ConnectionViewer/CreateFromIOM";

		/// <summary>
		/// Builds the steel frame, saves it and has the web service create the Connection project.
		/// </summary>
		/// <returns>0 when the project is saved, 1 when the web service fails.</returns>
		private static async Task<int> Main()
		{
			var container = new OpenModelContainer { OpenModel = SteelFrame.CreateIOM(), OpenModelResult = SteelFrame.LoadInternalForces() };
			string iomPath = Path.GetFullPath("SteelFrame.xml");
			Tools.OpenModelContainerToFile(container, iomPath);
			Console.WriteLine($"The IOM is saved in {iomPath}");

			string projectPath = Path.GetFullPath("SteelFrame-web.ideaCon");
			try
			{
				Console.WriteLine($"Sending it to {ServiceUrl} ...");
				using var http = new HttpClient();
				using var content = new ByteArrayContent(File.ReadAllBytes(iomPath));
				content.Headers.ContentType = new MediaTypeHeaderValue("application/xml");
				using HttpResponseMessage response = await http.PostAsync(ServiceUrl, content);
				response.EnsureSuccessStatusCode();

				var result = await response.Content.ReadFromJsonAsync<CreateFromIomResult>();
				File.WriteAllBytes(projectPath, Convert.FromBase64String(result.FileContent));
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine($"The web service failed: {ex.Message}");
				return 1;
			}

			Console.WriteLine($"The project is saved in {projectPath}");
			return 0;
		}

		/// <summary>
		/// The answer of the web service, with the .ideaCon file in Base64.
		/// </summary>
		private sealed record CreateFromIomResult(string FileContent);
	}
}
