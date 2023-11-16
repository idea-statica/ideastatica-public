using System.Collections.Generic;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.Identifiers;

namespace IdeaStatiCa.BimApiLink.Plugin
{
	public interface IFeaModel : IModel
	{
		FeaUserSelection GetUserSelection();

		/// <summary>
		/// Get's the user selected nodes and members as input parameters and projects the selection in connected Fea application
		/// </summary>
		/// <param name="nodes">Nodes to be selected</param>
		/// <param name="members">Members to be selected</param>
		void SelectUserSelection(IEnumerable<Identifier<IIdeaNode>> nodes, IEnumerable<Identifier<IIdeaMember1D>> members);
	}
}
