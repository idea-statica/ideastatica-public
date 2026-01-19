using ConApiWpfClientApp.ViewModels;
using ConnectionIomGenerator.UI.Views;
using IdeaStatiCa.Plugin;
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
	/// Command that generates a connection from IOM (IDEA Open Model) using an interactive editor dialog.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This command provides an end-to-end workflow for creating steel connections in IDEA StatiCa Connection:
	/// </para>
	/// <list type="number">
	/// <item><description>Opens the <see cref="IomEditorWindow"/> dialog for interactive connection definition</description></item>
	/// <item><description>Allows users to define connection geometry, members, and loading in JSON format</description></item>
	/// <item><description>Generates the OpenModelContainer using the BIM API</description></item>
	/// <item><description>Serializes the IOM to XML format</description></item>
	/// <item><description>Sends the IOM directly to the Connection API service via memory stream (no disk I/O)</description></item>
	/// <item><description>Creates a new project in the service with the generated connections</description></item>
	/// </list>
	/// <para>
	/// The command follows the same pattern as <c>CreateProjectFromIomFileAsync</c> extension method but works
	/// entirely in memory without requiring temporary file creation. The OpenModelContainer is serialized to XML,
	/// converted to UTF-8 encoding, and sent to the API via <see cref="System.IO.MemoryStream"/>.
	/// </para>
	/// <para><b>Execution Requirements:</b></para>
	/// <list type="bullet">
	/// <item><description>Connection API client must be connected (<c>ConApiClient != null</c>)</description></item>
	/// <item><description>No project can be currently open (<c>ProjectInfo == null</c>)</description></item>
	/// </list>
	/// <para><b>User Workflow:</b></para>
	/// <list type="number">
	/// <item><description>User clicks the "Generate connection" button in the UI</description></item>
	/// <item><description>IomEditorWindow opens with default ECEN connection template</description></item>
	/// <item><description>User can edit connection definition (material, members, cross-sections, etc.)</description></item>
	/// <item><description>User can optionally generate default loading</description></item>
	/// <item><description>User clicks "Generate IOM" to create the OpenModelContainer</description></item>
	/// <item><description>User clicks "OK" to create the project</description></item>
	/// <item><description>Project is created in the Connection API service</description></item>
	/// <item><description>Connections are displayed in the main window</description></item>
	/// </list>
	/// </remarks>
	/// <example>
	/// <para>This command is typically bound to a button in XAML:</para>
	/// <code language="xml">
	/// &lt;Button Command="{Binding ConIomGeneratorCommand, Mode=OneWay}" 
	///         ToolTip="Define basic connection geometry. Use BIM Api to convert it to IOM and create connection."&gt;
	///     Generate connection
	/// &lt;/Button&gt;
	/// </code>
	/// <para>And initialized in the ViewModel constructor:</para>
	/// <code language="csharp">
	/// ConIomGeneratorCommand = new GenerateConnectionIomCommand(this, logger, cts);
	/// </code>
	/// </example>
	public class GenerateConnectionIomCommand : AsyncCommandBase
	{
		private readonly CancellationTokenSource _cts;

		/// <summary>
		/// Initializes a new instance of the <see cref="GenerateConnectionIomCommand"/> class.
		/// </summary>
		/// <param name="viewModel">The main window view model that owns this command. Provides access to the Connection API client,
		/// project information, and UI state management.</param>
		/// <param name="logger">The logger instance used to log diagnostic and operational information throughout the command execution.</param>
		/// <param name="cts">The <see cref="CancellationTokenSource"/> used to manage cancellation of asynchronous operations,
		/// particularly the API call to import the IOM.</param>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="viewModel"/> or <paramref name="logger"/> is null.</exception>
		public GenerateConnectionIomCommand(
			ConApiWpfClientApp.ViewModels.MainWindowViewModel viewModel,
			IPluginLogger logger,
			CancellationTokenSource cts)
			: base(viewModel, logger)
		{
			_cts = cts;
		}

		/// <summary>
		/// Determines whether the command can execute in its current state.
		/// </summary>
		/// <param name="parameter">Command parameter (not used by this command).</param>
		/// <returns>
		/// <c>true</c> if the Connection API client is connected and no project is currently open;
		/// otherwise, <c>false</c>.
		/// </returns>
		/// <remarks>
		/// This command can only execute when:
		/// <list type="bullet">
		/// <item><description><c>ConApiClient != null</c> - The Connection API client is initialized and connected</description></item>
		/// <item><description><c>ProjectInfo == null</c> - No project is currently open (similar to ImportIomCommand behavior)</description></item>
		/// </list>
		/// </remarks>
		public override bool CanExecute(object? parameter) => ConApiClient != null && _viewModel.ProjectInfo == null;

		/// <summary>
		/// Executes the command asynchronously to generate and import a connection from IOM.
		/// </summary>
		/// <param name="parameter">Command parameter (not used by this command).</param>
		/// <returns>A task representing the asynchronous operation.</returns>
		/// <exception cref="Exception">Thrown when the Connection API client is not connected.</exception>
		/// <remarks>
		/// <para><b>Execution Flow:</b></para>
		/// <list type="number">
		/// <item><description>Sets <c>IsBusy = true</c> to indicate operation in progress</description></item>
		/// <item><description>Opens <see cref="IomEditorWindow"/> dialog for user input</description></item>
		/// <item><description>If user clicks OK and IOM is generated:
		///     <list type="bullet">
		///     <item><description>Serializes <c>OpenModelContainer</c> to XML using <c>IdeaRS.OpenModel.Tools.OpenModelContainerToXml</c></description></item>
		///     <item><description>Replaces "utf-16" encoding declaration with "utf-8" for API compatibility</description></item>
		///     <item><description>Creates <see cref="MemoryStream"/> from UTF-8 encoded XML bytes</description></item>
		/// <item><description>Calls <c>ConApiClient.Project.ImportIOMWithHttpInfoAsync</c> with the stream</description></item>
		///     <item><description>Updates <c>ProjectInfo</c> with the created project data</description></item>
		///     <item><description>Populates <c>Connections</c> collection with created connections</description></item>
		///     <item><description>Displays formatted project information in <c>OutputText</c></description></item>
		///     </list>
		/// </description></item>
		/// <item><description>If user cancels, displays cancellation message</description></item>
		/// <item><description>On error, logs the exception and displays error details in <c>OutputText</c></description></item>
		/// <item><description>Finally:
		///   <list type="bullet">
		///     <item><description>Sets <c>IsBusy = false</c></description></item>
		///     <item><description>Calls <c>RefreshCommands()</c> to update command availability</description></item>
		///     <item><description>Selects the first connection if any were created</description></item>
		///     </list>
		/// </description></item>
		/// </list>
		/// <para><b>No Disk I/O:</b></para>
		/// <para>
		/// Unlike <c>CreateProjectFromIomFileAsync</c>, this command does not save the IOM to a temporary file.
		/// The entire operation is performed in memory, improving performance and security.
		/// </para>
		/// <para><b>Error Handling:</b></para>
		/// <para>
		/// All exceptions are caught, logged via <see cref="IPluginLogger"/>, and displayed to the user in the
		/// <c>OutputText</c> property. The UI is always returned to a non-busy state even if an error occurs.
		/// </para>
		/// <para><b>Cancellation Support:</b></para>
		/// <para>
		/// The API call to <c>ImportIOMWithHttpInfoAsync</c> supports cancellation via the <see cref="CancellationToken"/>
		/// provided by the <c>_cts</c> field. User can cancel long-running operations.
		/// </para>
		/// </remarks>
		/// <example>
		/// <para>Typical execution scenario:</para>
		/// <code language="csharp">
		/// // User clicks button bound to ConIomGeneratorCommand
		/// // Command checks CanExecute (client connected, no project open)
		/// // ExecuteAsync is called:
		/// 
		/// // 1. Dialog opens, user defines connection
		/// var editorWindow = new IomEditorWindow(editorWindowVM) { Owner = mainWindow };
		/// bool? result = editorWindow.ShowDialog();
		/// 
		/// // 2. If OK and IOM generated, serialize to XML
		/// if (result == true &amp;&amp; model.IomContainer != null)
		/// {
		///     string xmlString = OpenModelContainerToXml(model.IomContainer);
		///     xmlString = xmlString.Replace("utf-16", "utf-8");
		///
		///     // 3. Send to API via memory stream
		///     using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(xmlString)))
		///{
		///         var response = await ImportIOMWithHttpInfoAsync(stream, null, cts.Token);
		///         ProjectInfo = response.Data;
		///     }
		/// }
		/// </code>
		/// </example>
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

				var editorWindowVM = _viewModel._iomEditorViewModel;

				// Create and show the IOM editor dialog
				var editorWindow = new IomEditorWindow(editorWindowVM)
				{
					Owner = System.Windows.Application.Current.MainWindow,
				};

				bool? dialogResult = editorWindow.ShowDialog();

				// If user clicked OK and we have a result
				if (dialogResult == true && editorWindowVM?.IomEditorViewModel != null)
				{
					var model = await editorWindowVM.GetResultModelAsync();

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
