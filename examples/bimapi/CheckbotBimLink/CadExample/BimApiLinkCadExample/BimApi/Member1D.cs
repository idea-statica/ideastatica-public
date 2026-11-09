using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.BimApi;

namespace BimApiLinkCadExample.BimApi
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

		public string CrossSectionName { get; set; }

		public override IIdeaCrossSection CrossSection => Get<IIdeaCrossSection>(CrossSectionName);
	}
}
