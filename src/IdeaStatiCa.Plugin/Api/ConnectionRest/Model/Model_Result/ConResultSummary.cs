using IdeaRS.OpenModel.Connection;
using System.Collections.Generic;

namespace IdeaStatiCa.Plugin.Api.ConnectionRest.Model.Model_Result
{
	public class ConResultSummary
	{
		public int Id { get; set; }

		public bool Passed { get; set; }

		public ICollection<CheckResSummary> ResultSummary { get; set; }
	}
}