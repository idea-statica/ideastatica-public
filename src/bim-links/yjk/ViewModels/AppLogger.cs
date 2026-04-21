using CommunityToolkit.Mvvm.Input;
using IdeaStatiCa.Plugin;
using IdeaStatiCa.PluginLogger;
using IdeaStatiCa.Public;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Threading;

namespace yjk.ViewModels
{
	internal class AppLogger : IPluginLogger
	{
		private IPluginLogger _logger;

		// Public static property provides the global access point
		public static AppLogger Instance => _instance;

		// The single instance is stored in a private static field
		private static readonly AppLogger _instance = new AppLogger();

		public AppLogger()
		{
			ClearCommand = new RelayCommand(OnClear);

			SerilogFacade.Initialize("IdeaYJKPlugin.log");
			_logger = LoggerProvider.GetLogger("con.restapi.client");
		}

		public ICommand ClearCommand { get; }

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