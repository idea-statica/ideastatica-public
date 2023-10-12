using System;
using System.Windows.Input;

namespace IdeaStatiCa.ConnectionClient.Commands
{
	public class CustomCommand : ICommand
	{
		private Action<object> ExecuteFunc;
		private Func<object, bool> CanExecuteFunc;

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

		public CustomCommand(Func<object, bool> canExecuteFunc, Action<object> executeFunc)
		{
			ExecuteFunc = executeFunc;
			CanExecuteFunc = canExecuteFunc;
		}

		public bool CanExecute(object parameter)
		{
			return CanExecuteFunc(parameter);
		}

		public void Execute(object parameter)
		{
			ExecuteFunc(parameter);
		}
	}
}
