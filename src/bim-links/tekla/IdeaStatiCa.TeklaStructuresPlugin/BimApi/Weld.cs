using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.BimApi;
using System.Collections.Generic;

namespace IdeaStatiCa.TeklaStructuresPlugin.BimApi
{
	internal class Weld : IdeaWeld
	{
		public override string Name { get; set; }

		public override IIdeaMaterial Material => string.IsNullOrEmpty(MaterialNo) ? null : Get<IIdeaMaterial>(MaterialNo);

		public string MaterialNo { get; set; }

		public override IIdeaNode Start => Get<IIdeaNode>(StartNo);

		public string StartNo { get; set; }

		public override IIdeaNode End => Get<IIdeaNode>(EndNo);

		public string EndNo { get; set; }

		public Weld(string no)
			: base(no)
		{
			ConnectedParts = new List<IIdeaObjectConnectable>();
		}
	}
}