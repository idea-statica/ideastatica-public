using ConApiWpfClientApp.ViewModels;
using IdeaStatiCa.Plugin;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ConApiWpfClientApp.Commands
{
	public class WeldSizingCommand : AsyncCommandBase
	{
		private readonly CancellationTokenSource _cts;

		public WeldSizingCommand(MainWindowViewModel viewModel, IPluginLogger logger, CancellationTokenSource cts)
			: base(viewModel, logger)
		{
			_cts = cts;
		}

		public override bool CanExecute(object? parameter) => _viewModel.SelectedConnection != null;

		protected override async Task ExecuteAsync(object? parameter)
		{
			_logger.LogInformation("DoWeldSizingAsync");

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
				var res = await ConApiClient!.Operation!.PreDesignWeldsAsync(_viewModel.ProjectInfo.ProjectId, _viewModel.SelectedConnection!.Id, IdeaStatiCa.Api.Connection.Model.Connection.ConWeldSizingMethodEnum.FullStrength);

				_viewModel.OutputText = res;
			}
			catch (Exception ex)
			{
				_logger.LogWarning("DoWeldSizingAsync failed", ex);
				_viewModel.OutputText = ex.Message;
			}
			finally
			{
				_viewModel.IsBusy = false;
				_viewModel.RefreshCommands();

				await _viewModel.ShowClientUIAsync();
			}

			await Task.CompletedTask;
		}
	}
}
