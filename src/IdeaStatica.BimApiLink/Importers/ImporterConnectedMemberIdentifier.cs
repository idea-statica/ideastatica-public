using IdeaStatica.BimApiLink.Identifiers;
using IdeaStatiCa.BimApi;

namespace IdeaStatica.BimApiLink.Importers
{
	public abstract class ImporterConnectedMemberIdentifier<T> : AbstractImporter<T>
		where T : IIdeaObject
	{
		public override T Create(Identifier<T> identifier)
		{
			return Create(((ConnectedMemberIdentidier<T>)identifier));
		}

		public abstract T Create(ConnectedMemberIdentidier<T> id);
	}
}