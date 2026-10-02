namespace IdeaStatiCa.Api.Connection.Model.Operations
{
	/// <summary>
	/// Grid layout type for the fastener positions.
	/// </summary>
	public enum ConGridGeometryType
	{
		/// <summary>Rectangular grid defined by top/bottom/left/right layer lists.</summary>
		Orthogonal,

		/// <summary>Polar grid defined by radii plus counts or explicit angles.</summary>
		Polar,
	}
}
