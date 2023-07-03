using CommunityToolkit.Mvvm.Input;
using IdeaRS.OpenModel.Connection;
using IdeaStatiCa.ConnectionClient;
using Microsoft.Win32;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ConnectionWebClient.ViewModels
{
	public class MainWindowViewModel : ViewModelBase
	{
		public readonly IConnectionClient connectionClient;
		private bool _isBusy;
		private ConProjectInfo? _projectInfo;
		private CancellationTokenSource cts;
		

		public MainWindowViewModel(IConnectionClient connectionClient)
		{
			this.cts = new CancellationTokenSource();
			this.connectionClient = connectionClient;
			OpenProjectCommand = new AsyncRelayCommand(OpenProjectCommandAsync, () => !IsBusy && ProjectInfo == null);
		}

		public bool IsBusy
		{
			get { return _isBusy; }
			set { SetProperty(ref _isBusy, value); }
		}

		public ConProjectInfo? ProjectInfo
		{
			get { return _projectInfo; }
			set { SetProperty(ref _projectInfo, value); }
		}


		public AsyncRelayCommand OpenProjectCommand { get; }

		public async Task OpenProjectCommandAsync()
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
				using (FileStream fs = new FileStream(openFileDialog.FileName, FileMode.Open, FileAccess.Read))
				{
					ProjectInfo = await connectionClient.OpenProjectAsync(fs, cts.Token);
				}
			}
			finally
			{
				IsBusy = false;
			}
		}
	}


}
