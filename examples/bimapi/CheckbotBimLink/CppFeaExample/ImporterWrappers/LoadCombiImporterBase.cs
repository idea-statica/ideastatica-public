using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.Importers;

namespace ImporterWrappers
{
	public class LoadCombiImporterBase : IntIdentifierImporter<IIdeaCombiInput>
	{
		public override IIdeaCombiInput? Create(int id)
		{
			throw new NotImplementedException();
		}
	}
}
