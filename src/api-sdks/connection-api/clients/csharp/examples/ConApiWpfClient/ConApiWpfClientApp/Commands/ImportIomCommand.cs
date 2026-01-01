using ConApiWpfClientApp.ViewModels;
using IdeaStatiCa.Plugin;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace ConApiWpfClientApp.Commands
{
	public class ImportIomCommand : AsyncCommandBase
	{
		public ImportIomCommand(MainWindowViewModel viewModel, IPluginLogger logger)
			: base(viewModel, logger)
		{
		}

		public override bool CanExecute(object? parameter) => ConApiClient != null && _viewModel.ProjectInfo == null;

		protected override async Task ExecuteAsync(object? parameter)
		{
			_logger.LogInformation("\t\tprivate async Task ImportIomAsync()\r\n");

			if (ConApiClient == null)
			{
				throw new Exception("IConnectionApiClient is not connected");
			}

			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "Iom files|*.iom;*.xml";
			if (openFileDialog.ShowDialog() != true)
			{
				return;
			}

			_viewModel.IsBusy = true;
			try
			{
				_viewModel.ProjectInfo = await ConApiClient.Project.CreateProjectFromIomFileAsync(openFileDialog.FileName);

				var projectInfoJson = Tools.JsonTools.ToFormatedJson(_viewModel.ProjectInfo);

				_viewModel.OutputText = string.Format("ProjectId = {0}\n\n{1}", ConApiClient.ActiveProjectId, projectInfoJson);

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
