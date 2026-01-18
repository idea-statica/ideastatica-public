using ConApiWpfClientApp.Services;
using ConApiWpfClientApp.ViewModels;
using ConnectionIomGenerator.UI.Services;
using ConnectionIomGenerator.UI.Views;
using IdeaStatiCa.Plugin;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConApiWpfClientApp.Commands
{
	/// <summary>
	/// Command to generate a connection from IOM using the IomEditorWindow.
	/// Opens the IOM editor dialog, generates IOM, and creates a project in the service by sending IOM directly through ImportIOMAsync.
	/// </summary>
	public class GenerateConnectionIomCommand : AsyncCommandBase
	{
		private readonly CancellationTokenSource _cts;

		/// <summary>
		/// Initializes a new instance of the <see cref="GenerateConnectionIomCommand"/> class.
		/// </summary>
		/// <param name="viewModel">The view model that owns this command.</param>
		/// <param name="logger">Logger for tracking command execution.</param>
		/// <param name="cts">Cancellation token source for async operations.</param>
		public GenerateConnectionIomCommand(
			ConApiWpfClientApp.ViewModels.MainWindowViewModel viewModel, 
			IPluginLogger logger, 
			CancellationTokenSource cts)
			: base(viewModel, logger)
		{
			_cts = cts;
		}

		/// <inheritdoc/>
		public override bool CanExecute(object? parameter) => ConApiClient != null && _viewModel.ProjectInfo == null;

		/// <inheritdoc/>
		protected override async Task ExecuteAsync(object? parameter)
		{
			_logger.LogInformation("GenerateConnectionIomAsync");

			if (ConApiClient == null)
			{
				throw new Exception("IConnectionApiClient is not connected");
			}

			_viewModel.IsBusy = true;
			try
			{
				// Create services for IomEditorWindow
				var iomService = new IomService(_logger);
				var fileDialogService = new ConnectionIomGenerator.UI.Services.FileDialogService();

				var iomGeneratorVM = new ConnectionIomGenerator.UI.ViewModels.MainWindowViewModel(_logger, iomService, fileDialogService);


				// Create and show the IOM editor dialog
				var editorWindow = new IomEditorWindow()
				{
					Owner = System.Windows.Application.Current.MainWindow,
					DataContext = iomGeneratorVM
				};

				bool? dialogResult = editorWindow.ShowDialog();

				// If user clicked OK and we have a result
				if (dialogResult == true && iomGeneratorVM?.Model?.IomContainer != null)
				{
					var model = iomGeneratorVM.Model;

					// Check if IOM was generated
					if (model.IomContainer != null)
					{
						_logger.LogInformation("IOM generated successfully, creating project from IOM");

						// Serialize the OpenModelContainer to XML string using IdeaRS.OpenModel.Tools
						string xmlString = IdeaRS.OpenModel.Tools.OpenModelContainerToXml(model.IomContainer);

						// Replace utf-16 with utf-8 (same as CreateProjectFromIomFileAsync)
						xmlString = xmlString.Replace("utf-16", "utf-8");

						// Convert XML string to MemoryStream
						using (var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(xmlString)))
						{
							memoryStream.Seek(0, SeekOrigin.Begin);

							// Send IOM directly to the service without saving to disk
							_logger.LogInformation("Importing IOM to Connection API service");

							var response = await ConApiClient.Project.ImportIOMWithHttpInfoAsync(
								containerXmlFile: memoryStream,
								connectionsToCreate: null, // null means create all connections
								cancellationToken: _cts.Token);

							_viewModel.ProjectInfo = response.Data;

							var projectInfoJson = Tools.JsonTools.ToFormatedJson(_viewModel.ProjectInfo);

							_viewModel.OutputText = string.Format("ProjectId = {0}\n\n{1}", ConApiClient.ActiveProjectId, projectInfoJson);

							_viewModel.Connections = new ObservableCollection<ConnectionViewModel>(_viewModel.ProjectInfo.Connections.Select(c => new ConnectionViewModel(c)));
						}

					}
				}
				else
				{
					_viewModel.OutputText = "Operation cancelled by user.";
					_logger.LogInformation("User cancelled the IOM editor dialog");
				}
			}
			catch (Exception ex)
			{
				_logger.LogWarning("GenerateConnectionIomAsync failed", ex);
				_viewModel.OutputText = $"Error: {ex.Message}\n\n{ex.StackTrace}";
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
