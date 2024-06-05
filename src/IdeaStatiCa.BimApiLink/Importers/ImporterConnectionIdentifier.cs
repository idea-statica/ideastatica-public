using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.Identifiers;
using System;

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

		public override T Check(Identifier<T> identifier)
		{
			return Check(((ConnectionIdentifier<T>)identifier));
		}

		public virtual T Check(ConnectionIdentifier<T> id)
		{
			throw new NotImplementedException();
		}
	}
}