using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.BimApi;

namespace BimApiLinkCadExample.BimApi
{
	/// <summary>
	/// A cross-section from the IDEA StatiCa library, whose material is imported when BimApiLink reads it.
	/// </summary>
	internal class CrossSectionByName : IdeaCrossSectionByName
	{
		public CrossSectionByName(string name)
			: base(name)
		{
		}

		public string MaterialName { get; set; }

		public override IIdeaMaterial Material => Get<IIdeaMaterial>(MaterialName);
	}
}
