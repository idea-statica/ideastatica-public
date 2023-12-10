using System.Collections.Generic;

namespace IdeaStatiCa.Plugin.Api.RCS.Model
{
	/// <summary>
	/// Required sections in a RCS project an types of checks to calculate
	/// </summary>
	public class RcsCalculationParameters
	{
		public IEnumerable<int> Sections { get; set; } = new List<int>();
	}
}
