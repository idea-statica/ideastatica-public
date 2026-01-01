using ConApiWpfClientApp.ViewModels;
using IdeaStatiCa.Plugin;
using Microsoft.Win32;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ConApiWpfClientApp.Commands
{
	/// <summary>
	/// Command to create a connection template from the selected connection and save it to a file.
	/// </summary>
	public class CreateTemplateCommand : AsyncCommandBase
	{
		private readonly CancellationTokenSource _cts;

		/// <summary>
		/// Initializes a new instance of the <see cref="CreateTemplateCommand"/> class.
		/// </summary>
		/// <param name="viewModel">The view model that owns this command.</param>
		/// <param name="logger">Logger for tracking command execution.</param>
		/// <param name="cts">Cancellation token source for async operations.</param>
		public CreateTemplateCommand(MainWindowViewModel viewModel, IPluginLogger logger, CancellationTokenSource cts)
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

			string conTempXmlString = string.Empty;

			_viewModel.IsBusy = true;
			try
			{
				conTempXmlString = await ConApiClient.Template.CreateConTemplateAsync(_viewModel.ProjectInfo.ProjectId, _viewModel.SelectedConnection.Id, 0, _cts.Token);
				_viewModel.OutputText = conTempXmlString;

				if (!string.IsNullOrEmpty(conTempXmlString))
				{
					SaveFileDialog saveTemplateFileDialog = new SaveFileDialog();
					saveTemplateFileDialog.Filter = "Connection template | *.contemp";
					saveTemplateFileDialog.OverwritePrompt = true;
					if (saveTemplateFileDialog.ShowDialog() == true)
					{
						await File.WriteAllTextAsync(saveTemplateFileDialog.FileName, conTempXmlString, System.Text.Encoding.Unicode);
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
