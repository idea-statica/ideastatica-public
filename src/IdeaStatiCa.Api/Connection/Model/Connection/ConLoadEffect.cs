using Newtonsoft.Json;
using System.Collections.Generic;

namespace IdeaStatiCa.Api.Connection.Model
{
	public class ConLoadEffect : ConItem
	{
		public ConLoadEffect() : base()
		{
		}

		[JsonConstructor]
		public ConLoadEffect(int id) : base(id)
		{
		}

		public bool IsPercentage { get; set; }

		public List<ConLoadEffectMemberLoad> MemberLoadings { get; set; } = new List<ConLoadEffectMemberLoad>();
	}
}