using ConApiWpfClientApp.ViewModels;
using IdeaStatiCa.Plugin;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ConApiWpfClientApp.Commands
{
	/// <summary>
	/// Command to retrieve the topology information of the selected connection.
	/// </summary>
	public class GetTopologyCommand : AsyncCommandBase
	{
		private readonly CancellationTokenSource _cts;

		/// <summary>
		/// Initializes a new instance of the <see cref="GetTopologyCommand"/> class.
		/// </summary>
		/// <param name="viewModel">The view model that owns this command.</param>
		/// <param name="logger">Logger for tracking command execution.</param>
		/// <param name="cts">Cancellation token source for async operations.</param>
		public GetTopologyCommand(MainWindowViewModel viewModel, IPluginLogger logger, CancellationTokenSource cts)
			: base(viewModel, logger)
		{
			_cts = cts;
		}

		/// <inheritdoc/>
		public override bool CanExecute(object? parameter) => _viewModel.SelectedConnection != null;

		/// <inheritdoc/>
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
