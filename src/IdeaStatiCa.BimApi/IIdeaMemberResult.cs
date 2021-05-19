using IdeaRS.OpenModel.Result;
using System.Collections.Generic;

namespace IdeaStatiCa.BimApi
{
	public interface IIdeaMemberResult
	{
		IIdeaObject Member { get; }
		ResultType Type { get; }
		ResultLocalSystemType CoordinateSystemType { get; }
		List<IIdeaMemberSection> Sections { get; }
	}
}