using yjk.FeaApis;
using IdeaStatiCa.BimApiLink.BimApi;
using IdeaStatiCa.BimApiLink.Importers;
using IdeaStatiCa.BimApi;
using IdeaRS.OpenModel.Material;

namespace yjk.Importers
{
	internal class MaterialImporter : IntIdentifierImporter<IIdeaMaterial>
	{
		private readonly IFeaMaterialApi materialApi;

		public MaterialImporter(IFeaMaterialApi materialApi)
		{
			this.materialApi = materialApi;
		}

		public override IIdeaMaterial Create(int id)
		{
			
			IFeaMaterial material = materialApi.GetMaterial(id);

			switch (material.MaterialType)
			{
				case MaterialType.Concrete:

					FeaMaterialConcrete materialConcrete = (FeaMaterialConcrete)material;

					MatConcreteCHN matConcrete = new MatConcreteCHN()
					{
						Fck = materialConcrete.Fck,
					};

					return new IdeaMaterialConcrete(id)
					{
						Name = materialConcrete.Name,
						Material = matConcrete,
					};

				case MaterialType.Steel:

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

			return null;

		}
	}
}