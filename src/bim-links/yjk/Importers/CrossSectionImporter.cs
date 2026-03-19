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

			if (crossSection.CrossSectionBy == CrossSectionBy.ByParameters)
			{
				return new CrossSectionByParameters(id)
				{
					MaterialNo = crossSection.MaterialId,
					Name = crossSection.Name,
					Parameters = crossSection.CrossSectionByParameters.Parameters.ToHashSet(),
					Type = crossSection.CrossSectionByParameters.CrossSectionType,
				};
			}
			else if (crossSection.CrossSectionBy == CrossSectionBy.ByName)
			{
				return new CrossSectionByName(id)
				{
					MaterialNo = crossSection.MaterialId,
					Name = crossSection.Name,
				};
			}

			return new CrossSectionByName(id)
			{
				MaterialNo = crossSection.MaterialId,
				Name = crossSection.Name,
			};
		}
	}
}