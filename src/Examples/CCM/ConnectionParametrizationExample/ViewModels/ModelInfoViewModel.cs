using CommunityToolkit.Mvvm.Input;
using ConnectionParametrizationExample.Models;
using ConnectionParametrizationExample.Services;
using PropertyChanged;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ConnectionParametrizationExample.ViewModels
{
	class ModelInfoViewModel : ViewModel
	{
		public ModelInfoSettings Settings { get; set; }

		private bool isBusy;

		public bool IsBusy 
		{
			get => isBusy;
			set
			{
				isBusy = value;
				RunCommand.NotifyCanExecuteChanged();
			}
		}

		public int CurrentProgress { get; set; }

		public List<string> ConnectionModelInfo { get; set; } = new List<string>();

		public RelayCommand RunCommand { get; }

		public string IdeaAppLocation
		{
			get => Settings.IdeaAppLocation;
			set
			{
				Settings.IdeaAppLocation = value;
				RunCommand.NotifyCanExecuteChanged();
			}
		}

		public string IdeaConFilesLocation
		{
			get => Settings.IdeaConFilesLocation;
			set
			{
				Settings.IdeaConFilesLocation = value;
				OnIdeaConFilesLocationChanged(value);
				RunCommand.NotifyCanExecuteChanged();
			}
		}

		public List<string> IdeaConFiles 
		{ 
			get => Settings.IdeaConFiles;
			set => Settings.IdeaConFiles = value;
		}
		private readonly ConnectionsManagerService service;

		public ModelInfoViewModel(ConnectionsManagerService service)
		{
			IsBusy = false;
			Settings = new ModelInfoSettings();
			RunCommand = new RelayCommand(Run, () => Directory.Exists(IdeaAppLocation) && Settings.IdeaConFiles.Count > 0 && !IsBusy);

			IdeaAppLocation = @"C:\Users\RadimMach\Repositories\IdeaStatiCa\bin\Debug\net48";
			IdeaConFilesLocation = @"C:\Users\RadimMach\Downloads";
			this.service = service;
		}

		[SuppressPropertyChangedWarnings]
		private void OnIdeaConFilesLocationChanged(string value)
		{
			if (Directory.Exists(value))
			{
				Settings.IdeaConFiles = Directory.GetFiles(value, "*.ideaCon").ToList();
			}
			else
			{
				Settings.IdeaConFiles = new List<string>();
			}
			OnPropertyChanged(nameof(IdeaConFiles));
		}

		public async void Run()
		{
			IsBusy = true;

			service.ProgressChanged += UpdateProgressBar;

			await Task.Run(() => service.SaveConnectionsCssToCsv(Settings));

			service.ProgressChanged -= UpdateProgressBar;

			IsBusy = false;
		}

		private void UpdateProgressBar(int progress)
		{
			CurrentProgress = progress;
		}
	}
}
