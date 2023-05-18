using IdeaStatiCa.BimApiLink.Identifiers;
using IdeaStatiCa.BimApi;

namespace IdeaStatiCa.BimApiLink.Importers
{
	public abstract class IntIdentifierImporter<T> : AbstractImporter<T>
		where T : IIdeaObject
	{
		public override T Create(Identifier<T> identifier)
		{
			return Create(((IntIdentifier<T>)identifier).Id);
		}

#if NET6_0_OR_GREATER
#nullable enable
		public abstract T? Create(int id);
#nullable disable
#else
		public abstract T Create(int id);
#endif
	}
}