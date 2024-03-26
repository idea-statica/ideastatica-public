using BimApiLinkCadExample.CadExampleApi;
using BimApiLinkFeaExample.FeaExampleApi;
using CommunityToolkit.Mvvm.Input;
using System;
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

		public ApplicationType ApplicationType
		{
			get => Properties.Settings.Default.ApplicationType;
			set
			{
				Properties.Settings.Default.ApplicationType = value;
				Properties.Settings.Default.Save();
			}
		}

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

			if (ApplicationType is ApplicationType.CAD)
			{
				ICadApi cadApi = new CadApi();
				Task.Run(() => BimApiLinkCadExample.TestPlugin.Run(CheckbotLocation, cadApi, Logger));
			}
			else if (ApplicationType is ApplicationType.FEA)
			{
				IFeaApi feaApi = new FeaApi();
				Task.Run(() => BimApiLinkFeaExample.TestPlugin.Run(CheckbotLocation, feaApi, Logger));
			}
			else
			{
				throw new NotImplementedException();
			}
		}
	}

}