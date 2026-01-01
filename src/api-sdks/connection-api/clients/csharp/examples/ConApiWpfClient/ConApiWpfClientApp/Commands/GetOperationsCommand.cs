using ConApiWpfClientApp.ViewModels;
using IdeaStatiCa.Plugin;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ConApiWpfClientApp.Commands
{
	public class GetOperationsCommand : AsyncCommandBase
	{
		private readonly CancellationTokenSource _cts;

		public GetOperationsCommand(MainWindowViewModel viewModel, IPluginLogger logger, CancellationTokenSource cts)
			: base(viewModel, logger)
		{
			_cts = cts;
		}

		public override bool CanExecute(object? parameter) => _viewModel.SelectedConnection != null;

		protected override async Task ExecuteAsync(object? parameter)
		{
			_logger.LogInformation("GetOperationsAsync");

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
				var operations = await ConApiClient.Operation.GetOperationsAsync(_viewModel.ProjectInfo.ProjectId,
					_viewModel.SelectedConnection!.Id, 0, _cts.Token);

				if (operations == null)
				{
					_viewModel.OutputText = "No operations";
				}
				else
				{
					_viewModel.OutputText = ConApiWpfClientApp.Tools.JsonTools.ToFormatedJson(operations);
				}
			}
			catch (Exception ex)
			{
				_logger.LogWarning("GetOperationsAsync failed", ex);
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
