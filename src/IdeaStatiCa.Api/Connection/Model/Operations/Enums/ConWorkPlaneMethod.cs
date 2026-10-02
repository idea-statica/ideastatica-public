namespace IdeaStatiCa.Api.Connection.Model.Operations
{
	/// <summary>
	/// How the work plane is defined.
	/// </summary>
	public enum ConWorkPlaneMethod
	{
		/// <summary>
		/// Defined by rotations X / Y / Z around the origin.
		/// </summary>
		ByAngles = 0,

		/// <summary>
		/// Defined by origin and a normal vector.
		/// </summary>
		ByNormalVector = 1,

		/// <summary>
		/// Defined by intersection of two members.
		/// </summary>
		ByIntersection = 2,
	}
}
