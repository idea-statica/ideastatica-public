using System;
using System.Runtime.Serialization;

namespace IdeaRS.OpenModel.Parameters
{
	/// <summary>
	/// Parameter which represts the general record in IDEA MPRL
	/// </summary>
	[DataContract]
	[Serializable]
	[KnownType(typeof(AnchorParam))]
	[KnownType(typeof(BoltParam))]
	[KnownType(typeof(CrossSectionParam))]
	[KnownType(typeof(MaterialParam))]
	[KnownType(typeof(WeldParam))]
	public class MprlRecord
	{
		/// <summary>
		/// Name of the item
		/// </summary>
		[DataMember]
		public string Name { get; set; }

		/// <summary>
		/// Mprl TableId
		/// </summary>
		[DataMember]
		public Guid TableId { get; set; }

		/// <summary>
		///  Mprl ItemId
		/// </summary>
		[DataMember]
		public Guid ItemId { get; set; }
	}
}
