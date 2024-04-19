using IdeaRS.OpenModel.Geometry2D;
using System.Xml.Serialization;

namespace IdeaRS.OpenModel.Detail
{
	/// <summary>
	/// Solid shape type
	/// </summary>
	public enum SolidShapeCode : int
	{
		/// <summary>
		/// General shape
		/// </summary>
		General = 1,

		/// <summary>
		/// Rectangular shape
		/// </summary>
		Rectangular = 2,
	}

	/// <summary>
	/// Representation of Solid Block in IDEA StatiCa Detail
	/// </summary>
	[XmlInclude(typeof(SolidBlock3D))]
	public class SolidBlock3D : OpenElementId
	{
		public SolidBlock3D()
			: base()
		{

		}

		/// <summary>
		/// Gets or sets the code of the solid block:
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
		/// Width of solid block
		/// </summary>
		public double Width { get; set; }

		/// <summary>
		/// Height of solid block
		/// </summary>
		public double Height { get; set; }

		/// <summary>
		/// Depth of solid block
		/// </summary>
		public double Depth { get; set; }

		/// <summary>
		/// x-offset of top left corner
		/// </summary>
		public double OffsetX1 { get; set; }

		/// <summary>
		/// x-offset of top right corner
		/// </summary>
		public double OffsetX2 { get; set; }

		/// <summary>
		/// y-offset of top right corner
		/// </summary>
		public double OffsetY1 { get; set; }

		/// <summary>
		/// y-offset of top left corner
		/// </summary>
		public double OffsetY2 { get; set; }

		/// <summary>
		/// Geometry region of element3D
		/// </summary>
		public PolyLine2D GeneralShape { get; set; }

		/// <summary>
		/// Master component
		/// </summary>
		public ReferenceElement MasterComponent { get; set; }

		/// <summary>
		/// Master point 0 - 9
		/// </summary>
		public int MasterPoint { get; set; }

		/// <summary>
		/// Insert point 0 - 9
		/// </summary>
		public int InsertPoint { get; set; }

		/// <summary>
		/// Offset between MasterPoint and InsertPoint
		/// If MasterPoint is null, position is from origin of coordinate system
		/// </summary>
		public IdeaRS.OpenModel.Geometry3D.Vector3D Position { get; set; }

		/// <summary>
		/// Rotation between MasterComponent axes and solid axes
		/// If MasterComponent is null, rotation is from global coordinate system
		/// </summary>
		public IdeaRS.OpenModel.Geometry3D.Vector3D Rotation { get; set; }
	}
}
