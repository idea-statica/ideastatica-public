
using System;
using System.Threading.Tasks;

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
	public interface IAutomation
	{
		/// <summary>
		/// Open the project <paramref name="fileName"/>
		/// </summary>
		/// <param name="fileName">Project to open</param>
		/// <param name="optionalParamJson">json string which can include optional parameters for a module</param>
		/// <returns>Task</returns>
		
		Task OpenProjectAsync(string fileName, string optionalParamJson);

		/// <summary>
		/// Open the project <paramref name="fileName"/>
		/// </summary>
		/// <param name="fileName">Project to open</param>
		/// <param name="optionalParamJson">json string which can include optional parameters for a module</param>
		
		void OpenProject(string fileName, string optionalParamJson);

		/// <summary>
		/// Select item with <paramref name="itemId"/> in the project
		/// </summary>
		/// <param name="itemId">The identifier of the requested item</param>
		
		void SelectItem(string itemId);

		/// <summary>
		/// Refresh the currently open project
		/// </summary>
		
		Task RefreshProjectAsync();

		/// <summary>
		/// Refresh the currently open project
		/// </summary>
		
		void RefreshProject();

		/// <summary>
		/// Close the project
		/// </summary>
		
		void CloseProject();

		/// <summary>
		/// Close the project
		/// </summary>
		
		Task CloseProjectAsync();

		
		void Shutdown();

		
		Task RefreshAsync();


		
		void Refresh();

		void NotifyChange();

		string TempWorkingDir
		{
			
			get;
		}

		string ProjectDir
		{
			
			get;
		}

		AutomationStatus Status { get; }
	}
}