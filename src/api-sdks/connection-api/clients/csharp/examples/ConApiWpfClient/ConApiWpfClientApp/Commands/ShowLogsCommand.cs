using ConApiWpfClientApp.ViewModels;
using IdeaStatiCa.Plugin;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace ConApiWpfClientApp.Commands
{
	/// <summary>
	/// Command to open the IdeaStatica logs folder in Windows Explorer.
	/// </summary>
	public class ShowLogsCommand : AsyncCommandBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ShowLogsCommand"/> class.
		/// </summary>
		/// <param name="viewModel">The view model that owns this command.</param>
		/// <param name="logger">Logger for tracking command execution.</param>
		public ShowLogsCommand(MainWindowViewModel viewModel, IPluginLogger logger)
			: base(viewModel, logger)
		{
		}

		/// <inheritdoc/>
		public override bool CanExecute(object? parameter) => true;

		/// <inheritdoc/>
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
