using System;

namespace IdeaStatiCa.Plugin
{
	/// <summary>
	/// Provides opening of project with UI (opens IDEA StatiCa Connection application).
	/// </summary>
	public interface IConnectionController
	{
		/// <summary>
		/// Fires when connection application exited.
		/// </summary>
		event EventHandler ConnectionAppExited;

		/// <summary>
		/// Indicates whether communication (open/close project) is alive.
		/// </summary>
		bool IsConnected { get; }

		/// <summary>
		/// Opens connection file project, provided service is connected.
		/// </summary>
		/// <param name="fileName">The file path to the ideaCon.</param>
		/// <returns></returns>
		int OpenProject(string fileName);

		/// <summary>
		/// Close connection file project, provided service is connected and connection file is opened.
		/// </summary>
		/// <returns></returns>
		int CloseProject();
	}
}
