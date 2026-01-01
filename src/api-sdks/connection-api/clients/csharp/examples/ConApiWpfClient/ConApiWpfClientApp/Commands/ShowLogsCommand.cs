using ConApiWpfClientApp.ViewModels;
using IdeaStatiCa.Plugin;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace ConApiWpfClientApp.Commands
{
	public class ShowLogsCommand : AsyncCommandBase
	{
		public ShowLogsCommand(MainWindowViewModel viewModel, IPluginLogger logger)
			: base(viewModel, logger)
		{
		}

		public override bool CanExecute(object? parameter) => true;

		protected override async Task ExecuteAsync(object? parameter)
		{
			_logger.LogInformation("ShowIdeaStatiCaLogsAsync");

			var tempPath = Environment.GetEnvironmentVariable("TEMP");
			var ideaLogDir = Path.Combine(tempPath!, "IdeaStatica", "Logs");

			try
			{
				Process.Start("explorer.exe", ideaLogDir);
			}
			catch (Exception ex)
			{
				_logger.LogWarning("Error", ex);
			}

			await Task.CompletedTask;
		}
	}
}
