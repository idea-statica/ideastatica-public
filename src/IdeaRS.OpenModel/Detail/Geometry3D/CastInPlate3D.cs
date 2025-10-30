using IdeaRS.OpenModel.Geometry2D;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace IdeaRS.OpenModel.Detail
{
	/// <summary>
	/// Type of cast-in plate
	/// </summary>
	public enum CastInPlateLoadType : int
	{
		Direct = 1,
		Column = 2,
	}

	/// <summary>
	/// Representation of Cast-in Plate3D 3D in IDEA StatiCa Detail
	/// </summary>
	[XmlInclude(typeof(CastInPlate3D))]
	public class CastInPlate3D : OpenElementId
	{
		public CastInPlate3D()
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
		/// Load position X
		/// </summary>
		public double LoadPositionX { get; set; }

		/// <summary>
		/// Load position Y
		/// </summary>
		public double LoadPositionY { get; set; }

		///// <summary>
		///// Load length
		///// </summary>
		//public double LoadLength { get; set; }

		/// <summary>
		/// Position load column related to edge or center point
		/// </summary>
		public PositionRelatedToMasterType LoadPositionRelatedToType { get; set; }

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
		/// Cast-in  plate Type:
		/// 1 - column
		/// 2 - forces
		/// </summary>
		public CastInPlateLoadType Type { get; set; }

		/// <summary>
		/// Master component
		/// </summary>
		public ReferenceElement MasterComponent { get; set; }

		/// <summary>
		/// Master component edge index
		/// </summary>
		public int MasterEdgeIndex { get; set; }

		/// <summary>
		/// Master component surface index
		/// </summary>
		public int MasterSurfaceIndex { get; set; }

		/// <summary>
		/// Position X on master component
		/// </summary>
		public double PositionX{ get; set; }

		/// <summary>
		/// Position Y on master component
		/// </summary>
		public double PositionY { get; set; }

		/// <summary>
		/// Rotation on master component
		/// </summary>
		public double Rotation { get; set; }

		/// <summary>
		/// Position related to edge or center point
		/// </summary>
		public PositionRelatedToMasterType PositionRelatedToMasterType { get; set; }

		/// <summary>
		/// Fasteners
		/// </summary>
		public List<CastInPlateFastenerBase> Fasteners { get; set; }
	}
}
