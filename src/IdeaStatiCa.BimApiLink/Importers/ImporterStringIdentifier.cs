using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.Identifiers;

namespace IdeaStatiCa.BimApiLink.Importers
{
	public abstract class StringIdentifierImporter<T> : AbstractImporter<T>
		where T : IIdeaObject
	{
		public override T Create(Identifier<T> identifier)
		{
			return Create(((StringIdentifier<T>)identifier).Id);
		}

		public abstract T Create(string id);

		public override T Check(Identifier<T> identifier)
		{
			return Check(((StringIdentifier<T>)identifier).Id);
		}

		//public abstract T Check(string id);
		public virtual T Check(string id)
		{
			return default(T);
		}
	}
}