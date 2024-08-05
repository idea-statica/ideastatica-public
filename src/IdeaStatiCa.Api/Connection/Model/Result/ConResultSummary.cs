using IdeaRS.OpenModel.Connection;
using System.Collections.Generic;

namespace IdeaStatiCa.Api.Connection.Model
{
	public class ConResultSummary
	{
		public int Id { get; set; }

		public bool Passed { get; set; }

		public List<CheckResSummary> ResultSummary { get; set; }
	}
}