using IdeaStatiCa.Public;
using System;
using System.Collections.Generic;

namespace IdeaStatiCa.Plugin
{
	/// <summary>
	/// Logger that does nothing.
	/// </summary>
	public class NullLogger : IPluginLogger
	{
		public void LogDebug(string message, Exception ex = null)
		{

		}

		public void LogError(string message, Exception ex = null)
		{

		}

		public void LogEventInformation(IIdeaUserEvent userEvent, string screenName = null, Dictionary<int, string> eventCustomDimensions = null)
		{
		}

		public void LogInformation(string message, Exception ex = null)
		{

		}

		public void LogTrace(string message, Exception ex = null)
		{

		}

		public void LogWarning(string message, Exception ex = null)
		{

		}
	}
}
