using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
		/// <param name="e">Exception to log.</param>
		void LogError(string message, object parameter);

		/// <summary>
		/// Logs info message.
		/// </summary>
		/// <param name="message">Mesage to log.</param>
		void LogInformation(string message);

		/// <summary>
		/// Logs debug message.
		/// </summary>
		/// <param name="message">Mesage to log.</param>
		void LogDebug(string message);

		/// <summary>
		/// Logs trace message.
		/// </summary>
		/// <param name="message">Mesage to log.</param>
		void LogTrace(string message);
	}
}
