using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.BimApi;

namespace BimApiLinkFeaExample.BimApi
{
	/// <summary>
	/// A cross-section from the IDEA StatiCa library, whose material is imported when BimApiLink reads it.
	/// </summary>
	internal class CrossSectionByName : IdeaCrossSectionByName
	{
		public CrossSectionByName(int id)
			: base(id)
		{
		}

		public int MaterialId { get; set; }

		public override IIdeaMaterial Material => Get<IIdeaMaterial>(MaterialId);
	}
}
