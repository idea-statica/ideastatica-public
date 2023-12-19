using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.BimApi;

namespace IdeaStatiCa.TeklaStructuresPlugin.BimApi.Library
{
	internal class CrossSectionByParameters : IdeaCrossSectionByParameters
	{
		public override IIdeaMaterial Material => Get<IIdeaMaterial>(MaterialNo);

		public string MaterialNo { get; set; }

		public CrossSectionByParameters(string no)
			: base(no)
		{
			Parameters = new System.Collections.Generic.HashSet<IdeaRS.OpenModel.CrossSection.Parameter>();
		}
	}
}
