using IdeaStatiCa.BimApi.Results;
using System.Collections.Generic;

namespace IdeaStatiCa.RamToIdea.BimApi
{
	internal class RamSection : IIdeaSection
	{
		public double Position { get; set; }

		public IEnumerable<IIdeaSectionResult> Results { get; set; }
	}
}