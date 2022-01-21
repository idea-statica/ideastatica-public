using IdeaStatiCa.BimApi;
using IdeaStatiCa.RamToIdea.BimApi;
using IdeaStatiCa.RamToIdea.Model;
using RAMDATAACCESSLib;
using System;

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
						Name = $"Steel {Math.Round(matSteel.dFy)}{GetUnit()}",
						MaterialType = MaterialType.Steel
					};

				case EMATERIALTYPES.EConcreteMat:
				case EMATERIALTYPES.EWallPropConcreteMat:
					IConcreteMaterial matConcrete = _model.GetConcreteMaterial(uid);
					return new RamMaterialByName()
					{
						Name = $"Concrete {Math.Round(matConcrete.dFpc)}{GetUnit()}",
						MaterialType = MaterialType.Concrete
					};
			}

			throw new NotImplementedException();
		}

		private string GetUnit()
		{
			switch (_model.eDisplayUnits)
			{
				case EUnits.eUnitsEnglish:
					return "Kips";

				case EUnits.eUnitsSI:
					return "N";

				case EUnits.eUnitsMetric:
					return "kN";

				default:
					return "Kips";
			}
		}
	}
}