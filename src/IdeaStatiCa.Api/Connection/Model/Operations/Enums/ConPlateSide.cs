namespace IdeaStatiCa.Api.Connection.Model.Operations
{
	/// <summary>
	/// Side of the plate the foundation block / contact is placed on.
	/// </summary>
	public enum ConPlateSide
	{
		LowerSide,
		UpperSide,
		BothSides,
		Center,

		/// <summary>
		/// The side is not prescribed - the connection engine decides which part remains.
		/// Reported for an imported plate cut, which does not force a side.
		/// </summary>
		/// <remarks>
		/// Must stay last and in lockstep with <c>PlateSide</c>: the two enums are mapped by an ordinal
		/// cast (<c>FastenerGridMappingProfile</c>), so a member missing here surfaces as an undefined
		/// value in the DTO and in the generated OpenAPI schema.
		/// </remarks>
		Default,
	}
}
