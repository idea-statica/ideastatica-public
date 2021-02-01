using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Message;
using IdeaRS.OpenModel.Result;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace IdeaStatiCa.Plugin
{
	[DataContract]
	public class ModelBIM
	{
		[DataMember]
		public string Project { get; set; }

		[DataMember]
		public RequestedItemsType RequestedItems { get; set; }

		[DataMember]
		public List<BIMItemId> Items { get; set; }

		[DataMember]
		public OpenModel Model { get; set; }

		[DataMember]
		public OpenModelResult Results { get; set; }

		[DataMember]
		public OpenMessages Messages { get; set; }
	}
}