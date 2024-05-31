using System.Collections.Generic;

namespace IdeaStatiCa.Plugin.Api.ConnectionRest.Model.Model_Connection
{
	public class ConCalculationParameter
	{
		public List<int> ConnectionIds { get; set; }
		public bool CalculateBuckling { get; set; }
	}
}