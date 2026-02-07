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
	/// <summary>
	/// View model for the connection library window, allowing users to search for and select
	/// connection design items from the IDEA StatiCa connection library.
	/// </summary>
	public class ConnectionLibraryViewModel : ViewModelBase
	{
		private readonly IPluginLogger _logger;
		private readonly IConnectionApiClient _connectionApiClient;
		private readonly ConnectionLibraryModel _model;
		ObservableCollection<ProposedCdiViewModel>? _allProposedCdi;
		ProposedCdiViewModel? _selectedCdiVM;
		string? _filterJson;

		/// <summary>
		/// Initializes a new instance of the <see cref="ConnectionLibraryViewModel"/> class.
		/// </summary>
		/// <param name="logger">The logger for diagnostic output.</param>
		/// <param name="model">The model containing search parameters and results.</param>
		/// <param name="connectionApiClient">The API client for querying the connection library.</param>
		public ConnectionLibraryViewModel(IPluginLogger logger, ConnectionLibraryModel model, IConnectionApiClient connectionApiClient)
		{
			_connectionApiClient = connectionApiClient;
			_model = model;
			_logger = logger;
			ProposeCommand = new AsyncRelayCommand(ProposeAsync, () => true);
		}

		/// <summary>
		/// Initializes the view model with the specified project and connection context, then performs an initial search.
		/// </summary>
		/// <param name="projectId">The unique identifier of the project.</param>
		/// <param name="connectionId">The identifier of the connection.</param>
		/// <param name="cts">A cancellation token to cancel the operation.</param>
		public async Task InitAsync(Guid projectId, int connectionId, CancellationToken cts)
		{
			_logger.LogInformation($"ConnectionLibraryViewModel.InitAsync projectId = {projectId} connectionId = {connectionId}");
			_model.ProjectId = projectId;
			_model.ConnectionId = connectionId;

			FilterJson = Tools.JsonTools.ToFormatedJson(_model.SearchParameters);

			await ProposeAsync();
		}

		/// <summary>
		/// Gets the command that triggers a new search in the connection library.
		/// </summary>
		public AsyncRelayCommand ProposeCommand { get; }

		/// <summary>
		/// Gets or sets the collection of proposed connection design item view models.
		/// </summary>
		public ObservableCollection<ProposedCdiViewModel>? ProposedCdiVMs
		{
			get => _allProposedCdi;
			set
			{
				SetProperty(ref _allProposedCdi, value);
			}
		}

		/// <summary>
		/// Gets or sets the currently selected connection design item.
		/// When changed, triggers loading of the item's detail data.
		/// </summary>
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

		/// <summary>
		/// Searches the connection library using the current filter and populates the proposed design items.
		/// </summary>
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

		/// <summary>
		/// Gets or sets the JSON string representing the search filter parameters.
		/// </summary>
		public string? FilterJson
		{
			get => _filterJson;
			set
			{
				SetProperty(ref _filterJson, value);
			}
		}

		/// <summary>
		/// Triggers asynchronous loading of detail data for the selected design item.
		/// </summary>
		private void UpdateDetails()
		{
			_selectedCdiVM?.InitDetailsAsync();

		}
	}
}
