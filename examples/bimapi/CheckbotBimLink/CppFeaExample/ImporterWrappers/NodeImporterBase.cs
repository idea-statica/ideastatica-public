using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.Importers;

namespace ImporterWrappers
{
	public class NodeImporterBase : IntIdentifierImporter<IIdeaNode>
	{
		public NodeImporterBase()
		{
		}

		public override IIdeaNode? Create(int id)
		{
			throw new NotImplementedException();
		}
	}
}
