
namespace CI.Geometry3D
{
	/// <summary>
	/// Determine the object position on node/segment/region
	/// </summary>
	public enum IntersectionResults
	{
		/// <summary>
		/// UnDefined
		/// </summary>
		Undefined = 0,

		/// <summary>
		/// Object Lies outside the region
		/// </summary>
		Outside = 1,

		/// <summary>
		/// Object Lies inside the region
		/// </summary>
		Inside = 2,

		/// <summary>
		/// Object Lies on segment
		/// </summary>
		OnBorderCurve = 4,

		/// <summary>
		/// Object lies on node
		/// </summary>
		OnBorderNode = 8,

		/// <summary>
		/// Object Lies inside Opening
		/// </summary>
		OnInSideOpening = 16,
	}

	/// <summary>
	/// Specifies the position of the segment to the plane.
	/// </summary>
	public enum PlaneIntersectionResult
	{
		/// <summary>
		/// The segment starts outside the plane and is parallel to the plane, there is no intersection.
		/// </summary>
		Parallel,

		/// <summary>
		/// The segment starts inside the plane and is parallel to the plane, the segment intersects the plane everywhere.
		/// </summary>
		InPlane,

		/// <summary>
		/// The segment intersects the plane at least once and this point is inside defined segment.
		/// </summary>
		Inside,

		/// <summary>
		/// The segment intersects the plane at least once and all these points ara outside defined segment.
		/// </summary>
		Outside,
	}
}
