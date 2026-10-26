using IdeaStatiCa.BimApi.Results;
using System.Collections.Generic;

namespace IdeaRstabPlugin.BimApi
{
	internal class RstabSection : IIdeaSection
	{
		public double Position { get; set; }

		public IEnumerable<IIdeaSectionResult> Results { get; set; }
	}
}