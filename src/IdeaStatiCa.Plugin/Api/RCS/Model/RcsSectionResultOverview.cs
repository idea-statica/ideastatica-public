using System.Collections.Generic;
using IdeaRS.OpenModel.Concrete.CheckResult;
using IdeaRS.OpenModel.Message;

namespace IdeaStatiCa.Plugin.Api.RCS.Model
{
	public class RcsSectionResultOverview
	{
		public int SectionId { get; set; }

		public List<ConcreteCheckResultOverallItem> OverallItems { get; set; }

		public List<NonConformityIssue> NonConformityIssues { get; set; }

		public RcsSectionResultOverview()
		{
			OverallItems = new List<ConcreteCheckResultOverallItem>();
			NonConformityIssues = new List<NonConformityIssue>();
		}
	}
}
