using CommunityToolkit.Mvvm.Input;
using ConApiWpfClientApp.Models;
using IdeaStatiCa.Api.Connection.Model;
using IdeaStatiCa.ConnectionApi;
using IdeaStatiCa.Plugin;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ConApiWpfClientApp.ViewModels
{
	public class ConnectionLibraryViewModel : ViewModelBase
	{
		private readonly IPluginLogger _logger;
		private readonly IConnectionApiClient _connectionApiClient;
		private readonly ConnectionLibraryModel _model;
		ObservableCollection<ProposedCdiViewModel>? _allProposedCdi;
		ProposedCdiViewModel? _selectedCdiVM;
		string? _filterJson;

		public ConnectionLibraryViewModel(IPluginLogger logger, ConnectionLibraryModel model, IConnectionApiClient connectionApiClient)
		{
			_connectionApiClient = connectionApiClient;
			_model = model;
			_logger = logger;
			ProposeCommand = new AsyncRelayCommand(ProposeAsync, () => true);
		}

		public async Task InitAsync(Guid projectId, int connectionId, CancellationToken cts)
		{
			_logger.LogInformation($"ConnectionLibraryViewModel.InitAsync projectId = {projectId} connectionId = {connectionId}");
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
			_logger.LogInformation("ConnectionLibraryViewModel.ProposeAsync");

			if (string.IsNullOrEmpty(FilterJson))
			{
				return;
			}

			try
			{
				ConConnectionLibrarySearchParameters filter = JsonConvert.DeserializeObject<ConConnectionLibrarySearchParameters>(FilterJson)!;

				_model.ProposedDesignItems = await _connectionApiClient.ConnectionLibrary.ProposeAsync(_model.ProjectId, _model.ConnectionId, filter, 0, CancellationToken.None);

				ProposedCdiVMs = new ObservableCollection<ProposedCdiViewModel>(_model.ProposedDesignItems.Select(c => new ProposedCdiViewModel(_connectionApiClient, _model.ProjectId, _model.ConnectionId, c, _logger)));
				SelectedCdiVM = ProposedCdiVMs.FirstOrDefault();

				_model.SearchParameters = filter;
				_logger.LogInformation($"ConnectionLibraryViewModel.ProposeAsync {ProposedCdiVMs?.Count} design items is proposed");
			}
			catch(Exception e)
			{
				_logger.LogInformation("ConnectionLibraryViewModel.ProposeAsync : Failed", e);
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
