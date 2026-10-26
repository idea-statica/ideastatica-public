using IdeaStatiCa.BimApi;
using System.Collections.Generic;

namespace IdeaRstabPlugin.Factories
{
	internal interface IElementFactory
	{
		List<IIdeaElement1D> GetElements(int memberNo);
	}
}