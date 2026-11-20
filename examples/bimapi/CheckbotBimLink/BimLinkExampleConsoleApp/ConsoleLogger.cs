using IdeaStatiCa.Plugin;
using IdeaStatiCa.Public;
using System;
using System.Collections.Generic;

namespace BimLinkExampleConsoleApp
{
	/// <summary>
	/// Writes the log of the link to the console. The link logs from more than one thread, so each message is
	/// written in a single call, which keeps messages from mixing.
	/// </summary>
	internal class ConsoleLogger : IPluginLogger
	{
		public void LogError(string message, Exception ex = null)
		{
			Write("Error", message, ex);
		}

		public void LogWarning(string message, Exception ex = null)
		{
			Write("Warning", message, ex);
		}

		public void LogInformation(string message, Exception ex = null)
		{
			Write("Information", message, ex);
		}

		public void LogDebug(string message, Exception ex = null)
		{
			Write("Debug", message, ex);
		}

		/// <summary>
		/// Not written: the import writes a trace message for every object it imports.
		/// </summary>
		public void LogTrace(string message, Exception ex = null)
		{
		}

		/// <summary>
		/// Not written: usage events are statistics for IDEA StatiCa, not messages for the user.
		/// </summary>
		public void LogEventInformation(IIdeaUserEvent userEvent, string screenName = null, Dictionary<int, string> eventCustomDimensions = null)
		{
		}

		/// <summary>
		/// Writes one message with its time and severity, and the exception below it if there is one.
		/// </summary>
		private static void Write(string severity, string message, Exception ex)
		{
			string text = $"{DateTime.Now:HH:mm:ss} {severity}: {message}";
			if (ex != null)
			{
				text += Environment.NewLine + ex;
			}

			Console.WriteLine(text);
		}
	}
}
