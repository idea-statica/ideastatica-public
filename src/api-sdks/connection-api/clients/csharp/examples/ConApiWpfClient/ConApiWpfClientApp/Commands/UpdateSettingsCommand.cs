using ConApiWpfClientApp.ViewModels;
using IdeaStatiCa.Api.Common;
using IdeaStatiCa.ConnectionApi;
using IdeaStatiCa.Plugin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ConApiWpfClientApp.Commands
{
	/// <summary>
	/// Command to update the project settings from the output text.
	/// </summary>
	public class UpdateSettingsCommand : AsyncCommandBase
	{
		private readonly CancellationTokenSource _cts;

		/// <summary>
		/// Initializes a new instance of the <see cref="UpdateSettingsCommand"/> class.
		/// </summary>
		/// <param name="viewModel">The view model that owns this command.</param>
		/// <param name="logger">Logger for tracking command execution.</param>
		/// <param name="cts">Cancellation token source for async operations.</param>
		public UpdateSettingsCommand(MainWindowViewModel viewModel, IPluginLogger logger, CancellationTokenSource cts)
			: base(viewModel, logger)
		{
			_cts = cts;
		}

		/// <inheritdoc/>
		public override bool CanExecute(object? parameter) => _viewModel.ProjectInfo != null;

		/// <inheritdoc/>
		protected override async Task ExecuteAsync(object? parameter)
		{
			_logger.LogInformation("UpdateSettingsAsync");

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
				var settingsMap = JsonConvert.DeserializeObject<Dictionary<string, object>>(_viewModel.OutputText!, IdeaJsonSerializerSetting.GetTypeNameSerializerSetting());

				var newSettings = await ConApiClient.Settings.UpdateSettingsAsync(_viewModel.ProjectInfo.ProjectId, settingsMap, 0, _cts.Token);

				var settingsJson = Tools.JsonTools.ToFormatedJson(newSettings);
				_viewModel.OutputText = settingsJson;
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
