using yjk.FeaApis;
using IdeaStatiCa.BimApiLink.BimApi;
using IdeaStatiCa.BimApiLink.Importers;
using IdeaStatiCa.BimApi;
using IdeaRS.OpenModel.Material;
using IdeaStatiCa.Plugin;
using yjk.ViewModels;

namespace yjk.Importers
{
	internal class MaterialImporter : StringIdentifierImporter<IIdeaMaterial>
	{
		private readonly IFeaMaterialApi materialApi;
		private readonly IPluginLogger _logger = AppLogger.Instance;

		public MaterialImporter(IFeaMaterialApi materialApi)
		{
			this.materialApi = materialApi;
		}

		public override IIdeaMaterial Create(string id)
		{
			_logger.LogInformation($"MaterialImporter.Create: id={id}");
			IFeaMaterial material = materialApi.GetMaterial(id);

			switch (material.MaterialType)
			{
				case MaterialType.Concrete:

					FeaMaterialConcrete materialConcrete = (FeaMaterialConcrete)material;

					MatConcreteCHN matConcrete = new MatConcreteCHN()
					{
						Fck = materialConcrete.Fck,
					};

					_logger.LogInformation($"Material '{id}': Concrete, Fck={materialConcrete.Fck}");
					return new IdeaMaterialConcrete(id)
					{
						Name = materialConcrete.Name,
						Material = matConcrete,
					};

				case MaterialType.Steel:

					_logger.LogInformation($"Material '{id}': Steel, name={material.Name}");
					return new IdeaMaterialByName(id)
					{
						MaterialType = material.MaterialType,
						Name = material.Name,
					};
			}
			
/*
			return new IdeaMaterialByName(id)
			{
				MaterialType = MaterialType.Steel,
				Name = "S 355",
			};*/

			_logger.LogWarning($"Material '{id}': unrecognised type {material.MaterialType}, returning null");
			return null;
		}
	}
}