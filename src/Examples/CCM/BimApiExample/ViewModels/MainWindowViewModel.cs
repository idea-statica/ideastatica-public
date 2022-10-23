using BimApiLinkFeaExample;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace BimApiExample.ViewModels
{
	internal class MainWindowViewModel
	{
		public MainWindowViewModel()
		{
			RunCheckbotCommand = new RelayCommand(OnRunCheckbot, () => File.Exists(CheckbotLocation));

			Logger = new AppLogger(Dispatcher.CurrentDispatcher) { EnableDebug = true, };
		}

		public IEnumerable<string> Actions { get; } = new List<string>();

		public string CheckbotLocation
		{
			get => Properties.Settings.Default.CheckbotLocation;
			set
			{
				Properties.Settings.Default.CheckbotLocation = value;
				Properties.Settings.Default.Save();
				RunCheckbotCommand.NotifyCanExecuteChanged();
			}
		}

		public RelayCommand RunCheckbotCommand { get; }

		public AppLogger Logger { get; }

		private void OnRunCheckbot()
		{
			Task.Run(() => TestPlugin.Run(CheckbotLocation, Logger));
		}
	}
}