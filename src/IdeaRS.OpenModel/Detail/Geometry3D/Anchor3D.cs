using System.Xml.Serialization;

namespace IdeaRS.OpenModel.Detail
{
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
		public ReferenceElement Material { get; set; }

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
		/// diameter of bar
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
		/// end type
		/// </summary>
		public LongReinfEndType EndsType { get; set; }
	}
}
