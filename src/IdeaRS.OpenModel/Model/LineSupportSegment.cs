using IdeaRS.OpenModel.Geometry3D;

namespace IdeaRS.OpenModel.Model
{
	/// <summary>
	/// Line support on segment
	/// </summary>
	[OpenModelClass("CI.StructModel.Structure.LineSupportSegment,CI.StructuralElements", "CI.StructModel.Structure.ILineSupportSegment,CI.BasicTypes")]
	public class LineSupportSegment : OpenElementId
	{
		/// <summary>
		/// Reference to geometrical segment3D
		/// </summary>
		[OpenModelProperty("GeoSegment")]
		public ReferenceElement Segment { get; set; }

		/// <summary>
		/// Support local coord system
		/// </summary>
		public CoordSystem LocalCoordinateSystem { get; set; }

		/// <summary>
		/// Support stiffness X dirrection
		/// </summary>
		public System.Double FlexibleStiffnessX { get; set; }

		/// <summary>
		/// Support stiffness Y dirrection
		/// </summary>
		public System.Double FlexibleStiffnessY { get; set; }

		/// <summary>
		/// Support stiffness Z dirrection
		/// </summary>
		public System.Double FlexibleStiffnessZ { get; set; }

		/// <summary>
		/// Support stiffness rotational RX
		/// </summary>
		public System.Double FlexibleStiffnessRX { get; set; }

		/// <summary>
		/// Support stiffness rotational RY
		/// </summary>
		public System.Double FlexibleStiffnessRY { get; set; }

		/// <summary>
		/// Support stiffness rotational RZ
		/// </summary>
		public System.Double FlexibleStiffnessRZ { get; set; }

		/// <summary>
		/// Support type in dirrection X
		/// </summary>
		public SupportTypeInDirrection SupportTypeX { get; set; }

		/// <summary>
		/// Support type in dirrection Y
		/// </summary>
		public SupportTypeInDirrection SupportTypeY { get; set; }

		/// <summary>
		/// Support type in dirrection Z
		/// </summary>
		public SupportTypeInDirrection SupportTypeZ { get; set; }

		/// <summary>
		/// Support type in dirrection round X
		/// </summary>
		public SupportTypeInDirrection SupportTypeRX { get; set; }

		/// <summary>
		/// Support type in dirrection round Y
		/// </summary>
		public SupportTypeInDirrection SupportTypeRY { get; set; }

		/// <summary>
		/// Support type in dirrection round Z
		/// </summary>
		public SupportTypeInDirrection SupportTypeRZ { get; set; }
	}
}