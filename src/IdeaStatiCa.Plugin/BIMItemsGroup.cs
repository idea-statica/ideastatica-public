using System.Collections.Generic;
using System.Runtime.Serialization;

namespace IdeaStatiCa.Plugin
{
	[DataContract]
	public class BIMItemsGroup
	{
		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public RequestedItemsType Type { get; set; }

		[DataMember]
		public List<BIMItemId> Items { get; set; }
	}
}