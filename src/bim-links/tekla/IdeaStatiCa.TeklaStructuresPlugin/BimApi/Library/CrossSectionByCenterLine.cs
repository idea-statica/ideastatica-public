using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.BimApi;


namespace IdeaStatiCa.TeklaStructuresPlugin.BimApi.Library
{
	internal class CrossSectionByCenterLine : IdeaCrossSectionByCenterLine
	{
		public override IIdeaMaterial Material => Get<IIdeaMaterial>(MaterialNo);

		public string MaterialNo { get; set; }

		public CrossSectionByCenterLine(string no)
			: base(no)
		{

		}
	}
}