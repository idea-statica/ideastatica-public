using IdeaStatiCa.Public;
using System;
using System.Collections.Generic;

namespace IdeaStatiCa.Plugin
{
	/// <summary>
	/// Default logging interface. 
	/// </summary>
	public interface IPluginLogger
	{
		/// <summary>
		/// Logs an error message.
		/// </summary>
		/// <param name="message">Message to log.</param>
		/// <param name="ex">Optional exception to log.</param>
		void LogError(string message, Exception ex = null);

		/// <summary>
		/// Logs a warning message.
		/// </summary>
		/// <param name="message">Message to log.</param>
		/// <param name="ex">Optional exception to log.</param>
		void LogWarning(string message, Exception ex = null);

		/// <summary>
		/// Logs info message.
		/// </summary>
		/// <param name="message">Mesage to log.</param>
		/// <param name="ex">Optional exception to log.</param>
		void LogInformation(string message, Exception ex = null);

		/// <summary>
		/// Logs debug message.
		/// </summary>
		/// <param name="message">Mesage to log.</param>
		/// <param name="ex">Optional exception to log.</param>
		void LogDebug(string message, Exception ex = null);

		/// <summary>
		/// Logs trace message.
		/// </summary>
		/// <param name="message">Mesage to log.</param>
		/// <param name="ex">Optional exception to log.</param>
		void LogTrace(string message, Exception ex = null);

		/// <summary>
		/// Logs the user activity (event) and reports it to Sentry, File log and Google Analytic as Event, if the logger is configured to the Information or lover logging severity.
		/// </summary>
		/// <param name="userEvent">Specifies the parameters of the event to be reported. Must not be null.</param>
		/// <param name="screenName">Optional: Screen name. Use the same value as to <see cref="LogScreenViewInformation()"/>. Can be null, if the event is not related to a screen.</param>
		/// <param name="eventCustomDimensions">Optional dictionary of the additional event-related custom dimensions to be reported to the cdX parameters.</param>
		void LogEventInformation(IIdeaUserEvent userEvent, string screenName = null, Dictionary<int, string> eventCustomDimensions = null);
	}
}
