using IdeaRS.OpenModel.Geometry2D;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace IdeaRS.OpenModel.Detail
{
	/// <summary>
	/// type of Base plate
	/// </summary>
	public enum BasePlateLoadType : int
	{
		Column = 1,
		Forces = 2,
	}

	/// <summary>
	/// shear force transfer
	/// </summary>
	public enum ShearForceTransfer
	{
		Friction = 1,
		ShearLug = 2,
		Anchors = 3
	}

	/// <summary>
	/// Representation of Base Plate 3D in IDEA StatiCa Detail
	/// </summary>
	[XmlInclude(typeof(BasePlate3D))]
	public class BasePlate3D : OpenElementId
	{
		public BasePlate3D()
			: base()
		{
		}

		/// <summary>
		/// Gets or sets the code of the base plate block:
		/// 1 - general
		/// 2 - rectangle
		/// </summary>
		public SolidShapeCode ShapeCode { get; set; }

		/// <summary>
		/// Name of 3D element
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Material
		/// </summary>
		public ReferenceElement Material { get; set; }

		/// <summary>
		/// Gets, sets cross section located on this base plate
		/// </summary>
		public string CrossSectionName { get; set; }

		/// <summary>
		/// load position X
		/// </summary>
		public double LoadPositionX { get; set; }

		/// <summary>
		/// load position Y
		/// </summary>
		public double LoadPositionY { get; set; }

		/// <summary>
		/// load length
		/// </summary>
		public double LoadLength { get; set; }

		/// <summary>
		/// Width of base plate
		/// </summary>
		public double Width { get; set; }

		/// <summary>
		/// Height of base plate
		/// </summary>
		public double Thickness { get; set; }

		/// <summary>
		/// Depth of base plate
		/// </summary>
		public double Depth { get; set; }

		/// <summary>
		/// Geometry region of element3D
		/// </summary>
		public PolyLine2D GeneralShape { get; set; }

		/// <summary>
		/// Base plate Type:
		/// 1 - column
		/// 2 - forces
		/// </summary>
		public BasePlateLoadType Type { get; set; }

		/// <summary>
		/// Master component
		/// </summary>
		public ReferenceElement MasterComponent { get; set; }

		/// <summary>
		/// master component edge index
		/// </summary>
		public int EdgeIndex { get; set; }

		/// <summary>
		/// master component surface index
		/// </summary>
		public int SurfaceIndex { get; set; }

		/// <summary>
		/// position X on master component
		/// </summary>
		public double PositionX{ get; set; }

		/// <summary>
		/// position Y on master component
		/// </summary>
		public double PositionY { get; set; }
		
		/// <summary>
		/// Shear force transfer type:
		/// 1 - friction
		/// 2 - shear lug
		/// 3 - anchors
		/// </summary>
		public ShearForceTransfer ShearForceTransfer { get; set; }

		/// <summary>
		/// friction coefficient base type
		/// </summary>
		public double FrictionCoefficient { get; set; }

		/// <summary>
		/// shear lug cross-section
		/// </summary>
		public string ShearLugCrossSectionName { get; set; }

		/// <summary>
		/// shear lug length
		/// </summary>
		public double ShearLugLength { get; set; }

		/// <summary>
		/// shear lug edge
		/// </summary>
		public int ShearLugEdge { get; set; }

		/// <summary>
		/// shear lug position X
		/// </summary>
		public double ShearLugPositionX { get; set; }

		/// <summary>
		/// shear lug position Y
		/// </summary>
		public double ShearLugPositionY { get; set; }

		/// <summary>
		/// shear lug rotation
		/// </summary>
		public double ShearLugRotation { get; set; }
	}
}
