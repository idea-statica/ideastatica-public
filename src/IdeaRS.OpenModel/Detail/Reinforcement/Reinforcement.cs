using System.Xml.Serialization;

namespace IdeaRS.OpenModel.Detail
{
	/// <summary>
	/// Base class representing reinforcement in IDEA StatiCa Detail
	/// </summary>
	[XmlInclude(typeof(ReinfBarByPolyline))]
	[XmlInclude(typeof(ReinfBarByTwoPoints))]
	[XmlInclude(typeof(ReinfBarOnEdge))]
	[XmlInclude(typeof(ReinfBarOnMoreEdges))]
	[XmlInclude(typeof(ReinfAroundOpening))]
	[XmlInclude(typeof(ReinfFabric))]
	[XmlInclude(typeof(ReinfBarInclined))]
	[XmlInclude(typeof(Hanging))]
	[XmlInclude(typeof(BentUpBar))]
	[XmlInclude(typeof(HangingAroundPatch))]
	public abstract class Reinforcement : OpenElementId
	{

		/// <summary>
		/// Id representing reinforcement in Detail
		/// </summary>
		public int ReinfId { get; set; }

		/// <summary>
		/// Material
		/// </summary>
		public ReferenceElement Material { get; set; }
	}
}
