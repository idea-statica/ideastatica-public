using System.Xml.Serialization;

namespace IdeaRS.OpenModel.Detail
{
	/// <summary>
	/// hanging - lifting anchor type
	/// </summary>
	public enum LiftingAnchorType : int
	{
		/// <summary>
		/// Unset value
		/// </summary>
		Unset,

		/// <summary>
		/// loop L1, L2
		/// </summary>
		StraightAnchorStraightLegs,

		/// <summary>
		/// loop L1, L2, L3
		/// </summary>
		StraightAnchorLShapedLegs,

		/// <summary>
		/// Loop L1, L2, angle
		/// </summary>
		AngleAnchorStraightLegs,

		/// <summary>
		/// Loop L1, L2, L3, angle
		/// </summary>
		AngleAnchorLShapedLegs,

		/// <summary>
		/// L1, L2
		/// </summary>
		StraightAnchorSingleBar
	}

	/// <summary>
	/// Representation of Anchor 3D in IDEA StatiCa Detail
	/// </summary>
	[XmlInclude(typeof(Anchor3D))]
	public class Anchor3D : OpenElementId
	{
		public Anchor3D()
			: base()
		{

		}

		/// <summary>
		/// Name of 3D element
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Master component
		/// </summary>
		public ReferenceElement MasterComponent { get; set; }

		/// <summary>
		/// hanging name
		/// </summary>
		public string MaterialName { get; set; }

		/// <summary>
		/// master component surface
		/// </summary>
		public int MasterSurfaceIndex { get; set; }

		/// <summary>
		/// master component edge
		/// </summary>
		public int MasterEdgeIndex { get; set; }

		/// <summary>
		/// Position in axis X
		/// </summary>
		public double PositionX { get; set; }

		/// <summary>
		/// Position in axis Y
		/// </summary>
		public double PositionY { get; set; }

		/// <summary>
		/// hanging as support or load
		/// </summary>
		public bool IsSupport { get; set; }

		/// <summary>
		/// diameter of bar
		/// </summary>
		public double Diameter { get; set; }

		/// <summary>
		/// hanging name
		/// </summary>
		public LiftingAnchorType AnchorType { get; set; }

		/// <summary>
		/// Length of upper part of anchor
		/// </summary>
		public double LengthUp { get; set; }

		/// <summary>
		/// Length of inside part of anchor
		/// </summary>
		public double LengthDown { get; set; }

		/// <summary>
		/// Length of legs of anchor
		/// </summary>
		public double LengthLegs { get; set; }

		/// <summary>
		/// Length of angle between legs
		/// </summary>
		public double Angle { get; set; }

		/// <summary>
		/// radius of loop
		/// </summary>
		public double Radius { get; set; }

		/// <summary>
		/// end type
		/// </summary>
		public LongReinfEndType EndsType { get; set; }

		/// <summary>
		/// edge position type
		/// </summary>
		public EdgeOrientationType EdgePositionType { get; set; }

		/// <summary>
		/// position od edge
		/// </summary>
		public double PositionOnEdge { get; set; }

		/// <summary>
		/// Position in axis Y
		/// </summary>
		public double GeometricPositionZ { get; set; }

		/// <summary>
		/// point support rotation
		/// </summary>
		public double PointSupportRotation { get; set; }

		/// <summary>
		/// reinforcement ratio
		/// </summary>
		public double ReinfRatio { get; set; }

		/// <summary>
		/// true for user diameter of mandrel
		/// </summary>
		public bool UserDiameterOfMandrel { get; set; }

		/// <summary>
		/// user diameter of mandrel value
		/// </summary>
		public double DiameterOfMandrel { get; set; }

		/// <summary>
		/// pom - pull out model - true - will be implemented for each reinforcement bars. No criteria is taken account
		/// </summary>
		public bool POM { get; set; }
	}
}
