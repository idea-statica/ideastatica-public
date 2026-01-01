using ConApiWpfClientApp.ViewModels;
using IdeaStatiCa.Plugin;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

namespace ConApiWpfClientApp.Commands
{
	public class CloseProjectCommand : AsyncCommandBase
	{
		private readonly CancellationTokenSource _cts;

		public CloseProjectCommand(MainWindowViewModel viewModel, IPluginLogger logger, CancellationTokenSource cts)
			: base(viewModel, logger)
		{
			_cts = cts;
		}

		public override bool CanExecute(object? parameter) => _viewModel.ProjectInfo != null;

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
