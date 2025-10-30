using IdeaRS.OpenModel.Parameters;
using System.Xml.Serialization;

namespace IdeaRS.OpenModel.Detail
{
	[XmlInclude(typeof(CastInPlateFastenerSingle))]
	public class CastInPlateFastenerSingle : CastInPlateFastenerBase
	{
		public CastInPlateFastenerSingle()
		{
		}

		/// <summary>
		/// Positioning type relative to the master component.
		/// </summary>
		public PositionRelatedToMasterType PositionRelatedToMasterType { get; set; }


		/// <summary>
		/// Master edge
		/// </summary>
		public int MasterEdge { get; set; }

		/// <summary>
		/// X position relative to the master component.
		/// </summary>
		public double PositionX { get; set; }

		/// <summary>
		/// Y position relative to the master component.
		/// </summary>
		public double PositionY { get; set; }
	}
}
