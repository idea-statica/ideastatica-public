using System;
using System.Windows.Input;

namespace IdeaStatiCaFake
{
	public class CustomCommand : ICommand
	{
#pragma warning disable CS0067 // The event is never used
		private event EventHandler _internalCanExecuteChanged;
#pragma warning restore CS0067

		private Action<object> ExecuteFunc;
		private Func<object, bool> CanExecuteFunc;

		public event EventHandler CanExecuteChanged
		{
			add
			{
				_internalCanExecuteChanged += value;
				CommandManager.RequerySuggested += value;
			}
			remove
			{
				_internalCanExecuteChanged -= value;
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