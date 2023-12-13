using System.Collections.Generic;

namespace IdeaStatiCa.Plugin.Api.RCS.Model
{
	public class RcsResultParameters
	{
		public IEnumerable<int> Sections { get; set; } = new List<int>();
	}
}
