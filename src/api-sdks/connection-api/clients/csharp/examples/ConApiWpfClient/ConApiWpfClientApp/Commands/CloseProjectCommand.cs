using ConApiWpfClientApp.ViewModels;
using IdeaStatiCa.Plugin;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

namespace ConApiWpfClientApp.Commands
{
	/// <summary>
	/// Command to close the currently open project.
	/// </summary>
	public class CloseProjectCommand : AsyncCommandBase
	{
		private readonly CancellationTokenSource _cts;

		/// <summary>
		/// Initializes a new instance of the <see cref="CloseProjectCommand"/> class.
		/// </summary>
		/// <param name="viewModel">The view model that owns this command.</param>
		/// <param name="logger">Logger for tracking command execution.</param>
		/// <param name="cts">Cancellation token source for async operations.</param>
		public CloseProjectCommand(MainWindowViewModel viewModel, IPluginLogger logger, CancellationTokenSource cts)
			: base(viewModel, logger)
		{
			_cts = cts;
		}

		/// <inheritdoc/>
		public override bool CanExecute(object? parameter) => _viewModel.ProjectInfo != null;

		/// <inheritdoc/>
		protected override async Task ExecuteAsync(object? parameter)
		{
			_logger.LogInformation("CloseProjectAsync");

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
				await ConApiClient.Project.CloseProjectAsync(_viewModel.ProjectInfo.ProjectId, 0, _cts.Token);
				_viewModel.ProjectInfo = null;
				_viewModel.SelectedConnection = null;
				_viewModel.Connections = new ObservableCollection<ConnectionViewModel>();
				_viewModel.OutputText = string.Empty;
			}
			catch (Exception ex)
			{
				_logger.LogWarning("CloseProjectAsync failed", ex);
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
