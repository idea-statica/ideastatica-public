using ConApiWpfClientApp.ViewModels;
using IdeaStatiCa.Plugin;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ConApiWpfClientApp.Commands
{
	/// <summary>
	/// Command to delete all fabrication operations from the selected connection.
	/// </summary>
	public class DeleteOperationsCommand : AsyncCommandBase
	{
		private readonly CancellationTokenSource _cts;

		/// <summary>
		/// Initializes a new instance of the <see cref="DeleteOperationsCommand"/> class.
		/// </summary>
		/// <param name="viewModel">The view model that owns this command.</param>
		/// <param name="logger">Logger for tracking command execution.</param>
		/// <param name="cts">Cancellation token source for async operations.</param>
		public DeleteOperationsCommand(MainWindowViewModel viewModel, IPluginLogger logger, CancellationTokenSource cts)
			: base(viewModel, logger)
		{
			_cts = cts;
		}

		/// <inheritdoc/>
		public override bool CanExecute(object? parameter) => _viewModel.SelectedConnection != null;

		/// <inheritdoc/>
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
