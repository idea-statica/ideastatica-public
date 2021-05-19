using IdeaRS.OpenModel.Result;
using System.Collections.Generic;

namespace IdeaStatiCa.BimApi
{
	public interface IIdeaResult
	{
		ResultType Type { get; }

		ResultLocalSystemType CoordinateSystemType { get; }

		List<IIdeaResultSection> Sections { get; }
	}
}