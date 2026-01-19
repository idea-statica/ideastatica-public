using ConApiWpfClientApp.Models;
using ConApiWpfClientApp.Services;
using ConApiWpfClientApp.ViewModels;
using IdeaStatiCa.Api.Connection.Model;
using IdeaStatiCa.ConRestApiClientUI;
using IdeaStatiCa.Plugin;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ConApiWpfClientApp.Commands
{
	/// <summary>
	/// Command to apply a connection template to the selected connection, either from a file or the connection library.
	/// </summary>
	public class ApplyTemplateCommand : AsyncCommandBase
	{
		private readonly CancellationTokenSource _cts;
		private readonly IdeaStatiCa.ConRestApiClientUI.ISceneController _sceneController;

		/// <summary>
		/// Initializes a new instance of the <see cref="ApplyTemplateCommand"/> class.
		/// </summary>
		/// <param name="viewModel">The view model that owns this command.</param>
		/// <param name="logger">Logger for tracking command execution.</param>
		/// <param name="cts">Cancellation token source for async operations.</param>
		/// <param name="sceneController">Controller for rendering 3D scenes.</param>
		public ApplyTemplateCommand(MainWindowViewModel viewModel, IPluginLogger logger, CancellationTokenSource cts, ISceneController sceneController)
			: base(viewModel, logger)
		{
			_cts = cts;
			_sceneController = sceneController;
		}

		/// <inheritdoc/>
		public override bool CanExecute(object? parameter) => _viewModel.SelectedConnection != null;

		/// <inheritdoc/>
		protected override async Task ExecuteAsync(object? parameter)
		{
			_logger.LogInformation("ApplyTemplateAsync");

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
				ConnectionLibraryModel? templateRes = null;
				if (parameter?.ToString()?.Equals("ConnectionLibrary", StringComparison.InvariantCultureIgnoreCase) == true)
				{
					var proposeService = new ConnectionLibraryProposer(ConApiClient, _logger);

					templateRes = await proposeService.GetTemplateAsync(_viewModel.ProjectInfo.ProjectId, _viewModel.SelectedConnection.Id, _cts.Token);
				}
				else
				{
					ITemplateProvider templateProvider = new TemplateProviderFile();
					templateRes = await templateProvider.GetTemplateAsync(_viewModel.ProjectInfo.ProjectId, _viewModel.SelectedConnection.Id, _cts.Token);
				}

				if (templateRes == null || string.IsNullOrEmpty(templateRes.SelectedTemplateXml) == true)
				{
					_logger.LogInformation("ApplyTemplateAsync : no template, leaving");
					return;
				}

				var getTemplateParam = new ConTemplateMappingGetParam()
				{
					Template = templateRes.SelectedTemplateXml,
				};

				if (templateRes?.SearchParameters?.Members?.Any() == true)
				{
					getTemplateParam.MemberIds = templateRes.SearchParameters.Members;
				}

				var templateMapping = await ConApiClient.Template.GetDefaultTemplateMappingAsync(_viewModel.ProjectInfo.ProjectId,
					_viewModel.SelectedConnection.Id,
					getTemplateParam,
					0, _cts.Token);

				if (templateMapping == null)
				{
					throw new ArgumentException($"Invalid mapping for connection '{_viewModel.SelectedConnection.Name}'");
				}

				var mappingSetter = new Services.TemplateMappingSetter();
				var modifiedTemplateMapping = await mappingSetter.SetAsync(templateMapping);
				if (modifiedTemplateMapping == null)
				{
					// operation was canceled
					return;
				}

				var applyTemplateParam = new ConTemplateApplyParam()
				{
					ConnectionTemplate = templateRes!.SelectedTemplateXml,
					Mapping = modifiedTemplateMapping,

				};

				var applyTemplateResult = await ConApiClient.Template.ApplyTemplateAsync(_viewModel.ProjectInfo.ProjectId,
					_viewModel.SelectedConnection!.Id,
					applyTemplateParam,
					0, _cts.Token);

				_viewModel.OutputText = "Template was applied";
			}
			catch (Exception ex)
			{
				_logger.LogWarning("ApplyTemplateAsync failed", ex);
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
