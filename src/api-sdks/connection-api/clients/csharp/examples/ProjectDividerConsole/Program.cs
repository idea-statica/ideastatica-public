using IdeaStatiCa.ConnectionApi;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectDividerConsole
{
	internal class Program
	{
		static async Task Main(string[] args)
		{
			var ideaPath = @"C:\Program Files\IDEA StatiCa\StatiCa 25.1";
			var runner = new ConnectionApiServiceRunner(ideaPath);

			Console.WriteLine("Starting Connection API...");
			var client = await runner.CreateApiClient();

			Console.WriteLine("Input folder with IDEA projects:");
			var inputFolder = Console.ReadLine()?.Trim();

			Console.WriteLine("Output folder for divided projects:");
			var outputFolder = Console.ReadLine()?.Trim();

			if (string.IsNullOrWhiteSpace(inputFolder) || !Directory.Exists(inputFolder))
			{
				Console.WriteLine("Invalid input folder.");
				return;
			}

			Directory.CreateDirectory(outputFolder);

			var files = Directory
				.GetFiles(inputFolder)
				.Where(f => f.EndsWith(".ideaCon", StringComparison.OrdinalIgnoreCase))
				.ToArray();

			foreach (var file in files)
			{
				Console.WriteLine($"Reading project: {file}");

				try
				{
					var tempProject = await client.Project.OpenProjectAsync(file);

					var connectionsMeta = tempProject.Connections
						.Select((c, index) => new
						{
							c.Id,
							c.Name,
							Index = index
						})
						.ToList();

					await client.Project.CloseProjectAsync(tempProject.ProjectId);

					Console.WriteLine($"Found {connectionsMeta.Count} connections");

					for (int i = 0; i < connectionsMeta.Count; i++)
					{
						try
						{
							var meta = connectionsMeta[i];

							var project = await client.Project.OpenProjectAsync(file);

							var connectionToKeep =
								project.Connections.FirstOrDefault(c => c.Id == meta.Id)
								?? project.Connections.ElementAt(meta.Index);

							foreach (var conn in project.Connections.ToList())
							{
								if (conn.Id != connectionToKeep.Id)
								{
									project.Connections.Remove(conn);
								}
							}

							var baseName = Path.GetFileNameWithoutExtension(file);
							var safeName = string.Concat(
								connectionToKeep.Name.Split(Path.GetInvalidFileNameChars())
							);

							var outputPath = Path.Combine(
								outputFolder,
								$"{baseName}_{i + 1}_{safeName}.idea"
							);

							await client.Project.SaveProjectAsync(project.ProjectId, outputPath);
							await client.Project.CloseProjectAsync(project.ProjectId);

							Console.WriteLine($"Saved: {outputPath}");
						}
						catch (Exception ex)
						{
							Console.WriteLine($"⚠ Failed to process connection {i + 1}: {ex.Message}");
						}
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine($"❌ Failed to open project: {file}");
					Console.WriteLine($"   Reason: {ex.Message}");
				}
			}

			runner?.Dispose();
			client?.Dispose();
			Console.WriteLine("Done.");
		}
	}
}
