using IdeaStatiCa.BimApi;
using System.Collections.Generic;

namespace IdeaStatiCa.RamToIdea.Factories
{
	internal interface IElementFactory
	{
		List<IIdeaElement1D> GetElements(int memberNo);
	}
}
