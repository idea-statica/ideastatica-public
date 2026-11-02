#nullable enable annotations

using Newtonsoft.Json;

namespace IdeaStatiCa.Api.Connection.Model.Operations
{
	/// <summary>
	/// Subtractive member-shaped volume used to trim other operations.
	/// Carries an existing cross-section reference + the segment lengths along
	/// the member axis; mirrors the shape of an Added Member but flagged as
	/// negative (the converter routes it to <c>NegativeVolumeMemberOperation</c>).
	/// </summary>
	public class ConNegativeMemberOperation : ConOperation
	{
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public int? CrossSectionId { get; set; }

		public double Length1 { get; set; }

		public double Length2 { get; set; }

		public bool MirrorY { get; set; }

		public bool MirrorZ { get; set; }

		public ConAddedMemberPositioning? Positioning { get; set; }
	}
}
