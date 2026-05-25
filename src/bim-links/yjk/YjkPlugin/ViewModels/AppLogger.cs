using IdeaStatiCa.Plugin;
using IdeaStatiCa.PluginLogger;
using IdeaStatiCa.Public;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace yjk.ViewModels
{
	internal class AppLogger : IPluginLogger
	{
		private IPluginLogger _logger;

		public static AppLogger Instance => _instance;

		private static readonly AppLogger _instance = new AppLogger();

		public AppLogger()
		{
			ClearCommand = new DelegateCommand(OnClear);

			SerilogFacade.Initialize("IdeaYJKPlugin.log");
			_logger = LoggerProvider.GetLogger("con.restapi.client");
		}

		public ICommand ClearCommand { get; }

		private sealed class DelegateCommand : ICommand
		{
			private readonly Action _execute;
			public DelegateCommand(Action execute) { _execute = execute; }
			public event EventHandler CanExecuteChanged { add { } remove { } }
			public bool CanExecute(object parameter) => true;
			public void Execute(object parameter) => _execute();
		}

		public bool EnableTrace { get; set; }

		public bool EnableDebug { get; set; }

		public ObservableCollection<MessageViewModel> Messages { get; } = new ObservableCollection<MessageViewModel>();

		public void LogDebug(string message, Exception ex = null)
		{
			if (EnableDebug)
			{
				_logger.LogDebug(message, ex);
			}
		}

		public void LogError(string message, Exception ex = null)
		{
			_logger.LogError(message, ex);
		}

		public void LogEventInformation(IIdeaUserEvent userEvent, string screenName = null, Dictionary<int, string> eventCustomDimensions = null)
		{
		}

		public void LogInformation(string message, Exception ex = null)
		{
			_logger.LogInformation(message);
		}

		public void LogTrace(string message, Exception ex = null)
		{
			if (EnableTrace)
			{
				_logger.LogTrace(message, ex);
			}
		}

		public void LogWarning(string message, Exception ex = null)
		{
			_logger.LogWarning(message, ex);
		}

		private void OnClear()
		{
			Messages.Clear();
		}
	}
}