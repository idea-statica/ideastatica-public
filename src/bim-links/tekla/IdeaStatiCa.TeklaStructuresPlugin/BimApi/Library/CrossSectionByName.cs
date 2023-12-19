using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.BimApi;

namespace IdeaStatiCa.TeklaStructuresPlugin.BimApi.Library
{
	internal class CrossSectionByName : IdeaCrossSectionByName
	{
		public override IIdeaMaterial Material => Get<IIdeaMaterial>(MaterialNo);

		public string MaterialNo { get; set; }

		public CrossSectionByName(string no)
			: base(no)
		{
		}
	}
}
