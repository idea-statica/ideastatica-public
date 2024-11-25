using System;
using System.Collections.Generic;

namespace IdeaStatiCa.Api.RCS.Model
{
	/// <summary>
	/// Class for Rcs.Rest.API
	/// </summary>
	public class RcsProject
	{
		public Guid ProjectId { get; set; }
		public RcsProjectData ProjectData { get; set; }

		public List<RcsSection> Sections = new List<RcsSection>();
		public List<RcsCheckMember> CheckMembers = new List<RcsCheckMember>();
		public List<RcsReinforcedCrossSection> ReinforcedCrossSections = new List<RcsReinforcedCrossSection>();
	}
}
