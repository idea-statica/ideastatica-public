using ConApiWpfClientApp.ViewModels;
using IdeaStatiCa.Plugin;
using Microsoft.Win32;
using System;
using System.Threading.Tasks;

namespace ConApiWpfClientApp.Commands
{
	public class GenerateReportCommand : AsyncCommandBase
	{
		public GenerateReportCommand(MainWindowViewModel viewModel, IPluginLogger logger)
			: base(viewModel, logger)
		{
		}

		public override bool CanExecute(object? parameter) => _viewModel.SelectedConnection != null;

		protected override async Task ExecuteAsync(object? parameter)
		{
			_logger.LogInformation("GenerateReportAsync");

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

				if (format!.Equals("pdf"))
				{
					await ConApiClient.Report.SaveReportPdfAsync(_viewModel.ProjectInfo.ProjectId, _viewModel.SelectedConnection.Id, saveFileDialog.FileName);
				}
				else if (format.Equals("docx"))
				{
					await ConApiClient.Report.SaveReportWordAsync(_viewModel.ProjectInfo.ProjectId, _viewModel.SelectedConnection.Id, saveFileDialog.FileName);
				}
				else
				{
					throw new Exception($"Unsupported format {format}");
				}

				_viewModel.OutputText = "Done";
			}
			catch (Exception ex)
			{
				_logger.LogWarning("GenerateReportAsync failed", ex);
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
