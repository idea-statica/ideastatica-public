using IdeaStatiCa.Plugin;
using System;
using System.Collections.ObjectModel;
using System.Windows.Threading;

namespace BimApiExample.ViewModels
{
	internal class AppLogger : IPluginLogger
	{
		private readonly Dispatcher uiDispatcher;

		public AppLogger(Dispatcher uiDispatcher)
		{
			this.uiDispatcher = uiDispatcher;
		}

		public bool EnableTrace { get; set; }

		public bool EnableDebug { get; set; }

		public ObservableCollection<MessageViewModel> Messages { get; } = new();

		public void LogDebug(string message, Exception? ex = null)
		{
			if (EnableDebug)
			{
				Log(MessageSeverity.Debug, message, ex);
			}
		}

		public void LogError(string message, Exception? ex = null)
		{
			Log(MessageSeverity.Error, message, ex);
		}

		public void LogInformation(string message, Exception? ex = null)
		{
			Log(MessageSeverity.Information, message, ex);
		}

		public void LogTrace(string message, Exception? ex = null)
		{
			if (EnableTrace)
			{
				Log(MessageSeverity.Trace, message, ex);
			}
		}

		public void LogWarning(string message, Exception? ex = null)
		{
			Log(MessageSeverity.Warning, message, ex);
		}

		private void Log(MessageSeverity severity, string message, Exception? ex)
		{
			uiDispatcher.BeginInvoke(new Action(() =>
			{
				Messages.Add(new(severity, message, ex));
			}));
		}
	}
}