using System.Xml.Serialization;

namespace IdeaRS.OpenModel.Detail
{

	/// <summary>
	/// Representation of Wall in IDEA StatiCa Detail
	/// </summary>
	[XmlInclude(typeof(WallRect))]
	public class Wall : Model.Element2D, IGeometryPart
	{
		/// <summary>
		/// Name
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Constructor
		/// </summary>
		public Wall() : base()
		{
			Edges = new System.Collections.Generic.List<ReferenceElement>();
		}

		/// <summary>
		/// Id representing geometrical parts of Detail
		/// </summary>
		public int GeomId { get; set; }

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
		/// Lines of Element2D including edges, which are created by opening
		/// </summary>
		public System.Collections.Generic.List<ReferenceElement> Edges { get; set; }

		/// <summary>
		/// Offset between MasterPoint and InsertPoint
		/// If MasterPoint is null, position is from origin of coordinate system
		/// </summary>
		public Geometry3D.Vector3D Position { get; set; }
	}
}
