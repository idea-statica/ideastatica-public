using IdeaRS.OpenModel.Connection;
using IdeaStatiCa.Public;
using System.Threading.Tasks;


namespace IdeaStatiCa.Plugin
{
	/// <summary>
	///
	/// </summary>
	public interface IConnectionAutomation
	{
		/// <summary>
		///
		/// </summary>
		/// <returns></returns>
		ConProjectInfo GetProjectInfo();

		/// <summary>
		/// Returns whether the currently open connection has unsaved changes.
		/// </summary>
		bool GetIsEdited();

		/// <summary>
		/// Saves the currently open project without closing it.
		/// Equivalent to clicking the Save button in the Connection application.
		/// Does nothing if there is no open project.
		/// </summary>
		Task SaveCurrentItemAsync();
	}
}
