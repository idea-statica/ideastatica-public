#nullable enable annotations

using Newtonsoft.Json;

namespace IdeaStatiCa.Api.Connection.Model.Operations
{
	public class ConStiffeningMemberOperation : ConOperation
	{
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public int? CrossSectionId { get; set; }

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public int? MaterialId { get; set; }

		public double Length1 { get; set; }

		public double Length2 { get; set; }

		public bool MirrorY { get; set; }

		public bool MirrorZ { get; set; }

		public ConAddedMemberPositioning? Positioning { get; set; }

		public ConWeldData? Weld { get; set; }
	}
}
