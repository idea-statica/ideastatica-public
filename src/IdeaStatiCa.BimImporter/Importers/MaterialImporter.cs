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
			if (material is IIdeaMaterialByName materialByName)
			{
				if (material.Name is null)
				{
					throw new ConstraintException("Name property must not be null for IIdeaMaterialByName.");
				}

				Material mat = CreateMaterialFromType(materialByName.MaterialType);
				mat.LoadFromLibrary = true;
				mat.Name = material.Name;

				return mat;
			}
			else if (material is IIdeaMaterialSteel matSteal)
			{
				MatSteel mat = matSteal.Material;

				if (mat.Name == null)
				{
					mat.Name = material.Name;
				}

				return mat;
			}
			else if (material is IIdeaMaterialConcrete matConcrete)
			{
				MatConcrete mat = matConcrete.Material;

				if (mat.Name == null)
				{
					mat.Name = material.Name;
				}

				return mat;
			}

			throw new NotImplementedException();
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

				case MaterialType.Steal:
					return new MatSteelEc2();
			}

			throw new NotImplementedException();
		}
	}
}