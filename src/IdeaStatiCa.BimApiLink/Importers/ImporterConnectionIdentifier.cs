using IdeaStatiCa.BimApiLink.Identifiers;
using IdeaStatiCa.BimApi;

namespace IdeaStatiCa.BimApiLink.Importers
{
	public abstract class ImporterConnectionIdentifier<T> : AbstractImporter<T>
		where T : IIdeaObject
	{
		public override T Create(Identifier<T> identifier)
		{
			return Create(((ConnectionIdentifier<T>)identifier));
		}

		public abstract T Create(ConnectionIdentifier<T> id);
	}
}