using System;

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
	}
}
