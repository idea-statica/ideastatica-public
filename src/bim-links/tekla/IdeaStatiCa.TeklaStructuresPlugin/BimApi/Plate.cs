using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.BimApi;

namespace IdeaStatiCa.TeklaStructuresPlugin.BimApi
{
	internal class Plate : IdeaPlate
	{
		public override string Name { get; set; }

		public override IIdeaMaterial Material => Get<IIdeaMaterial>(MaterialNo);

		public string MaterialNo { get; set; }

		public string No { get; }

		public override IIdeaNode Origin => Get<IIdeaNode>(OriginNo);

		public string OriginNo { get; set; }

		public Plate(string no)
			: base(no)
		{
			No = no;
		}
	}
}
