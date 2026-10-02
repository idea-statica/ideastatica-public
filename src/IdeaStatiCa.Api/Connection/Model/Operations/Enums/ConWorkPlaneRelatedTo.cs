namespace IdeaStatiCa.Api.Connection.Model.Operations
{
	/// <summary>
	/// Coordinate system the work plane is related to.
	/// </summary>
	public enum ConWorkPlaneRelatedTo
	{
		/// <summary>
		/// Work plane is positioned relative to the connection joint (global).
		/// </summary>
		Joint = 0,

		/// <summary>
		/// Work plane is positioned relative to a specific member's local coordinate system.
		/// </summary>
		Member = 1,

		/// <summary>
		/// Work plane is positioned relative to a specific plate.
		/// </summary>
		Plate = 2,
	}
}
