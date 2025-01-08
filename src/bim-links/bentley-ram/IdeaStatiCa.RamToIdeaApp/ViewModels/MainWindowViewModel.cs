using CommunityToolkit.Mvvm.Input;
using IdeaStatiCa.Plugin;
using IdeaStatiCa.RamToIdeaApp.Models;
using IdeaStatiCa.RamToIdeaApp.Services;
using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace IdeaStatiCa.RamToIdeaApp.ViewModels
{
	public class MainWindowViewModel : ViewModelBase
	{
		private readonly IProjectService _projectService;
		private readonly IPluginLogger _logger;
		//private readonly IBIMPluginFactory _bimPluginFactory;

		public MainWindowViewModel(IProjectService projectService, IPluginLogger logger)
		{
			this._logger = logger;
			this._projectService = projectService;
			RunCheckBotCommand = new AsyncRelayCommand(RunCheckbotAsync, CanRunCheckbotAsync);

			//_bimPluginFactory = new RamPluginFactory(this.ProjectInfo, _logger);
		}

		public IAsyncRelayCommand RunCheckBotCommand { get; }

		public IProjectInfo ProjectInfo { get; set; } = new ProjectInfo();

		private bool CanRunCheckbotAsync()
		{
			return true;
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

		private async Task RunCheckbotAsync()
		{
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

			this.ProjectInfo.RamDbFileName = selectedRamDbFileName;
			this.ProjectInfo.ProjectWorkingDir = workingDirectory;

			var bimHosting = new GrpcBimHostingFactory();
			RamPluginFactory pluginFactory = new RamPluginFactory(this.ProjectInfo, _projectService, _logger, bimHosting.InitGrpcClient(_logger));
			IBIMPluginHosting pluginHosting = bimHosting.Create(pluginFactory, _logger);

			_logger.LogDebug("Starting Checkbot");

			//Run GRPC
			await pluginHosting.RunAsync(Process.GetCurrentProcess().Id.ToString(), workingDirectory);
		}

		private string status;
		public string Status
		{
			get => status;
			set => SetProperty(ref status, value);
		}
	}
}
