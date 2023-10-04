#if (CAD)
using BimApiLinkCadExample;
using BimApiLinkCadExample.CadExampleApi;
#else
using BimApiLinkFeaExample;
using BimApiLinkFeaExample.FeaExampleApi;
#endif
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace BimLinkExampleRunner.ViewModels
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
			//Provides the instance of the Fea Application Api or Model Object.
			//We have created a simple test api for this example which mocks a typical Api. 

#if (CAD)
			ICadApi cadApi = new CadApi();
			Task.Run(() => TestPlugin.Run(CheckbotLocation, cadApi, Logger));
#else
			IFeaApi feaApi = new FeaApi();
			Task.Run(() => TestPlugin.Run(CheckbotLocation, feaApi, Logger));
#endif
		}
	}
}