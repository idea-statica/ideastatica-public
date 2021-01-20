using System.Xml.Serialization;

namespace IdeaRS.OpenModel.Geometry3D
{
	/// <summary>
	/// Represents a segment in three-dimensional space.
	/// </summary>
	[XmlInclude(typeof(ArcSegment3D))]
	[XmlInclude(typeof(LineSegment3D))]
	public abstract class Segment3D : OpenElementId
	{
		//private ReferenceElement endPoint;

		/// <summary>
		/// Gets or sets the reference to <see cref="IdeaRS.OpenModel.Geometry3D.Point3D "/> startpoint of the segment.
		/// </summary>
		public ReferenceElement StartPoint { get; set; }

		/// <summary>
		/// Gets or sets the reference to <see cref="IdeaRS.OpenModel.Geometry3D.Point3D "/> endpoint of the segment.
		/// </summary>
		public ReferenceElement EndPoint { get; set; }

		///// <summary>
		///// Gets or sets the endpoint of the segment.
		///// </summary>
		//[XmlIgnore]
		//public Point3D EndPoint { get; set; }

		/// <summary>
		/// Local coordinate system
		/// </summary>
		public CoordSystem LocalCoordinateSystem { get; set; }

		///// <summary>
		///// serializace
		///// </summary>
		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		//[XmlElement(ElementName = "EndPoint")]
		//public ReferenceElement EndPointSerialize
		//{
		//	get
		//	{
		//		return new ReferenceElement() { Id = EndPoint.Id, TypeName = EndPoint.GetType().FullName };
		//	}

		//	set
		//	{
		//		endPoint = value;
		//	}
		//}
	}
}