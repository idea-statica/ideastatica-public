using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.BimApi;
using IdeaStatiCa.Plugin;
using IdeaStatiCa.TeklaStructuresPlugin.BimApi;
using Tekla.Structures.Catalogs;

namespace IdeaStatiCa.TeklaStructuresPlugin.Importers
{
	internal class MaterialImporter : BaseImporter<IIdeaMaterial>
	{
		public MaterialImporter(IModelClient model, IPluginLogger plugInLogger)
			: base(model, plugInLogger)
		{
		}

		public override IIdeaMaterial Create(string id)
		{
			PlugInLogger.LogInformation($"MaterialImporter create {id}");
			var material = Model.GetMaterial(id);
			if (material == null)
			{
				id = string.IsNullOrWhiteSpace(id) ? "unknown material" : id;
				// e.g. the bolt material 10.9

				PlugInLogger.LogInformation($"MaterialImporter create IdeaMaterialByName {id}");
				return new IdeaMaterialByName(id)
				{
					MaterialType = MaterialType.Steel,
					Name = id,
				};
			}

			PlugInLogger.LogInformation($"MaterialImporter create IdeaMaterialByName {material.MaterialName}");
			return new IdeaMaterialByName(material.MaterialName)
			{
				MaterialType = GetMaterialType(material.Type),
				Name = material.MaterialName,
			};
		}

		private MaterialType GetMaterialType(MaterialItem.MaterialItemTypeEnum? materialType)
		{
			switch (materialType)
			{
				case MaterialItem.MaterialItemTypeEnum.MATERIAL_CONCRETE:
					return MaterialType.Concrete;

				case MaterialItem.MaterialItemTypeEnum.MATERIAL_REBAR:
					return MaterialType.Reinforcement;

				case MaterialItem.MaterialItemTypeEnum.MATERIAL_STEEL:
					return MaterialType.Steel;

				default:
					// default to Steel if we dont recognize the material
					return MaterialType.Steel;
			}
		}
	}
}
