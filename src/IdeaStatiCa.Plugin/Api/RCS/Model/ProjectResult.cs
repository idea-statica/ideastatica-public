using System.Collections.Generic;
using IdeaRS.OpenModel.Concrete.CheckResult;
using IdeaRS.OpenModel.Message;

namespace IdeaStatiCa.Plugin.Api.RCS.Model
{
	public class ProjectResult
	{
		public List<SectionConcreteCheckResult> Sections { get; set; } = new List<SectionConcreteCheckResult>();

		public List<NonConformityIssue> Issues { get; set; } = new List<NonConformityIssue>();
	}
}
