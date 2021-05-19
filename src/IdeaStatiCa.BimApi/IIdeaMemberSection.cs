using IdeaRS.OpenModel.Result;
using System.Collections.Generic;

namespace IdeaStatiCa.BimApi
{
	public interface IIdeaMemberSection
	{
		bool AbsoluteOrRelative { get; }
		double Position { get; }
		List<SectionResultBase> Results { get; }
	}
}