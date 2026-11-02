using System.Runtime.Serialization;
namespace IdeaRS.OpenModel.Geometry3D
{
	/// <summary>
	/// Represents an x- , y- an z-coordinates in three-dimensional space.
	/// </summary>
	[OpenModelClass("CI.Geometry3D.Point3D,CI.Geometry3D", "CI.Geometry3D.IPoint3D,CI.BasicTypes")]
	[DataContract]
	public class Point3D : OpenElementId
	{
		/// <summary>
		/// Gets or sets the Name
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the X-coordinate value
		/// </summary>
		[DataMember]
		public double X { get; set; }

		/// <summary>
		/// Gets or sets the Y-coordinate value
		/// </summary>
		[DataMember]
		public double Y { get; set; }

		/// <summary>
		/// Gets or sets the Z-coordinate value
		/// </summary>
		[DataMember]
		public double Z { get; set; }
	}
}