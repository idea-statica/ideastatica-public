using IdeaRS.OpenModel;
using IdeaStatica.BimApiLink.Identifiers;
using IdeaStatiCa.BimApi;
using System.Collections.Generic;

namespace IdeaStatica.BimApiLink.Plugin
{
	public interface IModel
	{
		IEnumerable<Identifier<IIdeaMember1D>> GetAllMembers();

		OriginSettings GetOriginSettings();
	}
}
