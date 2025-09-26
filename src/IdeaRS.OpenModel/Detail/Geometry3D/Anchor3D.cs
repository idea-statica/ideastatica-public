using System.Xml.Serialization;

namespace IdeaRS.OpenModel.Detail
{
	public enum InstallationProcessOfAnchor : int
	{
		/// <summary>
		/// Post-Installed
		/// </summary>
		PostInstalled,

		/// <summary>
		/// Cast-in-Place
		/// </summary>
		CastInPlace
	}

	/// <summary>
	/// Anchor type
	/// </summary>
	public enum TypeOfAnchor : int
	{
		/// <summary>
		/// Adhesive
		/// </summary>
		/// <remarks>
		/// Obsolete (kept for backwards compatibility). Use type 'ThreadedRod' instead.
		/// Not necessarily Post-Installed by default anymore. There is an enum <see cref="InstallationProcessOfAnchor"/> for anchor installation process.
		/// You can set the anchor installation process using <see cref="Anchor3D.AnchorInstallationProcess"/> property.
		/// </remarks>
		Adhesive,

		/// <summary>
		/// Reinforcement
		/// </summary>
		/// <remarks>
		/// Obsolete - not necessarily Cast-in-Place anymore (the name remains the same for backwards compatibility). Use type 'Reinforcement' instead.
		/// There is an enum <see cref="InstallationProcessOfAnchor"/> for anchor installation process.
		/// You can set the anchor installation process using <see cref="Anchor3D.AnchorInstallationProcess"/> property.
		/// </remarks>
		CastInReinforcement,

		/// <summary>
		/// Reinforcement
		/// </summary>
		Reinforcement,

		/// <summary>
		/// Washer plate
		/// </summary>
		WasherPlate,

		/// <summary>
		/// Headed stud
		/// </summary>
		HeadedStud,

		/// <summary>
		/// Threaded rod
		/// </summary>
		ThreadedRod
	}

	/// <summary>
	/// Washer plate/Headed stud plate shape type
	/// </summary>
	public enum WasherPlateShape : int
	{
		/// <summary>
		/// Rectangular
		/// </summary>
		Rectangular,

		/// <summary>
		/// Circular
		/// </summary>
		Circular
	}

	/// <summary>
	/// Cast in place anchor shape type
	/// </summary>
	public enum CastInPlaceAnchorShape : int
	{
		/// <summary>
		/// Straight
		/// </summary>
		Straight,

		/// <summary>
		/// L shape
		/// </summary>
		LShape,

		/// <summary>
		/// U shape
		/// </summary>
		UShape
	}

	/// <summary>
	/// Position related to master type
	/// </summary>
	public enum PositionRelatedToMasterType : int
	{
		/// <summary>
		/// On edge
		/// </summary>
		Edge,

		/// <summary>
		/// On centre point
		/// </summary>
		CenterPoint
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
		/// Type of anchor
		/// </summary>
		public TypeOfAnchor AnchorType { get; set; }
		
		/// <summary>
		/// Installation process of anchor
		/// </summary>
		public InstallationProcessOfAnchor AnchorInstallationProcess { get; set; }

		/// <summary>
		/// Name of 3D element
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Master component
		/// </summary>
		public ReferenceElement MasterComponent { get; set; }

		/// <summary>
		/// Hanging name
		/// </summary>
		public ReferenceElement Material { get; set; }

		/// <summary>
		/// Bond strength for Adhesive anchor type only
		/// </summary>
		public double BondStrength { get; set; }

		/// <summary>
		/// Master component surface
		/// </summary>
		public int MasterSurfaceIndex { get; set; }

		/// <summary>
		/// Master component edge
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
		/// Diameter of bar
		/// </summary>
		public double Diameter { get; set; }

		/// <summary>
		/// Length of upper part of anchor
		/// </summary>
		public double LengthUp { get; set; }

		/// <summary>
		/// Length of inside part of anchor
		/// </summary>
		public double LengthDown { get; set; }

		/// <summary>
		/// Length of reinforcemetn part of anchor
		/// </summary>
		public double LengthReinfL3 { get; set; }

		/// <summary>
		/// Cast in place anchor shape
		/// </summary>
		public CastInPlaceAnchorShape CastInPlaceAnchorShapeType { get; set; }

		/// <summary>
		/// Rotation of reinforcemetn part of anchor
		/// </summary>
		public double RotationReinf { get; set; }

		/// <summary>
		/// Washer plate/Headed stud plate shape
		/// </summary>
		public WasherPlateShape WasherAndHeadStudPlateShape { get; set; }

		/// <summary>
		/// Washer plate/Headed stud plate size
		/// </summary>
		public double WasherAndHeadStudPlateSize { get; set; }

		/// <summary>
		/// Washer plate/Headed stud plate thickness
		/// </summary>
		public double WasherAndHeadStudPlateThickness { get; set; }

		/// <summary>
		/// Headed Stud head diameter
		/// </summary>
		public double HeadedStudHeadDiameter { get; set; }

		/// <summary>
		/// End type
		/// </summary>
		public LongReinfEndType EndsType { get; set; }

		/// <summary>
		/// Interconnection between plate and anchor
		/// </summary>
		public bool BasePlateInterconnect { get; set; }

		/// <summary>
		/// Transfer of shear
		/// </summary>
		public bool TransferOfShear { get; set; }

		/// <summary>
		/// Position related to edge or center point
		/// </summary>
		public PositionRelatedToMasterType PositionRelatedToMasterType { get; set; }
	}
}
