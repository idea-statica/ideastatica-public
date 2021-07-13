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
		[OperationContract]
		void OpenProject(string fileName);

		/// <summary>
		/// Select item with <paramref name="itemId"/> in the project
		/// </summary>
		/// <param name="itemId">The identifier of the requested item</param>
		[OperationContract]
		void SelectItem(string itemId);

		/// <summary>
		/// Refresh the currently open project
		/// </summary>
		[OperationContract]
		void RefreshProject();

		/// <summary>
		/// Close the
		/// </summary>
		[OperationContract]
		void CloseProject();

		[OperationContract]
		void Shutdown();

		[OperationContract]
		void Refresh();

		void NotifyChange();

		string TempWorkingDir
		{
			[OperationContract]
			get;
		}

		string ProjectDir
		{
			[OperationContract]
			get;
		}

		AutomationStatus Status { get; }
	}
}