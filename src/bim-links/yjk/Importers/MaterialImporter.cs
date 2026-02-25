using IdeaStatiCa.BimApiLink.BimApi;
using IdeaStatiCa.BimApiLink.Importers;
using IdeaStatiCa.BimApi;

namespace yjk.Importers
{
	internal class MaterialImporter : IntIdentifierImporter<IIdeaMaterial>
	{
		public override IIdeaMaterial Create(int id)
		{
			return new IdeaMaterialByName(id)
			{
				MaterialType = MaterialType.Steel,
				Name = "S 355",
			};
		}
	}
}