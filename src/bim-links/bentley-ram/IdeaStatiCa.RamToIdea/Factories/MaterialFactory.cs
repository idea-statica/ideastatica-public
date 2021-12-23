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
					ISteelMaterial matSteel = _model.GetSteelMaterial(uid);
					return new RamMaterialByName(uid)
					{
						Name = $"Steel {Math.Round(matSteel.dFy)}Kips",
						MaterialType = MaterialType.Steel
					};

				case EMATERIALTYPES.EConcreteMat:
					IConcreteMaterial matConcrete = _model.GetConcreteMaterial(uid);
					return new RamMaterialByName(uid)
					{
						Name = $"Concrete {Math.Round(matConcrete.dFpc)}Kips",
						MaterialType = MaterialType.Concrete
					};
			}

			throw new NotImplementedException();
		}
	}
}