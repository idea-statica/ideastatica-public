using System.Collections.Generic;

namespace IdeaStatiCa.Plugin.Api.RCS.Model
{
	public class RcsProjectSummaryModel
	{
		public List<RcsSectionModel> Sections { get; set; }
		public List<RcsCheckMemberModel> CheckMembers { get; set; }
		public List<ReinforcedCrossSectionModel> ReinforcedCrossSections { get; set; }

		public RcsProjectSummaryModel()
		{
			Sections = new List<RcsSectionModel>();
			CheckMembers = new List<RcsCheckMemberModel>();
			ReinforcedCrossSections = new List<ReinforcedCrossSectionModel>();
		}
	}
}
