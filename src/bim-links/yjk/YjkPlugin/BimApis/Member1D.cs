using IdeaStatiCa.BimApiLink.BimApi;
using IdeaStatiCa.BimApi;

namespace yjk.BimApis
{
	internal class Member1D : IdeaMember1D
	{
		public string CrossSectionId { get; set; }

		public override IIdeaCrossSection CrossSection => Get<IIdeaCrossSection>(CrossSectionId);

		public Member1D(int no) : base(no)
		{
		}
	}
}