using IdeaRS.OpenModel.Parameters;
using System.Xml.Serialization;

namespace IdeaRS.OpenModel.Detail
{
	[XmlInclude(typeof(CastInPlateFastenerGroup))]
	public class CastInPlateFastenerGroup : CastInPlateFastenerBase
	{
		public CastInPlateFastenerGroup()
		{
		}

		/// <summary>
		/// Gets or sets the rows of a fasteners positions.
		/// Absolute positions of groups, in group are relative positions.
		/// </summary>
		public NumberGroups Rows { get; set; }

		/// <summary>
		/// Distance of fasteners
		/// </summary>
		public double Distance { get; set; }

		/// <summary>
		/// Gets or sets the spacing between fasteners positions.
		/// Absolute positions of groups, in group are relative positions.
		/// </summary>
		public NumberGroups Spacing { get; set; }
	}
}
