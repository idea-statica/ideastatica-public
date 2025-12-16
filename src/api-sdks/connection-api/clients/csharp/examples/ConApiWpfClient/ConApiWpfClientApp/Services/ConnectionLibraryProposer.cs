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
	public class ConnectionLibraryProposer : ITemplateProvider
	{
		private readonly IConnectionApiClient _connectionApiClient;
		private readonly IPluginLogger _logger;

		public ConnectionLibraryProposer(IConnectionApiClient connectionApiClient, IPluginLogger logger)
		{
			_connectionApiClient = connectionApiClient;
			_logger = logger;
		}

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
