using IdeaRS.OpenModel.Result;
using System.Collections.Generic;

namespace IdeaStatiCa.BimApi.Results
{
	public interface IIdeaResult
	{
		ResultType Type { get; }

		ResultLocalSystemType CoordinateSystemType { get; }

		IEnumerable<IIdeaSection> Sections { get; }
	}
}