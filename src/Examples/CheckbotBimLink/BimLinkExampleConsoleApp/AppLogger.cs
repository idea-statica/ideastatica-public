using IdeaStatiCa.Plugin;
using IdeaStatiCa.Public;
using System.Windows.Input;

namespace BimLinkExampleConsoleApp
{
	internal class AppLogger : IPluginLogger
	{
		public AppLogger()
		{
		}

		public ICommand ClearCommand { get; }

		public bool EnableTrace { get; set; }

		public bool EnableDebug { get; set; }


		public void LogDebug(string message, Exception ex = null)
		{
			if (EnableDebug)
			{
				Log(MessageSeverity.Debug, message, ex);
			}
		}

		public void LogError(string message, Exception ex = null)
		{
			Log(MessageSeverity.Error, message, ex);
		}

		public void LogEventInformation(IIdeaUserEvent userEvent, string screenName = null, Dictionary<int, string> eventCustomDimensions = null)
		{
		}

		public void LogInformation(string message, Exception ex = null)
		{
			Log(MessageSeverity.Information, message, ex);
		}

		public void LogTrace(string message, Exception ex = null)
		{
			if (EnableTrace)
			{
				Log(MessageSeverity.Trace, message, ex);
			}
		}

		public void LogWarning(string message, Exception ex = null)
		{
			Log(MessageSeverity.Warning, message, ex);
		}

		private void Log(MessageSeverity severity, string message, Exception ex)
		{
			//Yours Logs
		}

	}
}