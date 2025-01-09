using CommunityToolkit.Mvvm.Input;
using IdeaStatiCa.Plugin;
using IdeaStatiCa.RamToIdeaApp.Models;
using IdeaStatiCa.RamToIdeaApp.Services;
using Microsoft.Extensions.Configuration;
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
		private readonly IConfiguration _configuration;
		private readonly bool _isRamApp;
		IBIMPluginHosting PluginHosting { get; set; }

		public MainWindowViewModel(IProjectService projectService, IConfiguration configuration, IPluginLogger logger)
		{
			this._configuration = configuration;
			this._logger = logger;
			this._projectService = projectService;
			this._isRamApp = projectService.IsAvailable();

			if (!_isRamApp)
			{
				Status = "RAM is not installed on the computer";
			}
			else
			{
				Status = "RAM is installed on the computer";
			}

			RunCheckBotCommand = new AsyncRelayCommand(RunCheckbotAsync, CanRunCheckbotAsync);
		}

		public IAsyncRelayCommand RunCheckBotCommand { get; }

		public IProjectInfo ProjectInfo { get; set; } = new ProjectInfo();

		private bool CanRunCheckbotAsync()
		{
			return _isRamApp && PluginHosting != null;
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
		private async Task RunCheckbotAsync()
		{
			var selectedRamDbFileName = SelectRamProject();
			if (string.IsNullOrEmpty(selectedRamDbFileName))
			{
				await Task.CompletedTask;
				RefreshCommands();
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
				Status = string.Empty;
			}

			this.ProjectInfo.RamDbFileName = selectedRamDbFileName;
			this.ProjectInfo.ProjectWorkingDir = workingDirectory;

			var bimHosting = new GrpcBimHostingFactory();
			RamPluginFactory pluginFactory = new RamPluginFactory(this.ProjectInfo, _projectService, _logger, bimHosting.InitGrpcClient(_logger));
			PluginHosting = bimHosting.Create(pluginFactory, _logger);

			_logger.LogDebug("Starting Checkbot");

			//Run GRPC
			await PluginHosting.RunAsync(Process.GetCurrentProcess().Id.ToString(), workingDirectory);
			Status = selectedRamDbFileName;

			RefreshCommands();
		}

		private string status;
		public string Status
		{
			get => status;
			set => SetProperty(ref status, value);
		}

		private void RefreshCommands()
		{
			this.RunCheckBotCommand.NotifyCanExecuteChanged();
		}
	}
}
