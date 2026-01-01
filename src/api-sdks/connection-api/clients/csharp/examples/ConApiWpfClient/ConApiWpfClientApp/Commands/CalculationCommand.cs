using ConApiWpfClientApp.ViewModels;
using IdeaStatiCa.Api.Connection.Model;
using IdeaStatiCa.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ConApiWpfClientApp.Commands
{
	/// <summary>
	/// Command to perform structural calculation on the selected connection.
	/// </summary>
	public class CalculationCommand : AsyncCommandBase
	{
		private readonly CancellationTokenSource _cts;

		/// <summary>
		/// Initializes a new instance of the <see cref="CalculationCommand"/> class.
		/// </summary>
		/// <param name="viewModel">The view model that owns this command.</param>
		/// <param name="logger">Logger for tracking command execution.</param>
		/// <param name="cts">Cancellation token source for async operations.</param>
		public CalculationCommand(MainWindowViewModel viewModel, IPluginLogger logger, CancellationTokenSource cts)
			: base(viewModel, logger)
		{
			_cts = cts;
		}

		/// <inheritdoc/>
		public override bool CanExecute(object? parameter) => _viewModel.SelectedConnection != null;

		/// <inheritdoc/>
		protected override async Task ExecuteAsync(object? parameter)
		{
			_logger.LogInformation("CalculateAsync");

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
				var connectionIdList = new List<int>();
				connectionIdList.Add(_viewModel.SelectedConnection!.Id);

				ConCalculationParameter calculationParameter = new ConCalculationParameter()
				{
					AnalysisType = _viewModel.SelectedAnalysisType,
					ConnectionIds = connectionIdList
				};

				var selectedConData = await ConApiClient.Connection.GetConnectionAsync(_viewModel.ProjectInfo.ProjectId, _viewModel.SelectedConnection!.Id, 0, _cts.Token);
				if (selectedConData.AnalysisType != _viewModel.SelectedAnalysisType ||
					selectedConData.IncludeBuckling != _viewModel.CalculateBuckling)
				{
					selectedConData.AnalysisType = _viewModel.SelectedAnalysisType;
					selectedConData.IncludeBuckling = _viewModel.CalculateBuckling;
					await ConApiClient.Connection.UpdateConnectionAsync(_viewModel.ProjectInfo.ProjectId, _viewModel.SelectedConnection!.Id, selectedConData, 0, _cts.Token);
				}

				var calculationResults = await ConApiClient.Calculation.CalculateAsync(_viewModel.ProjectInfo.ProjectId, connectionIdList, 0, _cts.Token);

				string rawResultsXml = string.Empty;

				if (_viewModel.GetRawXmlResults)
				{
					var rawResults = await ConApiClient.Calculation.GetRawJsonResultsAsync(_viewModel.ProjectInfo.ProjectId, connectionIdList, 0, _cts.Token);
					rawResultsXml = rawResults!.Any() ? rawResults[0] : string.Empty;
				}

				_viewModel.OutputText = $"{ConApiWpfClientApp.Tools.JsonTools.ToFormatedJson(calculationResults)}\n\n{rawResultsXml}";
			}
			catch (Exception ex)
			{
				_logger.LogWarning("GetRawResultsAsync failed", ex);
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
