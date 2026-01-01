using ConApiWpfClientApp.Services;
using ConApiWpfClientApp.ViewModels;
using IdeaStatiCa.Api.Connection.Model;
using IdeaStatiCa.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ConApiWpfClientApp.Commands
{
	public class UpdateConnectionLoadingCommand : AsyncCommandBase
	{
		private readonly CancellationTokenSource _cts;

		public UpdateConnectionLoadingCommand(MainWindowViewModel viewModel, IPluginLogger logger, CancellationTokenSource cts)
			: base(viewModel, logger)
		{
			_cts = cts;
		}

		public override bool CanExecute(object? parameter) => _viewModel.SelectedConnection != null;

		protected override async Task ExecuteAsync(object? parameter)
		{
			_logger.LogInformation("UpdateConnectionLoadingCommand");

			if (_viewModel.ProjectInfo == null)
			{
				return;
			}

			if (ConApiClient == null)
			{
				return;
			}

			if (_viewModel.SelectedConnection == null)
			{
				return;
			}

			_viewModel.IsBusy = true;
			try
			{
				var connectionLoadingData = await ConApiClient.LoadEffect.GetLoadEffectsAsync(_viewModel.ProjectInfo.ProjectId, _viewModel.SelectedConnection.Id, false, 0, _cts.Token);

				if (connectionLoadingData == null || connectionLoadingData.Any() == false)
				{
					_logger.LogInformation("UpdateConnectionLoadingCommand : no loading for connection");
					return;
				}

				var jsonEditorService = new JsonEditorService<List<ConLoadEffect>>();
				var editedLoadEffects = await jsonEditorService.EditAsync(connectionLoadingData);

				if (editedLoadEffects == null || editedLoadEffects.Any() == false)
				{
					return;
				}

				foreach (var loadEffect in editedLoadEffects)
				{
					var updateRes = await ConApiClient.LoadEffect.UpdateLoadEffectAsync(_viewModel.ProjectInfo.ProjectId, _viewModel.SelectedConnection.Id, loadEffect, 0, _cts.Token);
				}

				_viewModel.OutputText = "Loading was updated";
			}
			catch (Exception ex)
			{
				_logger.LogWarning("UpdateConnectionLoadingCommand failed", ex);
				_viewModel.OutputText = ex.Message;
			}
			finally
			{
				_viewModel.IsBusy = false;
				_viewModel.RefreshCommands();
			}
		}
	}
}
