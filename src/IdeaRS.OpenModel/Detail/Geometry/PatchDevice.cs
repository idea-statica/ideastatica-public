using System.Xml.Serialization;

namespace IdeaRS.OpenModel.Detail
{
	/// <summary>
	/// Abstract class of patch support/load
	/// </summary>
	[XmlInclude(typeof(PatchLoad))]
	[XmlInclude(typeof(PatchSupport))]
	public abstract class PatchDevice : OpenElementId
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public PatchDevice()
		{
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
		/// Offset between MasterPoint and patch support/load
		/// </summary>
		public Geometry3D.Vector3D Position { get; set; }
	}
}