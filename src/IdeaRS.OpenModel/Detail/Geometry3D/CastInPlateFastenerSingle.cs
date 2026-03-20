using IdeaRS.OpenModel.Parameters;
using System.Xml.Serialization;

namespace IdeaRS.OpenModel.Detail
{
	[XmlInclude(typeof(CastInPlateFastenerSingle))]
	public class CastInPlateFastenerSingle : CastInPlateFastenerBase, ISynchronization
	{
		public CastInPlateFastenerSingle()
		{
		}

		/// <summary>
		/// Synchronization ID for element tracking during OpenModel to Detail updates.
		/// </summary>
		public int SyncId { get; set; }

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
