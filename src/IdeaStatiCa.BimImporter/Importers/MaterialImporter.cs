using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Material;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.Plugin;
using System;

namespace IdeaStatiCa.BimImporter.Importers
{
	internal class MaterialImporter : AbstractImporter<IIdeaMaterial>
	{
		public MaterialImporter(IPluginLogger logger) : base(logger)
		{
		}

		protected override OpenElementId ImportInternal(IImportContext ctx, IIdeaMaterial material)
		{
			switch (material)
			{
				case IIdeaMaterialByName materialByName:
					return CreateMaterialByName(material, materialByName);

				case IIdeaMaterialSteel matSteal:
					return CreateMaterialSteel(material, matSteal);

				case IIdeaMaterialConcrete matConcrete:
					return CreateMaterialConcrete(material, matConcrete);
			}

			throw new ConstraintException($"Unsupported cross-section type '{material.GetType().Name}'.");
		}

		private OpenElementId CreateMaterialConcrete(IIdeaMaterial material, IIdeaMaterialConcrete matConcrete)
		{
			MatConcrete mat = matConcrete.Material;

			if (mat.Name == null)
			{
				mat.Name = material.Name;
			}

			return mat;
		}

		private OpenElementId CreateMaterialSteel(IIdeaMaterial material, IIdeaMaterialSteel matSteal)
		{
			MatSteel mat = matSteal.Material;

			if (mat.Name == null)
			{
				mat.Name = material.Name;
			}

			return mat;
		}

		private OpenElementId CreateMaterialByName(IIdeaMaterial material, IIdeaMaterialByName materialByName)
		{
			if (material.Name is null)
			{
				throw new ConstraintException($"Name property must not be null for {nameof(IIdeaMaterialByName)}.");
			}

			Material mat = CreateMaterialFromType(materialByName.MaterialType);
			mat.LoadFromLibrary = true;
			mat.Name = material.Name;

			return mat;
		}

		private Material CreateMaterialFromType(MaterialType matType)
		{
			// we use ECEN materials just as placeholders
			switch (matType)
			{
				case MaterialType.Concrete:
					return new MatConcreteEc2();

				case MaterialType.Reinforcement:
					return new MatReinforcementEc2();

				case MaterialType.Steel:
					return new MatSteelEc2();
			}

			throw new NotImplementedException();
		}
	}
}