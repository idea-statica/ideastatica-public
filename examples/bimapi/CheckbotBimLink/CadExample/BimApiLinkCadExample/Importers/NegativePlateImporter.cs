using BimApiLinkCadExample.BimApi;
using BimApiLinkCadExample.CadExampleApi;
using IdeaStatiCa.BimApi;

namespace BimApiLinkCadExample.Importers
{
	internal class NegativePlateImporter : BaseImporter<IIdeaNegativePlate>
	{
		public NegativePlateImporter(ICadGeometryApi model)
			: base(model)
		{
		}

		public override IIdeaNegativePlate Create(int id)
		{
			//PlugInLogger.LogInformation($"NegativePlateImporter create '{id}'");

			var ideaPlate = GetMaybe<IIdeaPlate>(id);

			if (ideaPlate is Plate plate)
			{
				//PlugInLogger.LogInformation("NegativePlateImporter created plate");
				return new NegativePlate(plate.No)
				{
					Thickness = plate.Thickness,
					MaterialNo = plate.MaterialNo,
					OriginNo = plate.OriginNo,
					LocalCoordinateSystem = plate.LocalCoordinateSystem,
					Geometry = plate.Geometry,
				};
			}

			return null;
		}
	}
}
