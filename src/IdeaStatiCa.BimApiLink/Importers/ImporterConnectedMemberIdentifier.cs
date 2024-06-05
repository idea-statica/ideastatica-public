using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.Identifiers;
using System;

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

		public override T Check(Identifier<T> identifier)
		{
			return Check(((ConnectedMemberIdentifier<T>)identifier));
		}

		public virtual T Check(ConnectedMemberIdentifier<T> id)
		{
			throw new NotImplementedException();
		}
	}
}