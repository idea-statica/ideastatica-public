using ConApiWpfClientApp.ViewModels;
using IdeaStatiCa.Plugin;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace ConApiWpfClientApp.Commands
{
	public class EditDiagnosticsCommand : AsyncCommandBase
	{
		public EditDiagnosticsCommand(MainWindowViewModel viewModel, IPluginLogger logger)
			: base(viewModel, logger)
		{
		}

		public override bool CanExecute(object? parameter) => true;

		protected override async Task ExecuteAsync(object? parameter)
		{
			_logger.LogInformation("EditDiagnosticsAsync");

			string localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
			var ideaDiagnosticsConfig = Path.Combine(localAppDataPath!, "IDEA_RS", "IdeaDiagnostics.config");

			try
			{
				Process.Start("notepad.exe", ideaDiagnosticsConfig);
			}
			catch (Exception ex)
			{
				_logger.LogWarning("Error", ex);
			}

			await Task.CompletedTask;
		}
	}
}
