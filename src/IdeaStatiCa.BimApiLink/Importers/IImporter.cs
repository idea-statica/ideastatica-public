using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.Identifiers;

namespace IdeaStatiCa.BimApiLink.Importers
{
	public interface IImporter
	{
		T Create<T>(Identifier<T> identifier)
			where T : IIdeaObject;

		IIdeaObject Create(IIdentifier identifier);

		T Check<T>(Identifier<T> identifier)
			where T : IIdeaObject;

		IIdeaObject Check(IIdentifier identifier);
	}

	public interface IImporter<T> : IImporter
		where T : IIdeaObject
	{
		T Create(Identifier<T> identifier);

		T Check(Identifier<T> identifier);
	}
}