using yjk.BimApis;
using IdeaStatiCa.BimApiLink.Importers;
using IdeaStatiCa.BimApi;
using IdeaRS.OpenModel.CrossSection;
using IdeaStatiCa.BimApiLink.BimApi;
using System.Collections.Generic;
using System.Linq;
using yjk.FeaApis;

namespace yjk.Importers
{
	internal class CrossSectionImporter : IntIdentifierImporter<IIdeaCrossSection>
	{
		private readonly IFeaCrossSectionApi crossSectionApi;
		public CrossSectionImporter(IFeaCrossSectionApi crossSectionApi)
		{
			this.crossSectionApi = crossSectionApi;
		}

		public override IIdeaCrossSection Create(int id)
		{
			IFeaCrossSection crossSection = crossSectionApi.GetCrossSection(id);

			switch (crossSection.CrossSectionType)
			{
				case CrossSectionType.Rect:
					List<Parameter> parameters = new List<Parameter>();
					parameters.Add(new ParameterDouble { Name = "Width", Value = crossSection.ShapeParameters[0] });
					parameters.Add(new ParameterDouble { Name = "Height", Value = crossSection.ShapeParameters[1] });

					return new CrossSectionByParameters(id)
					{
						MaterialNo = 1,
						Name = "IPE200",
						Parameters = parameters.ToHashSet(),
						Type = CrossSectionType.Rect,
					};

			}




			return new CrossSectionByName(id)
			{
				MaterialNo = 1,
				Name = "IPE200",
			};
		}
	}
}