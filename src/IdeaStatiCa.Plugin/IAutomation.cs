using System;
using System.ServiceModel;

namespace IdeaStatiCa.Plugin
{
	[Flags]
	public enum AutomationStatus
	{
		Unknown = 0,
		IsClient = 1
	}

	/// <summary>
	/// This interface is used for controlling Idea Statica application from external applictions.
	/// </summary>
	[ServiceContract]
	public interface IAutomation
	{
		/// <summary>
		/// Open <paramref name="fileName"/>
		/// </summary>
		/// <param name="fileName">Project to open</param>
		/// <returns>Returns 'true' if success</returns>
		[OperationContract]
		string OpenProject(string fileName);

		/// <summary>
		/// Select item with <paramref name="itemId"/> in the project
		/// </summary>
		/// <param name="itemId">The identifier of the requested item</param>
		/// <returns>Returns 1 if success</returns>
		[OperationContract]
		int SelectItem(string itemId);

		/// <summary>
		/// Refresh the currently open project
		/// </summary>
		/// <returns>Returns 1 if success</returns>
		[OperationContract]
		int RefreshProject();

		/// <summary>
		/// Close the project
		/// </summary>
		/// <returns>Returns 1 if success</returns>
		[OperationContract]
		int CloseProject();

		[OperationContract]
		int Shutdown();

		[OperationContract]
		int Refresh();

		int NotifyChange();

		string GetTempWorkingDir();

		string GetProjectDir();
		
		AutomationStatus GetStatus();
	}
}