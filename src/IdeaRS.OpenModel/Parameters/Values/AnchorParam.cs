using System;
using System.Runtime.Serialization;

namespace IdeaRS.OpenModel.Parameters
{
	/// <summary>
	/// Defines a type of anchor end.
	/// </summary>
	public enum AnchorType : int
	{
		/// <summary>
		/// Straight end - no washer.
		/// </summary>
		Straight,

		/// <summary>
		/// Circular washer plate.
		/// </summary>
		WasherPlateCircular,

		/// <summary>
		/// Rectangular washer plate.
		/// </summary>
		WasherPlateRectangular,

		/// <summary>
		/// Hook
		/// </summary>
		Hook
	}

	/// <summary>
	/// Parameter which represts the record of the anchor in IDEA MPRL
	/// </summary>
	[DataContract]
	[Serializable]
	public class AnchorParam : BoltParam
	{
		/// <summary>
		/// Gets or sets the anchor length.
		/// </summary>
		[DataMember]
		public double Length { get; set; }

		/// <summary>
		/// Gets or sets the various type of anchor.
		/// </summary>
		[DataMember]
		public AnchorType AnchorTypeData { get; set; }

		/// <summary>
		/// Gets or sets the size of washer plate.
		/// </summary>
		[DataMember]
		public double AnchorTypeSize { get; set; }
	}
}