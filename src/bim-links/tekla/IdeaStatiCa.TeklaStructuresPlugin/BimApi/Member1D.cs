using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.BimApi;

namespace IdeaStatiCa.TeklaStructuresPlugin.BimApi
{
	internal class Member1D : IdeaMember1D
	{
		public override IIdeaCrossSection CrossSection => Get<IIdeaCrossSection>(CrossSectionNo);

		public string CrossSectionNo { get; set; }

		public string No { get; }

		public double Length { get; set; }

		public Member1D(string no)
			: base(no)
		{
			No = no;
		}
	}
}
