using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.Importers;

namespace ImporterWrappers
{
	public class MaterialImporterBase : IntIdentifierImporter<IIdeaMaterial>
	{
		public MaterialImporterBase()
		{
		}

		public override IIdeaMaterial? Create(int id)
		{
			throw new NotImplementedException();
		}
	}
}
