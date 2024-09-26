using System.Collections.Generic;

namespace IdeaStatiCa.Api.RCS.Model
{
	public class RcsProjectSummary
	{
		public List<RcsSection> Sections { get; set; }
		public List<RcsCheckMember> CheckMembers { get; set; }
		public List<RcsReinforcedCrossSection> ReinforcedCrossSections { get; set; }

		public RcsProjectSummary()
		{
			Sections = new List<RcsSection>();
			CheckMembers = new List<RcsCheckMember>();
			ReinforcedCrossSections = new List<RcsReinforcedCrossSection>();
		}
	}
}
