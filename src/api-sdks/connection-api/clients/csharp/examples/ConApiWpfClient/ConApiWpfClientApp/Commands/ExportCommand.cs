using ConApiWpfClientApp.ViewModels;
using IdeaStatiCa.Plugin;
using Microsoft.Win32;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ConApiWpfClientApp.Commands
{
	public class ExportCommand : AsyncCommandBase
	{
		public ExportCommand(MainWindowViewModel viewModel, IPluginLogger logger)
			: base(viewModel, logger)
		{
		}

		public override bool CanExecute(object? parameter) => _viewModel.SelectedConnection != null;

		protected override async Task ExecuteAsync(object? parameter)
		{
			_logger.LogInformation("ExportConnectionAsync");

			if (_viewModel.ProjectInfo == null)
			{
				return;
			}

			if (parameter == null)
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

			var format = parameter.ToString();

			_viewModel.IsBusy = true;
			try
			{
				SaveFileDialog saveFileDialog = new SaveFileDialog();
				saveFileDialog.Filter = $"{format} file| *.{format}";
				if (saveFileDialog.ShowDialog() != true)
				{
					return;
				}

				if (format!.Equals("iom"))
				{
					var iomContainerXml = await ConApiClient.Export.ExportIomAsync(_viewModel.ProjectInfo.ProjectId, _viewModel.SelectedConnection.Id);
					await File.WriteAllTextAsync(saveFileDialog.FileName, iomContainerXml);
					_viewModel.OutputText = iomContainerXml;
				}
				else if (format.Equals("ifc"))
				{
					await ConApiClient.Export.ExportIfcFileAsync(_viewModel.ProjectInfo.ProjectId, _viewModel.SelectedConnection.Id, saveFileDialog.FileName);
					var ifc = await File.ReadAllTextAsync(saveFileDialog.FileName);
					_viewModel.OutputText = ifc;
				}
				else
				{
					throw new Exception($"Unsupported format {format}");
				}
			}
			catch (Exception ex)
			{
				_logger.LogWarning("ExportConnectionAsync failed", ex);
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
