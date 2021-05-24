using System.Collections.Generic;

namespace IdeaStatiCa.BimApi.Results
{
	public interface IIdeaSection
	{
		bool AbsoluteOrRelative { get; }

		double Position { get; }

		IEnumerable<IIdeaSectionResult> Results { get; }
	}
}