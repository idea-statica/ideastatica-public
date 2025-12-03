using ConApiWpfClientApp.Models;
using ConApiWpfClientApp.ViewModels;
using ConApiWpfClientApp.Views;
using IdeaStatiCa.ConnectionApi;
using System;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace ConApiWpfClientApp.Services
{
	public class ConnectionLibraryProposer : ITemplateProvider
	{
		private readonly IConnectionApiClient _connectionApiClient;
		public ConnectionLibraryProposer(IConnectionApiClient connectionApiClient)
		{
			_connectionApiClient = connectionApiClient;
		}

		public async Task<string> GetTemplateAsync(Guid projectId, int connectionId, CancellationToken cts)
		{
			var model = new ConnectionLibraryModel();
			model.SearchParameters.InPredefinedSet = true;
			model.SearchParameters.InCompanySet = true;
			model.SearchParameters.InPersonalSet = true;

			var vm = new ConnectionLibraryViewModel(model, _connectionApiClient);

			await vm.InitAsync(projectId, connectionId, cts);

			var conLibWindow = new ConnectionLibraryWindow();
			conLibWindow.Owner = Application.Current.MainWindow;

			conLibWindow.DataContext = vm;

			var dlgRes = conLibWindow.ShowDialog();
			if(dlgRes != true || string.IsNullOrEmpty(vm?.SelectedCdiVM?.TemplateXml))
			{
				return string.Empty;
			}

			return vm.SelectedCdiVM.TemplateXml;
		}
	}
}
