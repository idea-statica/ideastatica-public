using ConApiWpfClientApp.ViewModels;
using IdeaStatiCa.Plugin;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace ConApiWpfClientApp.Commands
{
	/// <summary>
	/// Command to open an existing IdeaConnection project file (.ideacon).
	/// </summary>
	public class OpenProjectCommand : AsyncCommandBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="OpenProjectCommand"/> class.
		/// </summary>
		/// <param name="viewModel">The view model that owns this command.</param>
		/// <param name="logger">Logger for tracking command execution.</param>
		public OpenProjectCommand(MainWindowViewModel viewModel, IPluginLogger logger)
			: base(viewModel, logger)
		{
		}

		/// <inheritdoc/>
		public override bool CanExecute(object? parameter) => ConApiClient != null && _viewModel.ProjectInfo == null;

		/// <inheritdoc/>
		protected override async Task ExecuteAsync(object? parameter)
		{
			_logger.LogInformation("OpenProjectAsync");

			if (ConApiClient == null)
			{
				throw new Exception("IConnectionApiClient is not connected");
			}

			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "IdeaConnection | *.ideacon";
			if (openFileDialog.ShowDialog() != true)
			{
				return;
			}

			_viewModel.IsBusy = true;
			try
			{
				_viewModel.ProjectInfo = await ConApiClient.Project.OpenProjectAsync(openFileDialog.FileName);

				var projectInfoJson = Tools.JsonTools.ToFormatedJson(_viewModel.ProjectInfo);

				_viewModel.OutputText = string.Format("ClientId = {0}, ProjectId = {1}\n\n{2}", ConApiClient.ClientId, ConApiClient.ActiveProjectId, projectInfoJson);

				_viewModel.Connections = new ObservableCollection<ConnectionViewModel>(_viewModel.ProjectInfo.Connections.Select(c => new ConnectionViewModel(c)));
			}
			catch (Exception ex)
			{
				_logger.LogWarning("OpenProjectAsync", ex);
				_viewModel.OutputText = ex.Message;
			}
			finally
			{
				_viewModel.IsBusy = false;
				_viewModel.RefreshCommands();

				if (_viewModel.Connections?.Any() == true)
				{
					_viewModel.SelectedConnection = _viewModel.Connections.First();
				}
				else
				{
					_viewModel.SelectedConnection = null;
				}
			}

			await Task.CompletedTask;
		}
	}
}
