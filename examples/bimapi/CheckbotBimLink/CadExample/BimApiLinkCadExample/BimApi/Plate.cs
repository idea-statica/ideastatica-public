using IdeaStatiCa.BimApiLink.BimApi;
using IdeaStatiCa.BimApi;


namespace BimApiLinkCadExample.BimApi
{
	internal class Plate : IdeaPlate
	{
		public override string Name { get; set; }

		public override IIdeaMaterial Material => Get<IIdeaMaterial>(MaterialNo);

		public int MaterialNo { get; set; }

		public int No { get; }

		public override IIdeaNode Origin => Get<IIdeaNode>(OriginNo);

		public string OriginNo { get; set; }

		public Plate(int no) : base(no)
		{
			No = no;
			Name = $"P{No}";
		}
	}
}
