using System.Collections.Generic;
using IdeaRS.OpenModel.Concrete.CheckResult;
using IdeaRS.OpenModel.Message;

namespace IdeaStatiCa.Plugin.Api.RCS.Model
{
	public class RcsDetailedResultForSection
	{
		public int Id { get; set; }

		public SectionConcreteCheckResult SectionResult { get; set; }

		public List<NonConformityIssue> Issues { get; set; } = new List<NonConformityIssue>();
	}
}
