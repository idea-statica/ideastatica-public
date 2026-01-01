using ConApiWpfClientApp.ViewModels;
using IdeaStatiCa.Plugin;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ConApiWpfClientApp.Commands
{
	public class GetMembersCommand : AsyncCommandBase
	{
		private readonly CancellationTokenSource _cts;

		public GetMembersCommand(MainWindowViewModel viewModel, IPluginLogger logger, CancellationTokenSource cts)
			: base(viewModel, logger)
		{
			_cts = cts;
		}

		public override bool CanExecute(object? parameter) => _viewModel.SelectedConnection != null;

		protected override async Task ExecuteAsync(object? parameter)
		{
			_logger.LogInformation("GetMembersAsync");

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
				var members = await ConApiClient.Member.GetMembersAsync(_viewModel.ProjectInfo.ProjectId,
					_viewModel.SelectedConnection!.Id, 0, _cts.Token);

				if (members == null)
				{
					_viewModel.OutputText = "No members";
				}
				else
				{
					_viewModel.OutputText = ConApiWpfClientApp.Tools.JsonTools.ToFormatedJson(members);
				}
			}
			catch (Exception ex)
			{
				_logger.LogWarning("GetMembersAsync failed", ex);
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
