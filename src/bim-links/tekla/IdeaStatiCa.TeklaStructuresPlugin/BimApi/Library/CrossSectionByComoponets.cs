using IdeaRS.OpenModel.Geometry2D;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.BimApi;
using System.Collections.Generic;


namespace IdeaStatiCa.TeklaStructuresPlugin.BimApi.Library
{
	internal class CrossSectionByComoponets : IdeaCrossSectionByComponents
	{
		public CrossSectionByComoponets(string no)
			: base(no)
		{
			Components = new HashSet<IIdeaCrossSectionComponent>();
		}
	}

	internal class CrossSectionComponent : IIdeaCrossSectionComponent
	{
		public IIdeaMaterial Material { get; set; }

		public Region2D Geometry { get; set; }

		public int Phase { get; set; }
	}

}
