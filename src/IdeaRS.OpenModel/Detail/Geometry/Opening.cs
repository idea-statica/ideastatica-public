using System.Xml.Serialization;

namespace IdeaRS.OpenModel.Detail
{
	/// <summary>
	/// Representation of opening in IDEA StatiCa Detail
	/// </summary>
	[XmlInclude(typeof(OpeningRect))]
	[XmlInclude(typeof(OpeningCirc))]
	[XmlInclude(typeof(OpeningRectOffsets))]
	public class Opening : OpenElementId
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public Opening()
		{
		}

		/// <summary>
		/// Name
		/// </summary>
		public string Name { get; set; }

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
		/// Outline of Oopening
		/// </summary>
		public ReferenceElement Outline { get; set; }

		///// <summary>
		///// Rectengle circumscribed outline of DetElement2D
		///// </summary>
		//public ReferenceElement OutlineCircumscribed { get; set; }

		/// <summary>
		/// Offset between MasterPoint and InsertPoint
		/// If MasterPoint is null, position is from origin of coordinate system
		/// </summary>
		public Geometry3D.Vector3D Position { get; set; }
	}
}