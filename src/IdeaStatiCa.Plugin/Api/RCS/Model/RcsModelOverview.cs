using System.Collections.Generic;

namespace IdeaStatiCa.Plugin.Api.RCS.Model
{
	public class RcsModelOverview
	{
		public RcsProjectData ProjectData { get; set; }
		public List<RcsCrossSectionOverviewModel> Sections { get; set; }
		public List<RcsCheckMemberModel> CheckMembers { get; set; }
		public List<RcsReinforcedCrossSectionModel> ReinforcedCrossSections { get; set; }

		public RcsModelOverview()
		{
			Sections = new List<RcsCrossSectionOverviewModel>();
			CheckMembers = new List<RcsCheckMemberModel>();
			ReinforcedCrossSections = new List<RcsReinforcedCrossSectionModel>();
		}
	}
}
