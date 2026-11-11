using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.BimApi;

namespace BimApiLinkFeaExample.BimApi
{
	/// <summary>
	/// A member whose cross-section is imported when BimApiLink reads it.
	/// </summary>
	internal class Member1D : IdeaMember1D
	{
		public Member1D(int id)
			: base(id)
		{
		}

		public int CrossSectionId { get; set; }

		public override IIdeaCrossSection CrossSection => Get<IIdeaCrossSection>(CrossSectionId);
	}
}
