using IdeaRS.OpenModel;
using IdeaStatiCa.BimApiLink.Identifiers;
using IdeaStatiCa.BimApi;
using System.Collections.Generic;

namespace IdeaStatiCa.BimApiLink.Plugin
{
	public interface IModel
	{
		IEnumerable<Identifier<IIdeaMember1D>> GetAllMembers();

		OriginSettings GetOriginSettings();
	}
}
