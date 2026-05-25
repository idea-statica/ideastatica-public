using IdeaStatiCa.BimApiLink.BimApi;
using IdeaStatiCa.BimApi;

namespace yjk.BimApis
{
	internal class CrossSectionByName : IdeaCrossSectionByName
	{
		public override IIdeaMaterial Material => Get<IIdeaMaterial>(MaterialId);

		public string MaterialId { get; set; }

		override public double Rotation { get; set; }

		public CrossSectionByName(string stringId) : base(stringId)
		{
		}
	}
}