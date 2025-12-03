using ConApiWpfClientApp.Models;
using IdeaStatiCa.ConnectionApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConApiWpfClientApp.ViewModels
{
	public class ConnectionLibraryViewModel : ViewModelBase
	{
		private readonly IConnectionApiClient _connectionApiClient;
		private readonly ConnectionLibraryModel _model;
		ObservableCollection<ProposedCdiViewModel>? _allProposedCdi;
		ProposedCdiViewModel? _selectedCdiVM;

		public ConnectionLibraryViewModel(ConnectionLibraryModel model, IConnectionApiClient connectionApiClient)
		{
			_connectionApiClient = connectionApiClient;
			_model = model;
		}

		public async Task InitAsync(Guid projectId, int connectionId, CancellationToken cts)
		{
			_model.ProjectId = projectId;
			_model.ConnectionId = connectionId;

			_model.ProposedDesignItems = await _connectionApiClient.ConnectionLibrary.ProposeAsync(projectId, connectionId, _model.SearchParameters, 0, cts);

			ProposedCdiVMs = new ObservableCollection<ProposedCdiViewModel>(_model.ProposedDesignItems.Select(c => new ProposedCdiViewModel(_connectionApiClient, projectId, connectionId, c)));
			SelectedCdiVM = ProposedCdiVMs.FirstOrDefault();
		}

		public ObservableCollection<ProposedCdiViewModel>? ProposedCdiVMs
		{
			get => _allProposedCdi;
			set
			{
				SetProperty(ref _allProposedCdi, value);
			}
		}

		public ProposedCdiViewModel? SelectedCdiVM
		{
			get => _selectedCdiVM;
			set
			{
				if (_selectedCdiVM != value)
				{
					// TODO Correct Synchronization 
					SetProperty(ref _selectedCdiVM, value);
					UpdateDetails();
				}
			}
		}

		private void UpdateDetails()
		{
			_selectedCdiVM?.InitDetailsAsync();

		}
	}
}
