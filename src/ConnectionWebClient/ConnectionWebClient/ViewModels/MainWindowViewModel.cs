using CommunityToolkit.Mvvm.Input;
using ConnectionWebClient.Tools;
using IdeaStatiCa.Plugin.Api.ConnectionRest;
using IdeaStatiCa.Plugin.Api.ConnectionRest.Model.Model_Project;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ConnectionWebClient.ViewModels
{
	public class MainWindowViewModel : ViewModelBase
	{
		public readonly IConnectionApiController connectionClient;
		private bool _isBusy;
		private string? outputText; 
		ObservableCollection<ConnectionViewModel>? connectionsVM;
		ConnectionViewModel? selectedConnection;
		private ConProject? _projectInfo;
		private CancellationTokenSource cts;
		

		public MainWindowViewModel(IConnectionApiController connectionClient)
		{
			this.cts = new CancellationTokenSource();
			this.connectionClient = connectionClient;
			OpenProjectCommand = new AsyncRelayCommand(OpenProjectAsync);
			CloseProjectCommand = new AsyncRelayCommand(CloseProjectAsync);

			//GetBriefResultsCommand = new AsyncRelayCommand(GetBriefResultsAsync);
			//GetDetailedResultsCommand = new AsyncRelayCommand(GetDetailedResultsAsync);

			//GetBucklingBriefResultsCommand = new AsyncRelayCommand(GetBucklingBriefResultsAsync);
			//GetBucklingDetailedResultsCommand = new AsyncRelayCommand(GetBucklingDetailedResultsAsync);
			Connections = new ObservableCollection<ConnectionViewModel>();
			selectedConnection = null;
		}

		public bool IsBusy
		{
			get { return _isBusy; }
			set { SetProperty(ref _isBusy, value); }
		}

		public ConProject? ProjectInfo
		{
			get { return _projectInfo; }
			set { SetProperty(ref _projectInfo, value); }
		}


		public ObservableCollection<ConnectionViewModel>? Connections
		{
			get => connectionsVM;
			set
			{
				SetProperty(ref connectionsVM, value);
			}
		}

		public ConnectionViewModel? SelectedConnection
		{
			get => selectedConnection;
			set
			{
				SetProperty(ref selectedConnection, value);
			}
		}

		public string? OutputText
		{
			get => outputText;
			set
			{
				SetProperty(ref outputText, value);
			}
		}

		public AsyncRelayCommand OpenProjectCommand { get; }

		public AsyncRelayCommand CloseProjectCommand { get; }

		//public AsyncRelayCommand GetBriefResultsCommand { get; }

		//public AsyncRelayCommand GetDetailedResultsCommand { get; }

		//public AsyncRelayCommand GetBucklingBriefResultsCommand { get; }

		//public AsyncRelayCommand GetBucklingDetailedResultsCommand { get; }

		private async Task OpenProjectAsync()
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "IdeaConnection | *.ideacon";
			if (openFileDialog.ShowDialog() != true)
			{
				return;
			}

			IsBusy = true;
			try
			{
				ProjectInfo = await connectionClient.OpenProjectAsync(openFileDialog.FileName, cts.Token);

				OutputText =JsonTools.ToFormatedJson(ProjectInfo);
				Connections = new ObservableCollection<ConnectionViewModel>(ProjectInfo.Connections.Select(c => new ConnectionViewModel(c)));
			}
			finally
			{
				IsBusy = false;
			}
		}

		//private async Task GetBriefResultsAsync()
		//{
		//	if (SelectedConnection == null)
		//	{
		//		return;
		//	}

		//	IsBusy = true;
		//	try
		//	{
		//		var chekRes = await connectionClient.GetPlasticBriefResultsAsync(SelectedConnection.Id, cts.Token);
		//		OutputText = JsonTools.ToFormatedJson(chekRes);
		//	}
		//	finally
		//	{
		//		IsBusy = false;
		//	}
		//}

		//private async Task GetDetailedResultsAsync()
		//{
		//	if (SelectedConnection == null)
		//	{
		//		return;
		//	}

		//	IsBusy = true;
		//	try
		//	{
		//		var res = await connectionClient.GetPlasticDetailedResultsJsonAsync(SelectedConnection.Id, cts.Token);
		//		OutputText = JsonTools.FormatJson(res);
		//	}
		//	finally
		//	{
		//		IsBusy = false;
		//	}
		//}

		//private async Task GetBucklingBriefResultsAsync()
		//{
		//	if (SelectedConnection == null)
		//	{
		//		return;
		//	}

		//	IsBusy = true;
		//	try
		//	{
		//		var chekRes = await connectionClient.GetBucklingBriefResultsAsync(SelectedConnection.Id, cts.Token);
		//		OutputText = JsonTools.ToFormatedJson(chekRes);
		//	}
		//	finally
		//	{
		//		IsBusy = false;
		//	}
		//}


		//private async Task GetBucklingDetailedResultsAsync()
		//{
		//	if (SelectedConnection == null)
		//	{
		//		return;
		//	}

		//	IsBusy = true;
		//	try
		//	{
		//		var json = await connectionClient.GetBucklingDetailedResultsJsonAsync(SelectedConnection.Id, cts.Token);
		//		OutputText = JsonTools.FormatJson(json);
		//	}
		//	finally
		//	{
		//		IsBusy = false;
		//	}
		//}

		private async Task CloseProjectAsync()
		{
			if(ProjectInfo == null)
			{
				return;
			}

			IsBusy = true;
			try
			{
				await connectionClient.CloseProjectAsync(cts.Token);
				ProjectInfo = null;
				SelectedConnection = null;
				Connections = new ObservableCollection<ConnectionViewModel>();
				OutputText = string.Empty;
			}
			finally
			{
				IsBusy = false;
			}
		}
	}


}
