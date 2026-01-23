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
					// 1️⃣ otevřeme zdrojový projekt JEDNOU (jen metadata)
					var sourceProject = await client.Project.OpenProjectAsync(file);

					var connections = sourceProject.Connections
						.Select(c => new
						{
							c.Id,
							c.Name
						})
						.ToList();

					await client.Project.CloseProjectAsync(sourceProject.ProjectId);

					Console.WriteLine($"Found {connections.Count} connections");

					// 2️⃣ pro každý connection vytvoříme kopii projektu
					for (int i = 0; i < connections.Count; i++)
					{
						var keep = connections[i];

						try
						{
							// otevřeme NOVOU instanci projektu
							var project = await client.Project.OpenProjectAsync(file);

							// smažeme všechny ostatní connection
							foreach (var conn in project.Connections)
							{
								if (conn.Id != keep.Id)
								{
									await client.Connection.DeleteConnectionAsync(
										project.ProjectId,
										conn.Id
									);
								}
							}

							var baseName = Path.GetFileNameWithoutExtension(file);
							var safeName = string.Concat(
								keep.Name.Split(Path.GetInvalidFileNameChars())
							);

							var outputPath = Path.Combine(
								outputFolder,
								$"{baseName}_{i + 1}_{safeName}.ideaCon"
							);

							await client.Project.SaveProjectAsync(
								project.ProjectId,
								outputPath
							);

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

			client?.Dispose();
			runner?.Dispose();

			Console.WriteLine("Done.");
		}
	}
}
