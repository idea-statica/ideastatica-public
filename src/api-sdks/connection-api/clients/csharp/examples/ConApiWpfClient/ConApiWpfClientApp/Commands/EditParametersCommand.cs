using ConApiWpfClientApp.Services;
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
	/// Command to edit parameters for the selected connection.
	/// </summary>
	public class EditParametersCommand : AsyncCommandBase
	{
		private readonly CancellationTokenSource _cts;

		/// <summary>
		/// Initializes a new instance of the <see cref="EditParametersCommand"/> class.
		/// </summary>
		/// <param name="viewModel">The view model that owns this command.</param>
		/// <param name="logger">Logger for tracking command execution.</param>
		/// <param name="cts">Cancellation token source for async operations.</param>
		public EditParametersCommand(MainWindowViewModel viewModel, IPluginLogger logger, CancellationTokenSource cts)
			: base(viewModel, logger)
		{
			_cts = cts;
		}

		/// <inheritdoc/>
		public override bool CanExecute(object? parameter) => _viewModel.SelectedConnection != null;

		/// <inheritdoc/>
		protected override async Task ExecuteAsync(object? parameter)
		{
			_logger.LogInformation("EditParametersAsync");

			if (_viewModel.ProjectInfo == null)
			{
				return;
			}

			if (ConApiClient == null)
			{
				return;
			}

			if (_viewModel == null || _viewModel.SelectedConnection == null || _viewModel.SelectedConnection.Id < 1)
			{
				return;
			}

			_viewModel.IsBusy = true;
			try
			{
				_logger.LogInformation("Editing parameters for connection");

				// get existing parameters from the selected connection
				var existingParameters = await _viewModel.ConApiClient!.Parameter.GetParametersAsync(_viewModel.ProjectInfo.ProjectId, _viewModel.SelectedConnection.Id, null, 0, _cts.Token);

				if(existingParameters == null || existingParameters.Any() == false)
				{
					return;
				}

				var jsonEditorService = new JsonEditorService<List<IdeaParameter>>();

				var updatedParams = await jsonEditorService.EditAsync(existingParameters);
				if (updatedParams == null)
				{
					return;
				}

				List<IdeaParameterUpdate> parameterUpdate = updatedParams.Select(p => {
					var r= new IdeaParameterUpdate();
					r.Key = p.Key;
					r.Expression = p.Expression;
					return r;
					}).ToList();


				var updateResponse = await _viewModel.ConApiClient.Parameter.UpdateAsync(_viewModel.ProjectInfo.ProjectId, _viewModel.SelectedConnection.Id, parameterUpdate, 0, _cts.Token);
			}
			catch (Exception ex)
			{
				_logger.LogWarning("EditParametersAsync failed", ex);
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
