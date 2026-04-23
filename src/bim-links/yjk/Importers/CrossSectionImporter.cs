using yjk.BimApis;
using IdeaStatiCa.BimApiLink.Importers;
using IdeaStatiCa.BimApi;
using IdeaRS.OpenModel.CrossSection;
using IdeaStatiCa.BimApiLink.BimApi;
using System.Collections.Generic;
using System.Linq;
using yjk.FeaApis;
using static yjk.Helpers.UnitConverter;

namespace yjk.Importers
{
	internal class CrossSectionImporter : StringIdentifierImporter<IIdeaCrossSection>
	{
		private readonly IFeaCrossSectionApi crossSectionApi;
		public CrossSectionImporter(IFeaCrossSectionApi crossSectionApi)
		{
			this.crossSectionApi = crossSectionApi;
		}

		public override IIdeaCrossSection Create(string id)
		{
			IFeaCrossSection crossSection = crossSectionApi.GetCrossSection(id);

			if (crossSection.CrossSectionBy == CrossSectionBy.ByParameters)
			{
				return new CrossSectionByParameters(id)
				{
					MaterialId = crossSection.MaterialId,
					Name = crossSection.Name,
					Parameters = crossSection.CrossSectionParameterYjk.Parameters.ToHashSet(),
					Type = crossSection.CrossSectionParameterYjk.CrossSectionType,
					//Rotation = DegToRad(90)
				};
			}
			else if (crossSection.CrossSectionBy == CrossSectionBy.ByName)
			{
				return new CrossSectionByName(id)
				{
					MaterialId = crossSection.MaterialId,
					Name = crossSection.Name,
					Rotation = DegToRad(180)
				};
			}

			return new CrossSectionByName(id)
			{
				MaterialId = crossSection.MaterialId,
				Name = crossSection.Name,
			};
		}
	}
}