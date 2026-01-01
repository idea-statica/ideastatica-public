using ConApiWpfClientApp.ViewModels;
using IdeaStatiCa.Plugin;
using Microsoft.Win32;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ConApiWpfClientApp.Commands
{
	public class DownloadProjectCommand : AsyncCommandBase
	{
		private readonly CancellationTokenSource _cts;

		public DownloadProjectCommand(MainWindowViewModel viewModel, IPluginLogger logger, CancellationTokenSource cts)
			: base(viewModel, logger)
		{
			_cts = cts;
		}

		public override bool CanExecute(object? parameter) => _viewModel.ProjectInfo != null;

		protected override async Task ExecuteAsync(object? parameter)
		{
			_logger.LogInformation("DownloadProjectAsync");

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
				SaveFileDialog saveFileDialog = new SaveFileDialog();
				saveFileDialog.Filter = "IdeaConnection | *.ideacon";
				if (saveFileDialog.ShowDialog() == true)
				{
					await ConApiClient.Project.SaveProjectAsync(_viewModel.ProjectInfo.ProjectId, saveFileDialog.FileName, _cts.Token);
				}
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
