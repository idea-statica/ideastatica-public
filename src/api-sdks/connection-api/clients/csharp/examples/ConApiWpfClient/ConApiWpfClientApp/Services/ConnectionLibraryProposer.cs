using ConApiWpfClientApp.Models;
using ConApiWpfClientApp.ViewModels;
using ConApiWpfClientApp.Views;
using IdeaStatiCa.ConnectionApi;
using IdeaStatiCa.Plugin;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace ConApiWpfClientApp.Services
{
	/// <summary>
	/// Template provider that browses the IDEA StatiCa connection library,
	/// presenting a UI for the user to search and select a connection design item.
	/// </summary>
	public class ConnectionLibraryProposer : ITemplateProvider
	{
		private readonly IConnectionApiClient _connectionApiClient;
		private readonly IPluginLogger _logger;

		/// <summary>
		/// Initializes a new instance of the <see cref="ConnectionLibraryProposer"/> class.
		/// </summary>
		/// <param name="connectionApiClient">The API client for querying the connection library.</param>
		/// <param name="logger">The logger for diagnostic output.</param>
		public ConnectionLibraryProposer(IConnectionApiClient connectionApiClient, IPluginLogger logger)
		{
			_connectionApiClient = connectionApiClient;
			_logger = logger;
		}

		/// <summary>
		/// Opens the connection library window, allowing the user to search for and select a template.
		/// </summary>
		/// <param name="projectId">The unique identifier of the project.</param>
		/// <param name="connectionId">The identifier of the connection.</param>
		/// <param name="cts">A cancellation token to cancel the operation.</param>
		/// <returns>A <see cref="ConnectionLibraryModel"/> containing the selected template XML and search parameters,
		/// or <see langword="null"/> if the user cancelled the selection.</returns>
		public async Task<ConnectionLibraryModel?> GetTemplateAsync(Guid projectId, int connectionId, CancellationToken cts)
		{
			_logger.LogInformation("ConnectionLibraryProposer.GetTemplateAsync");

			var members = await _connectionApiClient.Member.GetMembersAsync(projectId, connectionId, 0, cts);

			var model = new ConnectionLibraryModel();
			model.SearchParameters.InPredefinedSet = true;
			model.SearchParameters.InCompanySet = true;
			model.SearchParameters.InPersonalSet = true;
			model.SearchParameters.Members = members.Select(m => m.Id).ToList();

			var vm = new ConnectionLibraryViewModel(_logger, model, _connectionApiClient);

			await vm.InitAsync(projectId, connectionId, cts);

			var conLibWindow = new ConnectionLibraryWindow();
			conLibWindow.Owner = Application.Current.MainWindow;

			conLibWindow.DataContext = vm;

			var dlgRes = conLibWindow.ShowDialog();
			if(dlgRes != true || string.IsNullOrEmpty(vm?.SelectedCdiVM?.TemplateXml))
			{
				_logger.LogInformation("ConnectionLibraryProposer.GetTemplateAsync : Operation is cancelled");
				return null;
			}

			_logger.LogInformation("ConnectionLibraryProposer.GetTemplateAsync : Returning the selected template");
			model.SelectedTemplateXml = vm.SelectedCdiVM.TemplateXml;
			return model;
		}
	}
}
