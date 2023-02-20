namespace IdeaStatiCa.BimApi
{
	/// <summary>
	/// An plate is a part of a connection.
	/// </summary>
	public interface IIdeaPlate : IIdeaObjectConnectable
	{
		double Thickness { get; set; }

		/// <summary>
		/// Material of the plate.
		/// </summary>
		IIdeaMaterial Material { get; }

		/// <summary>
		/// Origin node of the plate.
		/// </summary>
		IIdeaNode Origin { get; }

		/// <summary>
		/// Local Coordinate System (LCS) of the plate. Only vector definition of the LCS is supported, so the instance of <see cref="IdeaRS.OpenModel.Geometry3D.CoordSystemByVector"/> must be returned.
		/// LCS only effects rotation of the segment, it does not modify on nodes' position.
		/// </summary>
		IdeaRS.OpenModel.Geometry3D.CoordSystem LocalCoordinateSystem { get; }

		/// <summary>
		/// Geometry of the component.
		/// </summary>
		IdeaRS.OpenModel.Geometry2D.Region2D Geometry { get; }
	}
}
