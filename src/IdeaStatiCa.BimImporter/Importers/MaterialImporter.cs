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
					return CreateMaterialByName(ctx, materialByName);

				case IIdeaMaterialSteel matSteal:
					return CreateMaterialSteel(matSteal);

				case IIdeaMaterialConcrete matConcrete:
					return CreateMaterialConcrete(matConcrete);

				case IIdeaMaterialBoltGrade matBoltGrade:
					return CreateMaterialBoltGrade(matBoltGrade);
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

		private OpenElementId CreateMaterialBoltGrade(IIdeaMaterialBoltGrade matBoltGrade)
		{
			Logger.LogTrace($"Importing boltgrade material '{matBoltGrade.Id}'.");

			MaterialBoltGrade mat = matBoltGrade.Material;

			if (mat.Name == null)
			{
				mat.Name = matBoltGrade.Name;
			}

			return mat;
		}

		private OpenElementId CreateMaterialByName(IImportContext ctx, IIdeaMaterialByName materialByName)
		{
			string name = materialByName.Name;
			MaterialType type = materialByName.MaterialType;

			Logger.LogTrace($"Importing material of type '{type}' by name '{materialByName.Id}'.");

			if (string.IsNullOrEmpty(name))
			{
				Logger.LogError($"Material '{materialByName.Id}' has empty/null name.");
				throw new ConstraintException($"Material '{materialByName.Id}' has empty/null name.");
			}

			Material mat = CreateMaterialFromType(ctx, materialByName.MaterialType);
			mat.LoadFromLibrary = true;
			mat.Name = name;

			return mat;
		}

		private static Material CreateMaterialFromType(IImportContext ctx, MaterialType matType)
		{
			CountryCode countryCode = ctx.CountryCode;

			switch (matType)
			{
				case MaterialType.Concrete:

					return CreateMaterialConcrete(countryCode);

				case MaterialType.Reinforcement:

					return CreateMaterialReinforcement(countryCode);

				case MaterialType.Steel:
					return CreateMaterialSteel(countryCode);

				case MaterialType.BoltGrade:
					return CreateMaterialBoltGrade(countryCode);
			}

			throw new NotImplementedException();
		}

		private static Material CreateMaterialSteel(CountryCode countryCode)
		{
			switch (countryCode)
			{
				case CountryCode.India:
					return new MatSteelIND();

				case CountryCode.American:
					return new MatSteelAISC();

				case CountryCode.Canada:
					return new MatSteelCISC();

				case CountryCode.Australia:
					return new MatSteelAUS();

				case CountryCode.RUS:
					return new MatSteelRUS();

				case CountryCode.CHN:
					return new MatSteelCHN();

				case CountryCode.HKG:
					return new MatSteelHKG();

				default:
					return new MatSteelEc2();
			}
		}

		private static Material CreateMaterialReinforcement(CountryCode countryCode)
		{
			switch (countryCode)
			{
				case CountryCode.India:
					return new MatReinforcementIND();

				case CountryCode.American:
					return new MatReinforcementACI();

				case CountryCode.Canada:
					return new MatReinforcementCAN();

				case CountryCode.Australia:
					return new MatReinforcementAUS();

				case CountryCode.RUS:
					return new MatReinforcementRUS();

				case CountryCode.CHN:
					return new MatReinforcementCHN();

				case CountryCode.HKG:
					return new MatReinforcementHKG();

				default:
					return new MatReinforcementEc2();
			}
		}

		private static Material CreateMaterialConcrete(CountryCode countryCode)
		{
			switch (countryCode)
			{
				case CountryCode.India:
					return new MatConcreteIND();

				case CountryCode.SIA:
					return new MatConcreteSIA();

				case CountryCode.American:
					return new MatConcreteACI();

				case CountryCode.Canada:
					return new MatConcreteCAN();

				case CountryCode.Australia:
					return new MatConcreteAUS();

				case CountryCode.RUS:
					return new MatConcreteRUS();

				case CountryCode.CHN:
					return new MatConcreteCHN();

				case CountryCode.HKG:
					return new MatConcreteHKG();

				default:
					return new MatConcreteEc2();
			}
		}

		private static Material CreateMaterialBoltGrade(CountryCode countryCode)
		{
			switch (countryCode)
			{
				case CountryCode.India:
					return new MaterialBoltGradeIND();

				case CountryCode.American:
					return new MaterialBoltGradeAISC();

				case CountryCode.Canada:
					return new MaterialBoltGradeCISC();

				case CountryCode.Australia:
					return new MaterialBoltGradeAUS();

				case CountryCode.CHN:
					return new MaterialBoltGradeCHN();

				case CountryCode.HKG:
					return new MaterialBoltGradeHKG();

				case CountryCode.ECEN:
					return new MaterialBoltGradeEc2();

				default:
					return new MaterialBoltGrade();
			}
		}
	}
}