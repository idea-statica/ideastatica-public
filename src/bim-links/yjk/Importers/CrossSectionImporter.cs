using yjk.BimApis;
using IdeaStatiCa.BimApiLink.Importers;
using IdeaStatiCa.BimApi;
using IdeaRS.OpenModel.CrossSection;
using IdeaStatiCa.BimApiLink.BimApi;
using System.Collections.Generic;
using System.Linq;
using yjk.FeaApis;
using static yjk.Helpers.UnitConverter;
using IdeaStatiCa.Plugin;
using yjk.ViewModels;

namespace yjk.Importers
{
	internal class CrossSectionImporter : StringIdentifierImporter<IIdeaCrossSection>
	{
		private readonly IFeaCrossSectionApi crossSectionApi;
		private readonly IPluginLogger _logger = AppLogger.Instance;

		public CrossSectionImporter(IFeaCrossSectionApi crossSectionApi)
		{
			this.crossSectionApi = crossSectionApi;
		}

		public override IIdeaCrossSection Create(string id)
		{
			_logger.LogInformation($"CrossSectionImporter.Create: id={id}");
			IFeaCrossSection crossSection = crossSectionApi.GetCrossSection(id);

			if (crossSection.CrossSectionBy == CrossSectionBy.ByParameters)
			{
				_logger.LogInformation($"CrossSection '{id}': ByParameters, type={crossSection.CrossSectionParameterYjk.CrossSectionType}");
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
				_logger.LogInformation($"CrossSection '{id}': ByName, name={crossSection.Name}");
				return new CrossSectionByName(id)
				{
					MaterialId = crossSection.MaterialId,
					Name = crossSection.Name,
					Rotation = DegToRad(180)
				};
			}

			_logger.LogWarning($"CrossSection '{id}': unrecognised CrossSectionBy={crossSection.CrossSectionBy}, falling back to ByName");
			return new CrossSectionByName(id)
			{
				MaterialId = crossSection.MaterialId,
				Name = crossSection.Name,
			};
		}
	}
}