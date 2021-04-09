using System.Collections.Generic;

namespace IdeaStatiCa.BimApi
{
	public interface IIdeaModel
	{
		ISet<IIdeaMember1D> GetMembers();

		/// <summary>
		/// @param nodes
		/// @param members
		/// </summary>
		void GetSelection(out ISet<IIdeaNode> nodes, out ISet<IIdeaMember1D> members);
	}
}