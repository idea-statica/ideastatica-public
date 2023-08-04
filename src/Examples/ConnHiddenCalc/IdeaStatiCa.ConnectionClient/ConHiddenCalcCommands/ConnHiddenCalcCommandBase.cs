using IdeaStatiCa.ConnectionClient.Model;
using IdeaStatiCa.Plugin;
using System;
using System.Windows.Input;

namespace IdeaStatiCa.ConnectionClient.Commands
{
	public abstract class ConnHiddenCalcCommandBase : ICommand
	{
		protected readonly IPluginLogger Logger;
		public ConnHiddenCalcCommandBase(IConHiddenCalcModel model, IPluginLogger logger = null)
		{
			if (logger == null)
			{
				Logger = new IdeaStatiCa.Plugin.NullLogger();
			}
			else
			{
				Logger = logger;
			}

			Model = model;
		}

		public static bool IsCommandRunning { get; set; }

		public IConHiddenCalcModel Model { get; private set; }

		public event EventHandler CanExecuteChanged
		{
			add
			{
				CommandManager.RequerySuggested += value;
			}
			remove
			{
				CommandManager.RequerySuggested -= value;
			}
		}

		public abstract bool CanExecute(object parameter);

		public abstract void Execute(object parameter);
	}
}
