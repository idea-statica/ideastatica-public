using ConApiWpfClientApp.ViewModels;
using IdeaStatiCa.Plugin;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ConApiWpfClientApp.Commands
{
	public class GetSettingsCommand : AsyncCommandBase
	{
		private readonly CancellationTokenSource _cts;

		public GetSettingsCommand(MainWindowViewModel viewModel, IPluginLogger logger, CancellationTokenSource cts)
			: base(viewModel, logger)
		{
			_cts = cts;
		}

		public override bool CanExecute(object? parameter) => _viewModel.ProjectInfo != null;

		protected override async Task ExecuteAsync(object? parameter)
		{
			_logger.LogInformation("GetSettingsAsync");

			if (_viewModel.ProjectInfo == null)
			{
				return;
			}

			if (ConApiClient == null)
			{
				return;
			}

			_viewModel.IsBusy = true;
			try
			{
				var settings = await ConApiClient.Settings.GetSettingsAsync(_viewModel.ProjectInfo.ProjectId, null, 0, _cts.Token);
				var settingsJson = Tools.JsonTools.ToFormatedJson(settings);
				_viewModel.OutputText = settingsJson;
			}
			catch (Exception ex)
			{
				_logger.LogWarning("SaveProjectAsync failed", ex);
				_viewModel.OutputText = ex.Message;
			}
			finally
			{
				_viewModel.IsBusy = false;
				_viewModel.RefreshCommands();
			}

			await Task.CompletedTask;
		}
	}
}
