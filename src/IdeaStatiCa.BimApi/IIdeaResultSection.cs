using IdeaRS.OpenModel.Result;
using System.Collections.Generic;

namespace IdeaStatiCa.BimApi
{
	public interface IIdeaResultSection
	{
		bool AbsoluteOrRelative { get; }

		double Position { get; }

		IEnumerable<SectionResultBase> Results { get; }
	}
}