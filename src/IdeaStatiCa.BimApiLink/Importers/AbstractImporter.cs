using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.Identifiers;
using System;

namespace IdeaStatiCa.BimApiLink.Importers
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

			// tady to sleti - spatny importer
			throw new ArgumentException();
		}

		public abstract T Create(Identifier<T> identifier);


		public TObj Check<TObj>(Identifier<TObj> identifier)
			where TObj : IIdeaObject
		{
			if (!(identifier is Identifier<T> id))
			{
				throw new ArgumentException();
			}

			T obj = Check(id);

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

		public IIdeaObject Check(IIdentifier identifier)
		{
			if (identifier is Identifier<T> id)
			{
				return Check(id);
			}

			// tady to sleti - spatny importer
			throw new ArgumentException();
		}

		public virtual T Check(Identifier<T> identifier)
		{
			throw new NotImplementedException();
		}
	}
}