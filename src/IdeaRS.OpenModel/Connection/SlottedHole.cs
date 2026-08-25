using System.Runtime.Serialization;

namespace IdeaRS.OpenModel.Connection
{
	/// <summary>
	/// Slotted hole of one fastener position in one connected plate
	/// </summary>
	[DataContract]
	public class SlottedHole
	{
		/// <summary>
		/// Id of the fastener position - matches the Id of the corresponding item in <see cref="FastenerGridBase.Positions"/>
		/// </summary>
		[DataMember]
		public int PositionId { get; set; }

		/// <summary>
		/// Reference to the connected plate which has the slotted hole
		/// </summary>
		[DataMember]
		public ReferenceElement Plate { get; set; }

		/// <summary>
		/// Ratio of the slot length to the borehole diameter - slot length = borehole * SizeFactor
		/// </summary>
		[DataMember]
		public double SizeFactor { get; set; }

		/// <summary>
		/// Direction of the slot in the plate LCS [rad]
		/// </summary>
		[DataMember]
		public double Angle { get; set; }
	}
}
