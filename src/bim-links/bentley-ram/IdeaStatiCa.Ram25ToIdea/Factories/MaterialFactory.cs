using IdeaStatiCa.BimApi;
using IdeaStatiCa.RamToIdea.BimApi;
using IdeaStatiCa.RamToIdea.Model;
using RAMDATAACCESSLib;
using System;
using IdeaStatiCa.RamToIdea.Utilities;

namespace IdeaStatiCa.RamToIdea.Factories
{
	internal class MaterialFactory : IMaterialFactory
	{
		private readonly IModel _model;

		public MaterialFactory(IModel model)
		{
			_model = model;
		}

		public IIdeaMaterial GetMaterial(RamMemberProperties props)
		{
			int uid = props.MaterialUID;

			switch (props.MaterialType)
			{
				case EMATERIALTYPES.ESteelMat:
				case EMATERIALTYPES.ESteelJoistMat:
					ISteelMaterial matSteel = _model.GetSteelMaterial(uid);
					return new RamMaterialByName()
					{
						Name = $"Steel {Math.Round(GetValue(matSteel.dFy))}{GetUnit()}",
						MaterialType = MaterialType.Steel
					};

				case EMATERIALTYPES.EConcreteMat:
				case EMATERIALTYPES.EWallPropConcreteMat:
					IConcreteMaterial matConcrete = _model.GetConcreteMaterial(uid);
					return new RamMaterialByName()
					{
						Name = $"Concrete {Math.Round(GetValue(matConcrete.dFpc))}{GetUnit()}",
						MaterialType = MaterialType.Concrete
					};

				case EMATERIALTYPES.EOtherMat:
				case EMATERIALTYPES.EWallPropOtherMat:
					IOtherMaterial matOther = _model.GetOtherMaterial(uid);
					return new RamMaterialByName()
					{
						Name = $"Other {Math.Round(GetValue(matOther.dE))}{GetUnit()}",
						MaterialType = MaterialType.Steel
					};
			}

			throw new NotImplementedException();
		}

		private double GetValue(double value)
		{
			switch (_model.eDisplayUnits)
			{
				case EUnits.eUnitsEnglish:
					return value;
				case EUnits.eUnitsSI:
					return value.KipsToMPascal();
				case EUnits.eUnitsMetric:
					return value.KipsToKgPerCm2();
				default:
					return value;
			}
		}

		private string GetUnit()
		{
			switch (_model.eDisplayUnits)
			{
				case EUnits.eUnitsEnglish:
					return "Kips";

				case EUnits.eUnitsSI:
					return "Mpa";

				case EUnits.eUnitsMetric:
					return "kg/cm^2";

				default:
					return "Kips";
			}
		}
	}
}