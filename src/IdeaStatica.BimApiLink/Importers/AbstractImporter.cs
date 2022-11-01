using IdeaStatica.BimApiLink.Identifiers;
using IdeaStatiCa.BimApi;
using System;

namespace IdeaStatica.BimApiLink.Importers
{
	public abstract class AbstractImporter<T> : BimLinkObject, IImporter<T>
		where T : IIdeaObject
	{
		public TObj Create<TObj>(Identifier<TObj> identifier)
			where TObj : IIdeaObject
		{
			if (!(identifier is Identifier<T> id))
			{
				throw new ArgumentException();
			}

			T obj = Create(id);

			if (obj == null)
			{
				return default;
			}

			if (!(obj is TObj res))
			{
				throw new ArgumentException();
			}

			return res;
		}

		public IIdeaObject Create(IIdentifier identifier)
		{
			if (identifier is Identifier<T> id)
			{
				return Create(id);
			}

			throw new ArgumentException();
		}

		public abstract T Create(Identifier<T> identifier);
	}
}