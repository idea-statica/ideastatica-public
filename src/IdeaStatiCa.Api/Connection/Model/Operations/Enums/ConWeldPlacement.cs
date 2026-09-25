namespace IdeaStatiCa.Api.Connection.Model.Operations
{
	/// <summary>
	/// How a Weld or Contact operation joins its two operands.
	/// </summary>
	public enum ConWeldPlacement
	{
		/// <summary>An edge of the first plate is joined to the surface of the second plate.</summary>
		EdgeToSurface,

		/// <summary>An edge of the first plate is joined to an edge of the second plate.</summary>
		EdgeToEdge,
	}
}
