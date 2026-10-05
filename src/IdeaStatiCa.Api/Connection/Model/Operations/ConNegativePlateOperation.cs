#nullable enable annotations

using Newtonsoft.Json;

namespace IdeaStatiCa.Api.Connection.Model.Operations
{
	/// <summary>
	/// Subtractive plate-shaped volume used to trim other operations.
	/// Mirrors <see cref="ConStiffeningPlateOperation"/> but without material/thickness/weld
	/// because a negative volume removes material rather than adding a real plate.
	/// </summary>
	public class ConNegativePlateOperation : ConOperation
	{
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public int? MaterialId { get; set; }

		public double Thickness { get; set; }

		public ConPlateShape Shape { get; set; }

		public string? PolygonData { get; set; }

		public double Width { get; set; }

		public double Height { get; set; }

		public double Width2 { get; set; }

		public double Height2 { get; set; }

		public double Radius { get; set; }

		public ConPlatePositioning? Positioning { get; set; }
	}
}
