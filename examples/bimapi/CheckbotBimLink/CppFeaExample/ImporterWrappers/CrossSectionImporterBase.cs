using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.Importers;

namespace ImporterWrappers
{
	public class CrossSectionImporterBase : IntIdentifierImporter<IIdeaCrossSection>
	{
		public CrossSectionImporterBase()
		{
		}

		public override IIdeaCrossSection? Create(int id)
		{
			throw new NotImplementedException();
		}
	}
}
