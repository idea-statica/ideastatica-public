using ConApiWpfClientApp.ViewModels;
using IdeaStatiCa.Plugin;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ConApiWpfClientApp.Commands
{
	public class DeleteOperationsCommand : AsyncCommandBase
	{
		private readonly CancellationTokenSource _cts;

		public DeleteOperationsCommand(MainWindowViewModel viewModel, IPluginLogger logger, CancellationTokenSource cts)
			: base(viewModel, logger)
		{
			_cts = cts;
		}

		public override bool CanExecute(object? parameter) => _viewModel.SelectedConnection != null;

		protected override async Task ExecuteAsync(object? parameter)
		{
			_logger.LogInformation("DeleteOperationsAsync");

			if (_viewModel.ProjectInfo == null)
			{
				return;
			}

			if (ConApiClient == null)
			{
				return;
			}

			if (_viewModel.SelectedConnection == null || _viewModel.SelectedConnection.Id < 1)
			{
				return;
			}

			_viewModel.IsBusy = true;
			try
			{
				await ConApiClient.Operation.DeleteOperationsAsync(_viewModel.ProjectInfo.ProjectId,
					_viewModel.SelectedConnection!.Id, 0, _cts.Token);

				_viewModel.OutputText = "Operations were removed";
			}
			catch (Exception ex)
			{
				_logger.LogWarning("DeleteOperationsAsync failed", ex);
				_viewModel.OutputText = ex.Message;
			}
			finally
			{
				_viewModel.IsBusy = false;
				_viewModel.RefreshCommands();
				await _viewModel.ShowClientUIAsync();
			}
		}
	}
}
