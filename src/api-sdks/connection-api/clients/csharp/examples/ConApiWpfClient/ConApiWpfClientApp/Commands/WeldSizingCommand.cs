using ConApiWpfClientApp.ViewModels;
using IdeaStatiCa.Plugin;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ConApiWpfClientApp.Commands
{
	/// <summary>
	/// Command to perform automatic weld sizing for the selected connection.
	/// </summary>
	public class WeldSizingCommand : AsyncCommandBase
	{
		private readonly CancellationTokenSource _cts;

		/// <summary>
		/// Initializes a new instance of the <see cref="WeldSizingCommand"/> class.
		/// </summary>
		/// <param name="viewModel">The view model that owns this command.</param>
		/// <param name="logger">Logger for tracking command execution.</param>
		/// <param name="cts">Cancellation token source for async operations.</param>
		public WeldSizingCommand(MainWindowViewModel viewModel, IPluginLogger logger, CancellationTokenSource cts)
			: base(viewModel, logger)
		{
			_cts = cts;
		}

		/// <inheritdoc/>
		public override bool CanExecute(object? parameter) => _viewModel.SelectedConnection != null;

		/// <inheritdoc/>
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
