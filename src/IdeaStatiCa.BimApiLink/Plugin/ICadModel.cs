using System.Collections.Generic;

namespace IdeaStatiCa.BimApiLink.Plugin
{
	public interface ICadModel : IModel
	{
		/// <summary>
		/// Get selection represent single connection
		/// </summary>
		/// <returns></returns>
		CadUserSelection GetUserSelection();

		/// <summary>
		/// Get bulk selection represent multiple connections
		/// </summary>
		/// <returns></returns>
		IEnumerable<CadUserSelection> GetUserSelections();


		/// <summary>
		/// Get selection represent bulk selection of all entities in model
		/// </summary>
		/// <returns></returns>
		IEnumerable<CadUserSelection> GetSelectionOfWholeModel();

	}
}
