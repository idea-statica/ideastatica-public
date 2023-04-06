using IdeaRS.OpenModel;
using System.Collections.Generic;

namespace IdeaStatiCa.BimApi
{
	/// <summary>
	/// Model for project to import.
	/// </summary>
	public interface IIdeaModel
	{
		/// <summary>
		/// Returns a set of all members in the model.
		/// </summary>
		/// <returns>Set of all members in the model.</returns>
		ISet<IIdeaMember1D> GetMembers();

		/// <summary>
		/// Returns nodes and/or members from whole model. At least one of the sets must be non-empty
		/// unless the user did not select anything. Set of returned members must be a
		/// subset of <see cref="GetMembers"/>.
		/// </summary>
		BulkSelection GetWholeModel();

		/// <summary>
		/// Returns currently selected nodes and/or members. At least one of the sets must be non-empty
		/// unless the user did not select anything. Set of returned members must be a
		/// subset of <see cref="GetMembers"/>.
		/// </summary>
		BulkSelection GetBulkSelection();

		/// <summary>
		/// Returns currently selected node and/or member. At least one of the sets must be non-empty
		/// unless the user did not select anything.Set of returned members must be a
		/// subset of <see cref="GetMembers"/>.
		/// </summary>
		SingleSelection GetSingleSelection();

		/// <summary>
		/// Returns information from the original application about the project this class refers to.
		/// </summary>
		/// <returns>Project information</returns>
		OriginSettings GetOriginSettings();

		/// <summary>
		/// Returns a set of all loads in the model.
		/// </summary>
		/// <returns>Set of all loads in the model.</returns>
		ISet<IIdeaLoading> GetLoads();
	}
}