using CommunityToolkit.Mvvm.Input;
using ConApiWpfClientApp.Models;
using IdeaStatiCa.Api.Connection.Model;
using IdeaStatiCa.ConnectionApi;
using Newtonsoft.Json;
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
		string? _filterJson;

		public ConnectionLibraryViewModel(ConnectionLibraryModel model, IConnectionApiClient connectionApiClient)
		{
			_connectionApiClient = connectionApiClient;
			_model = model;
			ProposeCommand = new AsyncRelayCommand(ProposeAsync, () => true);
		}

		public async Task InitAsync(Guid projectId, int connectionId, CancellationToken cts)
		{
			_model.ProjectId = projectId;
			_model.ConnectionId = connectionId;

			FilterJson = Tools.JsonTools.ToFormatedJson(_model.SearchParameters);

			await ProposeAsync();
		}

		public AsyncRelayCommand ProposeCommand { get; }

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

		private async Task ProposeAsync()
		{
			if(string.IsNullOrEmpty(FilterJson))
			{
				return;
			}

			try
			{
				ConConnectionLibrarySearchParameters filter = JsonConvert.DeserializeObject<ConConnectionLibrarySearchParameters>(FilterJson)!;

				_model.ProposedDesignItems = await _connectionApiClient.ConnectionLibrary.ProposeAsync(_model.ProjectId, _model.ConnectionId, filter, 0, CancellationToken.None);

				ProposedCdiVMs = new ObservableCollection<ProposedCdiViewModel>(_model.ProposedDesignItems.Select(c => new ProposedCdiViewModel(_connectionApiClient, _model.ProjectId, _model.ConnectionId, c)));
				SelectedCdiVM = ProposedCdiVMs.FirstOrDefault();

				_model.SearchParameters = filter;

			}
			catch
			{
				return;
			}
		}

		public string? FilterJson
		{
			get => _filterJson;
			set
			{
				SetProperty(ref _filterJson, value);
			}
		}

		private void UpdateDetails()
		{
			_selectedCdiVM?.InitDetailsAsync();

		}
	}
}
