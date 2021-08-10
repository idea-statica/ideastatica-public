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
					return CreateMaterialByName(materialByName);

				case IIdeaMaterialSteel matSteal:
					return CreateMaterialSteel(matSteal);

				case IIdeaMaterialConcrete matConcrete:
					return CreateMaterialConcrete(matConcrete);
			}

			Logger.LogError($"Material '{material.Id}' is of unsupported type '{material.GetType().Name}'.");
			throw new ConstraintException($"Material '{material.Id}' is of unsupported type '{material.GetType().Name}'.");
		}

		private OpenElementId CreateMaterialConcrete(IIdeaMaterialConcrete matConcrete)
		{
			Logger.LogTrace($"Importing concrete material '{matConcrete.Id}'.");

			MatConcrete mat = matConcrete.Material;

			if (mat.Name == null)
			{
				mat.Name = matConcrete.Name;
			}

			return mat;
		}

		private OpenElementId CreateMaterialSteel(IIdeaMaterialSteel matSteal)
		{
			Logger.LogTrace($"Importing steel material '{matSteal.Id}'.");

			MatSteel mat = matSteal.Material;

			if (mat.Name == null)
			{
				mat.Name = matSteal.Name;
			}

			return mat;
		}

		private OpenElementId CreateMaterialByName(IIdeaMaterialByName materialByName)
		{
			string name = materialByName.Name;
			MaterialType type = materialByName.MaterialType;

			Logger.LogTrace($"Importing material of type '{type}' by name '{materialByName.Id}'.");

			if (string.IsNullOrEmpty(name))
			{
				Logger.LogError($"Material '{materialByName.Id}' has empty/null name.");
				throw new ConstraintException($"Material '{materialByName.Id}' has empty/null name.");
			}

			Material mat = CreateMaterialFromType(materialByName.MaterialType);
			mat.LoadFromLibrary = true;
			mat.Name = name;

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

			// if we got here then someone forgot to implement something
			throw new NotImplementedException();
		}
	}
}