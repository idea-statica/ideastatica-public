#nullable enable annotations

using Newtonsoft.Json;

namespace IdeaStatiCa.Api.Connection.Model.Operations
{
	public class ConStiffeningPlateOperation : ConOperation
	{
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public int? PlateDataId { get; set; }

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

		public ConStiffeningPlateType PlateType { get; set; }

		public ConPlatePositioning? Positioning { get; set; }

		/// <summary>
		/// Weld attaching the plate to the part it is positioned on. Only <see cref="ConPlatePositioningEnum.Member"/>
		/// and <see cref="ConPlatePositioningEnum.PlateOnPlate"/> positioning give the operation a part to weld to,
		/// and a negative volume is never welded; asking for a weld in any other case is rejected - including with
		/// no positioning stated, which places the plate at the joint. To weld a plate positioned by joint, member
		/// LCS, coordinate system or concrete block, add a separate weld operation.
		/// </summary>
		public ConWeldData? Weld { get; set; }
	}
}
