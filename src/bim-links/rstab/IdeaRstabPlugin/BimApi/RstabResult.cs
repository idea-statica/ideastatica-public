using IdeaRS.OpenModel.Result;
using IdeaStatiCa.BimApi.Results;
using System.Collections.Generic;

namespace IdeaRstabPlugin.BimApi
{
	internal class RstabResult : IIdeaResult
	{
		public ResultLocalSystemType CoordinateSystemType { get; set; }

		public IEnumerable<IIdeaSection> Sections { get; set; }
	}
}