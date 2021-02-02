using IdeaStatiCa.ConnectionClient.Model;
using System;
using System.Windows.Input;

namespace IdeaStatiCa.ConnectionClient.Commands
{
	public abstract class ConnHiddenCalcCommandBase : ICommand
	{
		
		public ConnHiddenCalcCommandBase(IConHiddenCalcModel model)
		{
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
