using ConApiWpfClientApp.Services;
using ConApiWpfClientApp.ViewModels;
using IdeaStatiCa.Plugin;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ConApiWpfClientApp.Commands
{
	/// <summary>
	/// Command to evaluate an expression in the selected connection.
	/// </summary>
	public class EvaluateExpressionCommand : AsyncCommandBase
	{
		private readonly CancellationTokenSource _cts;

		/// <summary>
		/// Initializes a new instance of the <see cref="EvaluateExpressionCommand"/> class.
		/// </summary>
		/// <param name="viewModel">The view model that owns this command.</param>
		/// <param name="logger">Logger for tracking command execution.</param>
		/// <param name="cts">Cancellation token source for async operations.</param>
		public EvaluateExpressionCommand(MainWindowViewModel viewModel, IPluginLogger logger, CancellationTokenSource cts)
			: base(viewModel, logger)
		{
			_cts = cts;
		}

		/// <inheritdoc/>
		public override bool CanExecute(object? parameter) => _viewModel.SelectedConnection != null;

		/// <inheritdoc/>
		protected override async Task ExecuteAsync(object? parameter)
		{
			_logger.LogInformation("EvaluateExpressionAsync");

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
				var expressionProvider = new ExpressionProvider(ConApiClient, _logger);
				var expressionModel = await expressionProvider.GetExpressionAsync(_viewModel.ProjectInfo.ProjectId,
					_viewModel.SelectedConnection.Id, _cts.Token);

				if (expressionModel == null || string.IsNullOrEmpty(expressionModel.Expression))
				{
					_logger.LogInformation("EvaluateExpressionCommand.ExecuteAsync - leaving. No Expression was provided");
					return;
				}

				_logger.LogInformation($"Evaluating expression: {expressionModel.Expression}");

				string expressionText = $"\"{expressionModel.Expression}\"";

				var result = await ConApiClient.Parameter.EvaluateExpressionAsync(
					_viewModel.ProjectInfo.ProjectId,
					_viewModel.SelectedConnection.Id,
					expressionText,
					0,
					_cts.Token);

				_viewModel.OutputText = result;
			}
			catch (Exception ex)
			{
				_logger.LogWarning("EvaluateExpressionAsync failed", ex);
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
