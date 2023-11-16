using System.Collections.Generic;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.Identifiers;

namespace IdeaStatiCa.BimApiLink.Plugin
{
	public interface IFeaModel : IModel
	{
		FeaUserSelection GetUserSelection();
		void SelectUserSelection(IEnumerable<Identifier<IIdeaNode>> nodes, IEnumerable<Identifier<IIdeaMember1D>> members);
	}
}
