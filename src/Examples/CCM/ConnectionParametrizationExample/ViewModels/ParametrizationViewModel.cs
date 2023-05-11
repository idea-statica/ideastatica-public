using CommunityToolkit.Mvvm.Input;
using ConnectionParametrizationExample.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ConnectionParametrizationExample.ViewModels
{
    class ParametrizationViewModel : ViewModel
	{
		public ParametrizedModel model;
		public List<string> IdeaConFiles { get; set; } = new List<string>();
		public List<string> ResultChecks { get; set; } = new List<string>();

		public string ideaAppLocation = String.Empty;
		public string IdeaAppLocation
		{
			get => ideaAppLocation;
			set
			{
				ideaAppLocation = value;
				OnPropertyChanged();
				RunCommand.NotifyCanExecuteChanged();
			}
		}

		public string ideaConFilesLocation = String.Empty;
		public string IdeaConFilesLocation
		{
			get => ideaConFilesLocation;
			set
			{
				ideaConFilesLocation = value;
				OnPropertyChanged();
				OnIdeaConFilesLocationChanged(value);
				RunCommand.NotifyCanExecuteChanged();
			}
		}

		public int CurrentProgress { get; set; }
		public bool IsBusy => IsInProgress;
		private bool IsInProgress { get; set; } = false;
		readonly BackgroundWorker worker = new BackgroundWorker();

		public RelayCommand RunCommand { get; }

		public ParametrizationViewModel()
		{
			model = new ParametrizedModel();
			RunCommand = new RelayCommand(Run, () => Directory.Exists(IdeaAppLocation) && IdeaConFiles.Count > 0 && !IsInProgress);

			worker.DoWork += DoWork;
			worker.WorkerReportsProgress = true;
			worker.ProgressChanged += ProgressChanged;
			worker.RunWorkerCompleted += RunWorkerCompleted;

			IdeaAppLocation = @"C:\Program Files\IDEA StatiCa\StatiCa 23.0";
			IdeaAppLocation = @"C:\Users\RadimMach\Repositories\IdeaStatiCa\bin\Debug\net48";
			IdeaConFilesLocation = @"C:\Users\User\Downloads\models";
			IdeaConFilesLocation = @"C:\Users\RadimMach\Downloads\models";
		}

		private void ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			CurrentProgress = e.ProgressPercentage;
		}

		void DoWork(object sender, DoWorkEventArgs e)
		{
			if (IsInProgress)
				return;
			CurrentProgress = 0;
			IsInProgress = true;
			BackgroundWorker worker = sender as BackgroundWorker;
			model.RunParametrizedAnalysis(IdeaConFiles, ResultChecks, worker);
		}
		void RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			CurrentProgress = 100;
			IsInProgress = false;
		}

		public void Run()
		{
			model.IdeaAppLocation = IdeaAppLocation;
			model.IdeaConFilesLocation = IdeaConFilesLocation;

			worker.RunWorkerAsync();
		}

		private void OnIdeaConFilesLocationChanged(string value)
		{
			if (Directory.Exists(value))
			{
				IdeaConFiles = Directory.GetFiles(value, "*.ideaCon").ToList();
			}
			else
			{
				IdeaConFiles = new List<string>();
			}
		}
	}
}
