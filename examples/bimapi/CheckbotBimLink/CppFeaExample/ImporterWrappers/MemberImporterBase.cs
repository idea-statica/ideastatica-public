using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.Importers;

namespace ImporterWrappers
{
	public class MemberImporterBase : IntIdentifierImporter<IIdeaMember1D>
	{
		public MemberImporterBase()
		{ }

		public override IIdeaMember1D? Create(int id)
		{
			throw new NotImplementedException();
		}
	}
}
