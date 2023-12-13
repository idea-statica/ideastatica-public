using System.Collections.Generic;
using IdeaRS.OpenModel.Concrete.CheckResult;

namespace IdeaStatiCa.Plugin.Api.RCS.Model
{
	public class RcsSectionResultOverview
	{
		public int SectionId { get; set; }

		public List<ConcreteCheckResultOverallItem> OverallItems { get; set; }

		public RcsSectionResultOverview()
		{
			OverallItems = new List<ConcreteCheckResultOverallItem>();
		}
	}
}
