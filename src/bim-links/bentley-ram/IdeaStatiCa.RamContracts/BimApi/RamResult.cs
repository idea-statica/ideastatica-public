using IdeaRS.OpenModel.Result;
using IdeaStatiCa.BimApi.Results;
using System.Collections.Generic;

namespace IdeaStatiCa.RamToIdea.BimApi
{
	public class RamResult : IIdeaResult
	{
		public ResultLocalSystemType CoordinateSystemType { get; set; }

		public IEnumerable<IIdeaSection> Sections { get; set; }
	}
}