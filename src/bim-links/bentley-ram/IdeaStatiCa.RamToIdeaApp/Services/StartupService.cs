using IdeaStatiCa.Plugin;
using IdeaStatiCa.RamToIdeaApp.Models;
using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace IdeaStatiCa.RamToIdeaApp.Services
{
	public class StartupService : IStartupService
	{
		private readonly IProjectService _projectService;
		private readonly IPluginLogger _logger;

		public StartupService(IProjectService projectService, IPluginLogger logger)
		{
			_logger = logger;
			_projectService = projectService;
		}

		private string SelectRamProject()
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "Ram project | *.rss";
			if (openFileDialog.ShowDialog() != true)
			{
				return string.Empty;
			}

			return openFileDialog.FileName;
		}

		/// <summary>
		/// Start IdeaCheckbot
		/// </summary>
		/// <returns></returns>
		public async Task RunCheckbotAsync()
		{
			if (!_projectService.IsAvailable())
			{
				MessageBox.Show("RAM app is not installed on the computer", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			var selectedRamDbFileName = SelectRamProject();
			if (string.IsNullOrEmpty(selectedRamDbFileName))
			{
				await Task.CompletedTask;
				return;
			}

			var projectDir = Path.GetDirectoryName(selectedRamDbFileName);

			var projectName = Path.GetFileNameWithoutExtension(selectedRamDbFileName);

			string workingDirectory = Path.Combine(projectDir, "IdeaStatiCa-" + projectName);
			if (!Directory.Exists(workingDirectory))
			{
				_logger.LogInformation($"RunCheckbotAsync : Creating directory '{workingDirectory}'");
				Directory.CreateDirectory(workingDirectory);
			}
			else
			{
				_logger.LogInformation($"RunCheckbotAsync : directory '{workingDirectory}' exists");
			}

			var projectInfo = new ProjectInfo
			{
				RamDbFileName = selectedRamDbFileName,
				ProjectWorkingDir = workingDirectory,
			};

			var bimHosting = new GrpcBimHostingFactory();
			RamPluginFactory pluginFactory = new RamPluginFactory(projectInfo, _projectService, _logger, bimHosting.InitGrpcClient(_logger));
			var pluginHosting = bimHosting.Create(pluginFactory, _logger);

			_logger.LogDebug("Starting Checkbot");

			//Run GRPC
			await pluginHosting.RunAsync(Process.GetCurrentProcess().Id.ToString(), workingDirectory);
		}
	}
}
