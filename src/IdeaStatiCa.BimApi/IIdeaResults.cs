using System.Collections.Generic;

namespace IdeaStatiCa.BimApi
{
	public interface IIdeaResults
	{
		List<IIdeaMemberResult> Members { get; }
	}
}