﻿using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.Identifiers;
using IdeaStatiCa.Plugin;
using IdeaStatiCa.TeklaStructuresPlugin.BimApi;

namespace IdeaStatiCa.TeklaStructuresPlugin.Importers
{
	internal class NegativePlateImporter : BaseImporter<IIdeaNegativePlate>
	{

		public NegativePlateImporter(IModelClient model, IPluginLogger plugInLogger)
			: base(model, plugInLogger)
		{
		}

		public override IIdeaNegativePlate Create(string id)
		{
			PlugInLogger.LogInformation($"NegativePlateImporter create '{id}'");

			var ideaPlate = GetMaybe<IIdeaPlate>(id);

			if (ideaPlate is Plate plate)
			{
				PlugInLogger.LogInformation("NegativePlateImporter created plate");
				var negativePlate = new NegativePlate(plate.No)
				{
					Thickness = plate.Thickness,
					MaterialNo = plate.MaterialNo,
					OriginNo = plate.OriginNo,
					LocalCoordinateSystem = plate.LocalCoordinateSystem,
					Geometry = plate.Geometry,
				};

				Model.CacheCreatedObject(new StringIdentifier<IIdeaNegativePlate>(id), negativePlate);

				return negativePlate;
			}

			PlugInLogger.LogInformation($"NegativePlateImporter not created plate {id}");
			return null;
		}
	}
}
