using System.Collections.Generic;

namespace IdeaStatiCa.Plugin.Api.ConnectionRest.Model.Model_Connection
{
	public class ConLoadEffect : ConItem
	{
		public bool IsPercentage { get; set; }

		public IEnumerable<ConLoadEffectMemberLoad> MemberLoadings { get; set; } = new List<ConLoadEffectMemberLoad>();

		public ConLoadEffect(int id) : base(id)
		{

		}
	}
}