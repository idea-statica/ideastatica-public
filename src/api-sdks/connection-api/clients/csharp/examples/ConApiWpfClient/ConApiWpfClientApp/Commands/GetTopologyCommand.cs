using ConApiWpfClientApp.ViewModels;
using IdeaStatiCa.Plugin;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ConApiWpfClientApp.Commands
{
	public class GetTopologyCommand : AsyncCommandBase
	{
		private readonly CancellationTokenSource _cts;

		public GetTopologyCommand(MainWindowViewModel viewModel, IPluginLogger logger, CancellationTokenSource cts)
			: base(viewModel, logger)
		{
			_cts = cts;
		}

		public override bool CanExecute(object? parameter) => _viewModel.SelectedConnection != null;

		protected override async Task ExecuteAsync(object? parameter)
		{
			_logger.LogInformation("CreateTemplateAsync");

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

			var topologyJsonString = string.Empty;

			_viewModel.IsBusy = true;
			try
			{
				topologyJsonString = await ConApiClient.Connection.GetConnectionTopologyAsync(_viewModel.ProjectInfo.ProjectId, _viewModel.SelectedConnection.Id, 0, _cts.Token);

				if (string.IsNullOrEmpty(topologyJsonString))
				{
					_viewModel.OutputText = topologyJsonString;
				}
				else
				{
					dynamic? typology = JsonConvert.DeserializeObject(topologyJsonString!);

					if (typology != null)
					{
						var topologyCode = typology["typologyCode_V2"];

						_viewModel.OutputText = $"typologyCode_V2 = '{topologyCode}'\n\nConnection topology :\n{topologyJsonString}";
					}
					else
					{
						_viewModel.OutputText = "Error";
					}
				}
			}
			catch (Exception ex)
			{
				_logger.LogWarning("CreateConTemplateAsync failed", ex);
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
