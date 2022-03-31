namespace IdeaStatiCa.BimApi
{
	/// <summary>
	/// Represents the line or arc segment of the element.
	/// <para>
	/// Each element has just one segment, that can be of either line or arc type. See derived interfaces <see cref="IIdeaLineSegment3D"/> and <see cref="IIdeaArcSegment3D"/>.
	/// </para>
	/// </summary>
	public interface IIdeaSegment3D : IIdeaObject
	{
		/// <summary>
		/// Start node of the segment. Returns the same value as start node of the owned element. Must not be equal to <see cref="EndNode"/>.
		/// </summary>
		IIdeaNode StartNode { get; }

		/// <summary>
		/// End node of the segment. Returns the same value as end node of the owned element. Must not be equal to <see cref="StartNode"/>.
		/// </summary>
		IIdeaNode EndNode { get; }

		/// <summary>
		/// Local Coordinate System (LCS) of the segment. Only vector definition of the LCS is supported, so the instance of <see cref="IdeaRS.OpenModel.Geometry3D.CoordSystemByVector"/> must be returned.
		/// LCS only effects rotation of the segment, it does not modify on nodes' position.
		/// </summary>
		IdeaRS.OpenModel.Geometry3D.CoordSystem LocalCoordinateSystem { get; }
	}
}