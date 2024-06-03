using System.Collections.Generic;

namespace IdeaStatiCa.Plugin.Api.ConnectionRest.Model.Model_Connection
{
	public class ConLoadEffect : ConItem
	{
		public bool IsPercentage { get; set; }

		//Maybe we should only provide one list here and retrieve based on the current connection setting.
		public IEnumerable<ConLoadEffectMemberLoad> MemberLoadingsForce { get; set; } = new List<ConLoadEffectMemberLoad>();

		public IEnumerable<ConLoadEffectMemberLoad> MemberLoadingsPercentage { get; set; } = new List<ConLoadEffectMemberLoad>();
	}
}