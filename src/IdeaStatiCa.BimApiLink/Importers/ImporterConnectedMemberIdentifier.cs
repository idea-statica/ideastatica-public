using IdeaStatiCa.BimApiLink.Identifiers;
using IdeaStatiCa.BimApi;

namespace IdeaStatiCa.BimApiLink.Importers
{
	public abstract class ImporterConnectedMemberIdentifier<T> : AbstractImporter<T>
		where T : IIdeaObject
	{
		public override T Create(Identifier<T> identifier)
		{
			return Create(((ConnectedMemberIdentifier<T>)identifier));
		}

		public abstract T Create(ConnectedMemberIdentifier<T> id);
	}
}