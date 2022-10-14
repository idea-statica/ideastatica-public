using IdeaStatica.BimApiLink.BimApi;
using IdeaStatiCa.BimApi;

namespace BimApiExample.Plugin.BimApi
{
	internal class Member1D : IdeaMember1D
	{
		public int CrossSectionNo { get; init; }

		public override IIdeaCrossSection CrossSection => Get<IIdeaCrossSection>(CrossSectionNo);

		public Member1D(int no) : base(no)
		{
		}
	}
}