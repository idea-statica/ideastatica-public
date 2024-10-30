using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.Importers;

namespace ImporterWrappers
{
	public class LoadGroupImporterBase : IntIdentifierImporter<IIdeaLoadGroup>
	{
		public override IIdeaLoadGroup? Create(int id)
		{
			throw new NotImplementedException();
		}
	}
}
