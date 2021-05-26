using System.Collections.Generic;

namespace IdeaStatiCa.BimApi.Results
{
	public interface IIdeaSection
	{
		double Position { get; }

		IEnumerable<IIdeaSectionResult> Results { get; }
	}
}